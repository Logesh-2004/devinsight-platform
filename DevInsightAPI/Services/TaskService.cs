using DevInsightAPI.Constants;
using DevInsightAPI.Repositories;
using DevInsightAPI.DTOs;
using DevInsightAPI.Mappings;
using DevInsightAPI.Models;

namespace DevInsightAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly ICurrentUserContext _currentUserContext;

        public TaskService(
            ITaskRepository repository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            INotificationService notificationService,
            IRealtimeNotifier realtimeNotifier,
            ICurrentUserContext currentUserContext)
        {
            _repository = repository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _notificationService = notificationService;
            _realtimeNotifier = realtimeNotifier;
            _currentUserContext = currentUserContext;
        }

        public async Task<List<TaskDTO>> GetAllTasks()
        {
            var tasks = await GetScopedTasksAsync();
            return tasks.Select(task => task.ToDto()).ToList();
        }

        public async Task<TaskDTO?> GetTaskById(int id)
        {
            var task = await _repository.GetByIdAsync(id);
            return task != null && HasTaskAccess(task)
                ? task.ToDto()
                : null;
        }

        public async Task<TaskDTO> CreateTask(CreateTaskDTO dto)
        {
            EnsureAdminAccess("Only admins can create tasks.");
            await ValidateTaskReferences(dto.ProjectId, dto.AssignedUserId);

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                ProjectId = dto.ProjectId,
                AssignedUserId = dto.AssignedUserId,
                Priority = NormalizePriority(dto.Priority),
                Status = NormalizeStatus(dto.Status),
                CreatedAt = DateTime.UtcNow,
                DueDate = dto.DueDate
            };

            ApplyCompletionTracking(task, previousStatus: string.Empty);

            var createdTask = await _repository.CreateAsync(task);

            if (createdTask.AssignedUserId.HasValue)
            {
                await _notificationService.NotifyTaskAssignedAsync(createdTask);
            }

            await _realtimeNotifier.NotifyTaskCreatedAsync(createdTask.Id, createdTask.AssignedUserId);
            return createdTask.ToDto();
        }

        public async Task<TaskDTO?> UpdateTask(int id, UpdateTaskDTO dto)
        {
            EnsureAdminAccess("Only admins can update task details.");

            var existingTask = await _repository.GetByIdAsync(id);

            if (existingTask == null)
            {
                return null;
            }

            await ValidateTaskReferences(dto.ProjectId, dto.AssignedUserId);

            var previousAssignedUserId = existingTask.AssignedUserId;
            var previousStatus = existingTask.Status;

            existingTask.Title = dto.Title.Trim();
            existingTask.Description = dto.Description.Trim();
            existingTask.ProjectId = dto.ProjectId;
            existingTask.AssignedUserId = dto.AssignedUserId;
            existingTask.Priority = NormalizePriority(dto.Priority);
            existingTask.Status = NormalizeStatus(dto.Status);
            existingTask.DueDate = dto.DueDate;
            ApplyCompletionTracking(existingTask, previousStatus);

            var updatedTask = await _repository.UpdateAsync(existingTask);

            if (previousAssignedUserId != updatedTask.AssignedUserId && updatedTask.AssignedUserId.HasValue)
            {
                await _notificationService.NotifyTaskAssignedAsync(updatedTask);
            }

            if (!string.Equals(previousStatus, updatedTask.Status, StringComparison.Ordinal))
            {
                await _notificationService.NotifyTaskStatusChangedAsync(updatedTask, previousStatus);
                await _realtimeNotifier.NotifyTaskMovedAsync(
                    updatedTask.Id,
                    previousStatus,
                    updatedTask.Status,
                    previousAssignedUserId,
                    updatedTask.AssignedUserId);
            }
            else
            {
                await _realtimeNotifier.NotifyTaskUpdatedAsync(
                    updatedTask.Id,
                    previousAssignedUserId,
                    updatedTask.AssignedUserId);
            }

            return updatedTask.ToDto();
        }

        public async Task<TaskDTO?> UpdateTaskStatus(int id, string status)
        {
            var task = await _repository.GetByIdAsync(id);

            if (task == null)
            {
                return null;
            }

            EnsureTaskStatusAccess(task);

            var previousStatus = task.Status;
            task.Status = NormalizeStatus(status);
            ApplyCompletionTracking(task, previousStatus);

            var updatedTask = await _repository.UpdateAsync(task);

            if (!string.Equals(previousStatus, updatedTask.Status, StringComparison.Ordinal))
            {
                await _notificationService.NotifyTaskStatusChangedAsync(updatedTask, previousStatus);
                await _realtimeNotifier.NotifyTaskMovedAsync(
                    updatedTask.Id,
                    previousStatus,
                    updatedTask.Status,
                    updatedTask.AssignedUserId);
            }

            return updatedTask.ToDto();
        }

        public async Task<bool> DeleteTask(int id)
        {
            EnsureAdminAccess("Only admins can delete tasks.");

            var task = await _repository.GetByIdAsync(id);

            if (task == null)
            {
                return false;
            }

            await _repository.DeleteAsync(task);
            return true;
        }

        private async Task<List<TaskItem>> GetScopedTasksAsync()
        {
            var tasks = await _repository.GetAllAsync();

            if (_currentUserContext.IsDeveloper && _currentUserContext.UserId.HasValue)
            {
                return tasks
                    .Where(task => task.AssignedUserId == _currentUserContext.UserId.Value)
                    .ToList();
            }

            return tasks;
        }

        private bool HasTaskAccess(TaskItem task)
        {
            if (_currentUserContext.IsAdmin)
            {
                return true;
            }

            return _currentUserContext.IsDeveloper &&
                   _currentUserContext.UserId.HasValue &&
                   task.AssignedUserId == _currentUserContext.UserId.Value;
        }

        private void EnsureTaskStatusAccess(TaskItem task)
        {
            if (HasTaskAccess(task))
            {
                return;
            }

            throw new UnauthorizedAccessException("You do not have permission to change this task.");
        }

        private void EnsureAdminAccess(string message)
        {
            if (!_currentUserContext.IsAdmin)
            {
                throw new UnauthorizedAccessException(message);
            }
        }

        private async Task ValidateTaskReferences(int projectId, int? assignedUserId)
        {
            if (!await _projectRepository.ExistsAsync(projectId))
            {
                throw new InvalidOperationException("The selected project does not exist.");
            }

            if (assignedUserId.HasValue && !await _userRepository.ExistsAsync(assignedUserId.Value))
            {
                throw new InvalidOperationException("The selected user does not exist.");
            }
        }

        private static string NormalizeStatus(string status)
        {
            if (!TaskMetadata.IsValidStatus(status))
            {
                throw new InvalidOperationException("The provided task status is invalid.");
            }

            return TaskMetadata.NormalizeStatus(status);
        }

        private static string NormalizePriority(string priority)
        {
            if (!TaskMetadata.IsValidPriority(priority))
            {
                throw new InvalidOperationException("The provided task priority is invalid.");
            }

            return TaskMetadata.NormalizePriority(priority);
        }

        private static void ApplyCompletionTracking(TaskItem task, string previousStatus)
        {
            var wasCompleted = string.Equals(previousStatus, "Done", StringComparison.Ordinal);
            var isCompleted = string.Equals(task.Status, "Done", StringComparison.Ordinal);

            if (isCompleted && !wasCompleted)
            {
                task.CompletedAt = DateTime.UtcNow;
                return;
            }

            if (!isCompleted)
            {
                task.CompletedAt = null;
            }
        }
    }
}
