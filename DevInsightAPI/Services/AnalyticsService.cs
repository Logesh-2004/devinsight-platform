using DevInsightAPI.DTOs;
using DevInsightAPI.Repositories;

namespace DevInsightAPI.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserContext _currentUserContext;

        public AnalyticsService(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            ICurrentUserContext currentUserContext)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _currentUserContext = currentUserContext;
        }

        public async Task<AnalyticsDTO> GetAnalytics()
        {
            var tasks = await GetScopedTasksAsync();

            var usersById = (await _userRepository.GetAllAsync())
                .ToDictionary(user => user.Id, user => user.Name);

            var tasksByStatus = tasks
                .GroupBy(task => task.Status)
                .ToDictionary(group => group.Key, group => group.Count());

            var tasksPerDeveloper = tasks
                .Where(task => task.AssignedUserId.HasValue)
                .GroupBy(task => task.AssignedUserId!.Value)
                .ToDictionary(
                    group =>
                    {
                        if (usersById.TryGetValue(group.Key, out var name) &&
                            !string.IsNullOrWhiteSpace(name))
                        {
                            return $"{name} (#{group.Key})";
                        }

                        return $"User #{group.Key}";
                    },
                    group => group.Count());

            var tasksPerDay = tasks
                .GroupBy(task => task.CreatedAt.Date)
                .OrderBy(group => group.Key)
                .ToDictionary(
                    group => group.Key.ToString("yyyy-MM-dd"),
                    group => group.Count());

            return new AnalyticsDTO
            {
                TasksByStatus = tasksByStatus,
                TasksPerDeveloper = tasksPerDeveloper,
                TasksPerDay = tasksPerDay
            };
        }

        public async Task<ProjectAnalyticsDTO> GetProjectAnalytics(int projectId)
        {
            var tasks = (await GetScopedTasksAsync())
                .Where(task => task.ProjectId == projectId)
                .ToList();

            if (_currentUserContext.IsDeveloper && tasks.Count == 0)
            {
                throw new UnauthorizedAccessException("You do not have access to this project's analytics.");
            }

            int total = tasks.Count;

            int completed = tasks.Count(t => t.Status == "Done");

            int active = tasks.Count(t => t.Status == "InProgress");

            int overdue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Done");

            double progress = total == 0 ? 0 : (completed * 100.0 / total);

            string risk = overdue switch
            {
                0 => "Low",
                <= 2 => "Medium",
                _ => "High"
            };

            return new ProjectAnalyticsDTO
            {
                ProjectId = projectId,
                TotalTasks = total,
                CompletedTasks = completed,
                ActiveTasks = active,
                OverdueTasks = overdue,
                Progress = Math.Round(progress, 2),
                RiskLevel = risk
            };
        }
        public async Task<DeveloperAnalyticsDTO> GetDeveloperAnalytics(int userId)
        {
            if (_currentUserContext.IsDeveloper && _currentUserContext.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have access to another developer's analytics.");
            }

            var tasks = (await _taskRepository.GetAllAsync())
                .Where(task => task.AssignedUserId == userId)
                .ToList();

            int total = tasks.Count;

            int active = tasks.Count(t => t.Status == "InProgress");

            int completed = tasks.Count(t => t.Status == "Done");

            int overdue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "Done");

            string workload = active switch
            {
                <= 2 => "Light",
                <= 5 => "Normal",
                _ => "Overloaded"
            };

            return new DeveloperAnalyticsDTO
            {
                UserId = userId,
                TotalTasks = total,
                ActiveTasks = active,
                CompletedTasks = completed,
                OverdueTasks = overdue,
                WorkloadStatus = workload
            };
        }

        private async Task<List<Models.TaskItem>> GetScopedTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();

            if (_currentUserContext.IsDeveloper && _currentUserContext.UserId.HasValue)
            {
                return tasks
                    .Where(task => task.AssignedUserId == _currentUserContext.UserId.Value)
                    .ToList();
            }

            return tasks;
        }
    }
}
