namespace DevInsightAPI.Services.AIInsights
{
    public interface IAIInsightRule
    {
        IEnumerable<string> Evaluate(AIInsightsContext context);
    }
}
