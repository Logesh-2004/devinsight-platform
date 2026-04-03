using DevInsightAPI.Models;
using DevInsightAPI.Persistence;

namespace DevInsightAPI.Repositories
{
    public class FileUserRepository : IUserRepository
    {
        private readonly FileWorkspaceStore _store;

        public FileUserRepository(FileWorkspaceStore store)
        {
            _store = store;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Users
                .OrderBy(user => user.Name)
                .ToList();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Users
                .FirstOrDefault(user => user.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var data = await _store.ReadAsync();
            return WorkspaceGraph.From(data).Users
                .FirstOrDefault(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var data = await _store.ReadAsync();
            return data.Users.Any(user => user.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            var data = await _store.ReadAsync();
            return data.Users.Any(user =>
                user.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                (!excludeUserId.HasValue || user.Id != excludeUserId.Value));
        }

        public async Task<User> CreateAsync(User user)
        {
            var newId = await _store.UpdateAsync(data =>
            {
                var nextId = data.Users.Count == 0 ? 1 : data.Users.Max(item => item.Id) + 1;
                user.Id = nextId;

                data.Users.Add(new StoredUser
                {
                    Id = nextId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    PasswordHash = user.PasswordHash,
                    CreatedAt = user.CreatedAt
                });

                return nextId;
            });

            return (await GetByIdAsync(newId))!;
        }

        public async Task<User> UpdateAsync(User user)
        {
            await _store.UpdateAsync(data =>
            {
                var existing = data.Users.First(item => item.Id == user.Id);
                existing.Name = user.Name;
                existing.Email = user.Email;
                existing.Role = user.Role;
                existing.PasswordHash = user.PasswordHash;
                return 0;
            });

            return (await GetByIdAsync(user.Id))!;
        }

        public async Task DeleteAsync(User user)
        {
            await _store.UpdateAsync(data =>
            {
                data.Users.RemoveAll(item => item.Id == user.Id);

                foreach (var task in data.Tasks.Where(task => task.AssignedUserId == user.Id))
                {
                    task.AssignedUserId = null;
                }

                foreach (var project in data.Projects.Where(project => project.CreatedByUserId == user.Id))
                {
                    project.CreatedByUserId = null;
                }

                return 0;
            });
        }
    }
}
