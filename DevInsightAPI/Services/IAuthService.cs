using DevInsightAPI.DTOs;

namespace DevInsightAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto);

        Task<UserSummaryDTO?> GetCurrentUserAsync();
    }
}
