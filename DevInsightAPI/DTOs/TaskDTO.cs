namespace DevInsightAPI.DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public int? AssignedUserId { get; set; }

        public UserSummaryDTO? AssignedUser { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime DueDate { get; set; }
    }
}
