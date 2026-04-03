namespace DevInsightAPI.DTOs
{
    public class DeveloperAnalyticsDTO
    {
        public int UserId { get; set; }

        public int TotalTasks { get; set; }

        public int ActiveTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int OverdueTasks { get; set; }

        public string WorkloadStatus { get; set; } = string.Empty;
    }
}