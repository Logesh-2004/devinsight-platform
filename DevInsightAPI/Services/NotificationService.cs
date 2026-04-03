using DevInsightAPI.DTOs;
using DevInsightAPI.Mappings;
using DevInsightAPI.Models;
using DevInsightAPI.Repositories;

namespace DevInsightAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public NotificationService(
            INotificationRepository notificationRepository,
            ICurrentUserContext currentUserContext,
            IRealtimeNotifier realtimeNotifier)
        {
            _notificationRepository = notificationRepository;
            _currentUserContext = currentUserContext;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<List<NotificationDTO>> GetCurrentUserNotificationsAsync()
        {
            EnsureAuthenticated();

            var notifications = await _notificationRepository.GetByUserIdAsync(_currentUserContext.UserId!.Value);
            return notifications.Select(notification => notification.ToDto()).ToList();
        }

        public async Task<NotificationDTO?> MarkAsReadAsync(int id)
        {
            EnsureAuthenticated();

            var notification = await _notificationRepository.GetByIdAsync(id);

            if (notification == null || notification.UserId != _currentUserContext.UserId)
            {
                return null;
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification = await _notificationRepository.UpdateAsync(notification);
                await _realtimeNotifier.NotifyNotificationReadAsync(notification.UserId, notification.Id);
            }

            return notification.ToDto();
        }

        public async Task NotifyTaskAssignedAsync(TaskItem task)
        {
            if (!task.AssignedUserId.HasValue)
            {
                return;
            }

            var projectName = string.IsNullOrWhiteSpace(task.Project?.Name)
                ? "the workspace"
                : task.Project!.Name;

            await CreateNotificationAsync(
                task.AssignedUserId.Value,
                $"You were assigned \"{task.Title}\" in {projectName}.");
        }

        public async Task NotifyTaskStatusChangedAsync(TaskItem task, string previousStatus)
        {
            var recipientIds = new HashSet<int>();

            if (task.AssignedUserId.HasValue)
            {
                recipientIds.Add(task.AssignedUserId.Value);
            }

            if (task.Project?.CreatedByUserId.HasValue == true)
            {
                recipientIds.Add(task.Project.CreatedByUserId.Value);
            }

            foreach (var userId in recipientIds)
            {
                await CreateNotificationAsync(
                    userId,
                    $"Task \"{task.Title}\" moved from {previousStatus} to {task.Status}.");
            }
        }

        private async Task<NotificationDTO> CreateNotificationAsync(int userId, string message)
        {
            var notification = await _notificationRepository.CreateAsync(new Notification
            {
                Message = message,
                UserId = userId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            var dto = notification.ToDto();
            await _realtimeNotifier.NotifyNotificationCreatedAsync(dto);
            return dto;
        }

        private void EnsureAuthenticated()
        {
            if (!_currentUserContext.IsAuthenticated || !_currentUserContext.UserId.HasValue)
            {
                throw new UnauthorizedAccessException("You must be signed in to access notifications.");
            }
        }
    }
}
