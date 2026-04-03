using DevInsightAPI.DTOs;
using DevInsightAPI.Models;

namespace DevInsightAPI.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDTO>> GetCurrentUserNotificationsAsync();

        Task<NotificationDTO?> MarkAsReadAsync(int id);

        Task NotifyTaskAssignedAsync(TaskItem task);

        Task NotifyTaskStatusChangedAsync(TaskItem task, string previousStatus);
    }
}
