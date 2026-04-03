using DevInsightAPI.Constants;
using DevInsightAPI.Repositories;
using DevInsightAPI.DTOs;
using DevInsightAPI.Mappings;
using DevInsightAPI.Models;

namespace DevInsightAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordService _passwordService;

        public UserService(IUserRepository repository, IPasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(user => user.ToDto()).ToList();
        }

        public async Task<UserDTO?> GetUserById(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user?.ToDto();
        }

        public async Task<UserDTO> CreateUser(CreateUserDTO dto)
        {
            await ValidateUser(dto.Email);

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                Role = NormalizeRole(dto.Role),
                PasswordHash = _passwordService.HashPassword(ResolvePassword(dto.Password)),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _repository.CreateAsync(user);
            return createdUser.ToDto();
        }

        public async Task<UserDTO?> UpdateUser(int id, UpdateUserDTO dto)
        {
            var existingUser = await _repository.GetByIdAsync(id);

            if (existingUser == null)
            {
                return null;
            }

            await ValidateUser(dto.Email, id);

            existingUser.Name = dto.Name.Trim();
            existingUser.Email = dto.Email.Trim().ToLowerInvariant();
            existingUser.Role = NormalizeRole(dto.Role);

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                existingUser.PasswordHash = _passwordService.HashPassword(ResolvePassword(dto.Password));
            }

            var updatedUser = await _repository.UpdateAsync(existingUser);
            return updatedUser.ToDto();
        }

        public async Task<bool> DeleteUser(int id)
        {
            var existingUser = await _repository.GetByIdAsync(id);

            if (existingUser == null)
            {
                return false;
            }

            await _repository.DeleteAsync(existingUser);
            return true;
        }

        private async Task ValidateUser(string email, int? existingUserId = null)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (await _repository.EmailExistsAsync(normalizedEmail, existingUserId))
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }
        }

        private static string NormalizeRole(string role)
        {
            if (!UserRoles.IsValid(role))
            {
                throw new InvalidOperationException("The selected user role is invalid.");
            }

            return UserRoles.Normalize(role);
        }

        private static string ResolvePassword(string? password)
        {
            var resolvedPassword = string.IsNullOrWhiteSpace(password)
                ? SeedDefaults.TemporaryPassword
                : password.Trim();

            if (resolvedPassword.Length < 8)
            {
                throw new InvalidOperationException("User passwords must be at least 8 characters long.");
            }

            return resolvedPassword;
        }
    }
}
