namespace DevInsightAPI.Services.AIInsights
{
    public class DelayedTaskInsightRule : IAIInsightRule
    {
        public IEnumerable<string> Evaluate(AIInsightsContext context)
        {
            return context.Tasks
                .Where(task =>
                    !string.Equals(task.Status, "Done", StringComparison.Ordinal) &&
                    task.DueDate.Date < context.TodayUtc)
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Title, StringComparer.OrdinalIgnoreCase)
                .Select(task => $"Task {task.Title} is delayed")
                .ToList();
        }
    }
}
