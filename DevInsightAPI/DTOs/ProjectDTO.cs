namespace DevInsightAPI.DTOs
{
    public class ProjectDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int? CreatedByUserId { get; set; }

        public UserSummaryDTO? CreatedByUser { get; set; }

        public int TaskCount { get; set; }

        public int CompletedTaskCount { get; set; }
    }
}
