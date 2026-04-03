using DevInsightAPI.Models;
using DevInsightAPI.Persistence;

namespace DevInsightAPI.Repositories
{
    public class FileNotificationRepository : INotificationRepository
    {
        private readonly FileWorkspaceStore _store;

        public FileNotificationRepository(FileWorkspaceStore store)
        {
            _store = store;
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId)
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Notifications
                .Where(notification => notification.UserId == userId)
                .OrderByDescending(notification => notification.CreatedAt)
                .ToList();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Notifications
                .FirstOrDefault(notification => notification.Id == id);
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            var newId = await _store.UpdateAsync(data =>
            {
                var nextId = data.Notifications.Count == 0
                    ? 1
                    : data.Notifications.Max(item => item.Id) + 1;

                notification.Id = nextId;

                data.Notifications.Add(new StoredNotification
                {
                    Id = nextId,
                    Message = notification.Message,
                    UserId = notification.UserId,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt
                });

                return nextId;
            });

            return (await GetByIdAsync(newId))!;
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            await _store.UpdateAsync(data =>
            {
                var existing = data.Notifications.First(item => item.Id == notification.Id);
                existing.Message = notification.Message;
                existing.UserId = notification.UserId;
                existing.IsRead = notification.IsRead;
                existing.CreatedAt = notification.CreatedAt;
                return 0;
            });

            return (await GetByIdAsync(notification.Id))!;
        }
    }
}
