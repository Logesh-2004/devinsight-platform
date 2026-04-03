using DevInsightAPI.Constants;
using DevInsightAPI.Persistence;

namespace DevInsightAPI.Services
{
    public class FileAppDataSeeder : IAppDataSeeder
    {
        private readonly FileWorkspaceStore _store;
        private readonly IPasswordService _passwordService;

        public FileAppDataSeeder(FileWorkspaceStore store, IPasswordService passwordService)
        {
            _store = store;
            _passwordService = passwordService;
        }

        public async Task EnsureSeedDataAsync()
        {
            await _store.UpdateAsync(data =>
            {
                data.Notifications ??= [];

                foreach (var user in data.Users)
                {
                    user.Email = user.Email.Trim().ToLowerInvariant();
                    user.Role = UserRoles.Normalize(user.Role);

                    if (string.IsNullOrWhiteSpace(user.PasswordHash))
                    {
                        user.PasswordHash = _passwordService.HashPassword(SeedDefaults.TemporaryPassword);
                    }
                }

                EnsureSeedUser(data, SeedDefaults.AdminName, SeedDefaults.AdminEmail, UserRoles.Admin, SeedDefaults.AdminPassword);
                EnsureSeedUser(data, SeedDefaults.DeveloperName, SeedDefaults.DeveloperEmail, UserRoles.Developer, SeedDefaults.DeveloperPassword);

                return 0;
            });
        }

        private void EnsureSeedUser(
            WorkspaceDataFile data,
            string name,
            string email,
            string role,
            string password)
        {
            var existing = data.Users.FirstOrDefault(user =>
                user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                var nextId = data.Users.Count == 0 ? 1 : data.Users.Max(user => user.Id) + 1;

                data.Users.Add(new StoredUser
                {
                    Id = nextId,
                    Name = name,
                    Email = email,
                    Role = role,
                    PasswordHash = _passwordService.HashPassword(password),
                    CreatedAt = DateTime.UtcNow
                });

                return;
            }

            existing.Name = string.IsNullOrWhiteSpace(existing.Name) ? name : existing.Name;
            existing.Email = email;
            existing.Role = role;

            if (string.IsNullOrWhiteSpace(existing.PasswordHash))
            {
                existing.PasswordHash = _passwordService.HashPassword(password);
            }
        }
    }
}
