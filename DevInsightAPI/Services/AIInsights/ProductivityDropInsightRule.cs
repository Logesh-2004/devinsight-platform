namespace DevInsightAPI.Services.AIInsights
{
    public class ProductivityDropInsightRule : IAIInsightRule
    {
        public IEnumerable<string> Evaluate(AIInsightsContext context)
        {
            var completedThisWeek = CountCompletedTasks(
                context.Tasks,
                context.StartOfCurrentWeekUtc,
                context.StartOfNextWeekUtc);

            var completedLastWeek = CountCompletedTasks(
                context.Tasks,
                context.StartOfPreviousWeekUtc,
                context.StartOfCurrentWeekUtc);

            return completedThisWeek < completedLastWeek
                ? ["Productivity dropped this week"]
                : [];
        }

        private static int CountCompletedTasks(
            IEnumerable<DevInsightAPI.Models.TaskItem> tasks,
            DateTime fromUtc,
            DateTime toUtc)
        {
            return tasks.Count(task =>
                task.CompletedAt.HasValue &&
                task.CompletedAt.Value >= fromUtc &&
                task.CompletedAt.Value < toUtc);
        }
    }
}
