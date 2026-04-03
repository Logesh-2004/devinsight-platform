namespace DevInsightAPI.DTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public int UserId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
