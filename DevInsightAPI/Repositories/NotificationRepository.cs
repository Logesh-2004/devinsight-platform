using DevInsightAPI.Data;
using DevInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DevInsightAPI.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DevInsightDbContext _context;

        public NotificationRepository(DevInsightDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(notification => notification.UserId == userId)
                .OrderByDescending(notification => notification.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(notification => notification.Id == id);
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(notification.Id))!;
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(notification.Id))!;
        }
    }
}
