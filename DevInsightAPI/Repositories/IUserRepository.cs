using DevInsightAPI.Models;

namespace DevInsightAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> ExistsAsync(int id);

        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

        Task<User> CreateAsync(User user);

        Task<User> UpdateAsync(User user);

        Task DeleteAsync(User user);
    }
}
