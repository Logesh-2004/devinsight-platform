namespace DevInsightAPI.DTOs
{
    public class AnalyticsDTO
    {
        public Dictionary<string, int> TasksByStatus { get; set; } = new();

        public Dictionary<string, int> TasksPerDeveloper { get; set; } = new();

        public Dictionary<string, int> TasksPerDay { get; set; } = new();
    }
}
