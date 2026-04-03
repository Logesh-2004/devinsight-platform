using DevInsightAPI.Models;

namespace DevInsightAPI.Services
{
    public interface IJwtTokenService
    {
        (string AccessToken, DateTime ExpiresAt) CreateToken(User user);
    }
}
