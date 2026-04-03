using DevInsightAPI.DTOs;

namespace DevInsightAPI.Services
{
    public interface IRealtimeNotifier
    {
        Task NotifyTaskCreatedAsync(int taskId, params int?[] userIds);

        Task NotifyTaskUpdatedAsync(int taskId, params int?[] userIds);

        Task NotifyTaskMovedAsync(int taskId, string previousStatus, string currentStatus, params int?[] userIds);

        Task NotifyNotificationCreatedAsync(NotificationDTO notification);

        Task NotifyNotificationReadAsync(int userId, int notificationId);
    }
}
