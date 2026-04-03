using DevInsightAPI.Models;
using DevInsightAPI.Persistence;

namespace DevInsightAPI.Repositories
{
    public class FileProjectRepository : IProjectRepository
    {
        private readonly FileWorkspaceStore _store;

        public FileProjectRepository(FileWorkspaceStore store)
        {
            _store = store;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Projects
                .OrderByDescending(project => project.CreatedAt)
                .ToList();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Projects
                .FirstOrDefault(project => project.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var data = await _store.ReadAsync();
            return data.Projects.Any(project => project.Id == id);
        }

        public async Task<Project> CreateAsync(Project project)
        {
            var newId = await _store.UpdateAsync(data =>
            {
                var nextId = data.Projects.Count == 0 ? 1 : data.Projects.Max(item => item.Id) + 1;
                project.Id = nextId;

                data.Projects.Add(new StoredProject
                {
                    Id = nextId,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt,
                    CreatedByUserId = project.CreatedByUserId
                });

                return nextId;
            });

            return (await GetByIdAsync(newId))!;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            await _store.UpdateAsync(data =>
            {
                var existing = data.Projects.First(item => item.Id == project.Id);
                existing.Name = project.Name;
                existing.Description = project.Description;
                existing.CreatedByUserId = project.CreatedByUserId;
                return 0;
            });

            return (await GetByIdAsync(project.Id))!;
        }

        public async Task DeleteAsync(Project project)
        {
            await _store.UpdateAsync(data =>
            {
                data.Projects.RemoveAll(item => item.Id == project.Id);
                data.Tasks.RemoveAll(task => task.ProjectId == project.Id);
                return 0;
            });
        }
    }
}
