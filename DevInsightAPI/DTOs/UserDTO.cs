namespace DevInsightAPI.DTOs
{
    public class UserDTO : UserSummaryDTO
    {
        public DateTime CreatedAt { get; set; }

        public int AssignedTaskCount { get; set; }

        public int CreatedProjectCount { get; set; }
    }
}
