using DevInsightAPI.Models;

namespace DevInsightAPI.Repositories
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync();

        Task<Project?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<Project> CreateAsync(Project project);

        Task<Project> UpdateAsync(Project project);

        Task DeleteAsync(Project project);
    }
}
