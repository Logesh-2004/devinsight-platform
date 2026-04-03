using DevInsightAPI.DTOs;
using DevInsightAPI.Mappings;
using DevInsightAPI.Repositories;

namespace DevInsightAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICurrentUserContext _currentUserContext;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtTokenService jwtTokenService,
            ICurrentUserContext currentUserContext)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtTokenService = jwtTokenService;
            _currentUserContext = currentUserContext;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user == null || !_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return null;
            }

            var (accessToken, expiresAt) = _jwtTokenService.CreateToken(user);

            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                User = user.ToSummaryDto()
            };
        }

        public async Task<UserSummaryDTO?> GetCurrentUserAsync()
        {
            if (!_currentUserContext.UserId.HasValue)
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(_currentUserContext.UserId.Value);
            return user?.ToSummaryDto();
        }
    }
}
