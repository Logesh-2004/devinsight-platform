using System.Security.Claims;
using DevInsightAPI.Constants;

namespace DevInsightAPI.Services
{
    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var rawValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(rawValue, out var userId) ? userId : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        public string? Role => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

        public bool IsAdmin => string.Equals(Role, UserRoles.Admin, StringComparison.OrdinalIgnoreCase);

        public bool IsDeveloper => string.Equals(Role, UserRoles.Developer, StringComparison.OrdinalIgnoreCase);
    }
}
