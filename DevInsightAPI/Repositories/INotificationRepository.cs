using DevInsightAPI.Models;

namespace DevInsightAPI.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);

        Task<Notification?> GetByIdAsync(int id);

        Task<Notification> CreateAsync(Notification notification);

        Task<Notification> UpdateAsync(Notification notification);
    }
}
