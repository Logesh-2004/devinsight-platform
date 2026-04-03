using DevInsightAPI.Models;

namespace DevInsightAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllAsync();

        Task<TaskItem?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<TaskItem> CreateAsync(TaskItem task);

        Task<TaskItem> UpdateAsync(TaskItem task);

        Task DeleteAsync(TaskItem task);
    }
}
