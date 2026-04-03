using DevInsightAPI.Models;
using DevInsightAPI.Persistence;

namespace DevInsightAPI.Repositories
{
    public class FileTaskRepository : ITaskRepository
    {
        private readonly FileWorkspaceStore _store;

        public FileTaskRepository(FileWorkspaceStore store)
        {
            _store = store;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Tasks
                .OrderBy(task => task.Status)
                .ThenBy(task => task.DueDate)
                .ToList();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Tasks
                .FirstOrDefault(task => task.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var data = await _store.ReadAsync();
            return data.Tasks.Any(task => task.Id == id);
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            var newId = await _store.UpdateAsync(data =>
            {
                var nextId = data.Tasks.Count == 0 ? 1 : data.Tasks.Max(item => item.Id) + 1;
                task.Id = nextId;

                data.Tasks.Add(new StoredTask
                {
                    Id = nextId,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    ProjectId = task.ProjectId,
                    AssignedUserId = task.AssignedUserId,
                    CreatedAt = task.CreatedAt,
                    DueDate = task.DueDate,
                    CompletedAt = task.CompletedAt
                });

                return nextId;
            });

            return (await GetByIdAsync(newId))!;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            await _store.UpdateAsync(data =>
            {
                var existing = data.Tasks.First(item => item.Id == task.Id);
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.Status = task.Status;
                existing.Priority = task.Priority;
                existing.ProjectId = task.ProjectId;
                existing.AssignedUserId = task.AssignedUserId;
                existing.DueDate = task.DueDate;
                existing.CompletedAt = task.CompletedAt;
                return 0;
            });

            return (await GetByIdAsync(task.Id))!;
        }

        public async Task DeleteAsync(TaskItem task)
        {
            await _store.UpdateAsync(data =>
            {
                data.Tasks.RemoveAll(item => item.Id == task.Id);
                return 0;
            });
        }
    }
}
