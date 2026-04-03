namespace DevInsightAPI.DTOs
{
    public class DashboardDTO
    {
        public int TotalProjects { get; set; }

        public int TotalUsers { get; set; }

        public int TotalTasks { get; set; }

        public int ActiveTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int OverdueTasks { get; set; }

        public double CompletionRate { get; set; }
    }
}
