using DevInsightAPI.Constants;
using DevInsightAPI.Data;
using DevInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DevInsightAPI.Services
{
    public class DbAppDataSeeder : IAppDataSeeder
    {
        private readonly DevInsightDbContext _context;
        private readonly IPasswordService _passwordService;

        public DbAppDataSeeder(DevInsightDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task EnsureSeedDataAsync()
        {
            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                user.Email = user.Email.Trim().ToLowerInvariant();
                user.Role = UserRoles.Normalize(user.Role);

                if (string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    user.PasswordHash = _passwordService.HashPassword(SeedDefaults.TemporaryPassword);
                }
            }

            EnsureSeedUser(users, SeedDefaults.AdminName, SeedDefaults.AdminEmail, UserRoles.Admin, SeedDefaults.AdminPassword);
            EnsureSeedUser(users, SeedDefaults.DeveloperName, SeedDefaults.DeveloperEmail, UserRoles.Developer, SeedDefaults.DeveloperPassword);

            await _context.SaveChangesAsync();
        }

        private void EnsureSeedUser(
            List<User> users,
            string name,
            string email,
            string role,
            string password)
        {
            var existing = users.FirstOrDefault(user =>
                user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                _context.Users.Add(new User
                {
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
