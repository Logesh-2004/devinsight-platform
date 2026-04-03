using DevInsightAPI.Models;

namespace DevInsightAPI.Services.AIInsights
{
    public class AIInsightsContext
    {
        public IReadOnlyList<User> Users { get; init; } = [];

        public IReadOnlyList<Project> Projects { get; init; } = [];

        public IReadOnlyList<TaskItem> Tasks { get; init; } = [];

        public DateTime TodayUtc { get; init; }

        public DateTime StartOfPreviousWeekUtc { get; init; }

        public DateTime StartOfCurrentWeekUtc { get; init; }

        public DateTime StartOfNextWeekUtc { get; init; }
    }
}
