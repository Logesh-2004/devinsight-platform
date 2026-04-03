namespace DevInsightAPI.DTOs
{
    public class ProjectAnalyticsDTO
    {
        public int ProjectId { get; set; }

        public int TotalTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int ActiveTasks { get; set; }

        public int OverdueTasks { get; set; }

        public double Progress { get; set; }

        public string RiskLevel { get; set; } = string.Empty;
    }
}