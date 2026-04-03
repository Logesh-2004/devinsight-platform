using DevInsightAPI.Data;
using DevInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DevInsightAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DevInsightDbContext _context;

        public UserRepository(DevInsightDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(user => user.AssignedTasks)
                .Include(user => user.CreatedProjects)
                .OrderBy(user => user.Name)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(user => user.AssignedTasks)
                .Include(user => user.CreatedProjects)
                .Include(user => user.Notifications)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(user => user.AssignedTasks)
                .Include(user => user.CreatedProjects)
                .Include(user => user.Notifications)
                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(user => user.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            return await _context.Users.AnyAsync(user =>
                user.Email == email &&
                (!excludeUserId.HasValue || user.Id != excludeUserId.Value));
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(user.Id))!;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(user.Id))!;
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
