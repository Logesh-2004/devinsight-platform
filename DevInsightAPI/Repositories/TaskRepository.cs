using DevInsightAPI.Data;
using DevInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DevInsightAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DevInsightDbContext _context;

        public TaskRepository(DevInsightDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(task => task.Project)
                .Include(task => task.AssignedUser)
                .OrderBy(task => task.Status)
                .ThenBy(task => task.DueDate)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(task => task.Project)
                .Include(task => task.AssignedUser)
                .FirstOrDefaultAsync(task => task.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tasks.AnyAsync(task => task.Id == id);
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(task.Id))!;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(task.Id))!;
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
