using DevInsightAPI.Constants;
using DevInsightAPI.DTOs;
using DevInsightAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DevInsightAPI.Services
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public SignalRRealtimeNotifier(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyTaskCreatedAsync(int taskId, params int?[] userIds)
        {
            return NotifyTaskEventAsync(HubEventNames.TaskCreated, new TaskRealtimeEventDTO
            {
                TaskId = taskId
            }, userIds);
        }

        public Task NotifyTaskUpdatedAsync(int taskId, params int?[] userIds)
        {
            return NotifyTaskEventAsync(HubEventNames.TaskUpdated, new TaskRealtimeEventDTO
            {
                TaskId = taskId
            }, userIds);
        }

        public Task NotifyTaskMovedAsync(int taskId, string previousStatus, string currentStatus, params int?[] userIds)
        {
            return NotifyTaskEventAsync(HubEventNames.TaskMoved, new TaskMovedRealtimeEventDTO
            {
                TaskId = taskId,
                PreviousStatus = previousStatus,
                CurrentStatus = currentStatus
            }, userIds);
        }

        public Task NotifyNotificationCreatedAsync(NotificationDTO notification)
        {
            return _hubContext.Clients.Group(HubEventNames.UserGroup(notification.UserId))
                .SendAsync(HubEventNames.NotificationCreated, notification);
        }

        public Task NotifyNotificationReadAsync(int userId, int notificationId)
        {
            return _hubContext.Clients.Group(HubEventNames.UserGroup(userId))
                .SendAsync(HubEventNames.NotificationRead, notificationId);
        }

        private Task NotifyTaskEventAsync(string eventName, object payload, params int?[] userIds)
        {
            var groups = userIds
                .Where(userId => userId.HasValue)
                .Select(userId => HubEventNames.UserGroup(userId!.Value))
                .Distinct()
                .Append(HubEventNames.AdminGroup)
                .ToArray();

            return _hubContext.Clients.Groups(groups).SendAsync(eventName, payload);
        }
    }
}
