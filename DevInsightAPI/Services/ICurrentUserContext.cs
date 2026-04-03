namespace DevInsightAPI.Services
{
    public interface ICurrentUserContext
    {
        int? UserId { get; }

        string? Email { get; }

        string? Role { get; }

        bool IsAuthenticated { get; }

        bool IsAdmin { get; }

        bool IsDeveloper { get; }
    }
}
