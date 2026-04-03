namespace DevInsightAPI.DTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public UserSummaryDTO User { get; set; } = new();
    }
}
