namespace DevInsightAPI.Services
{
    public interface IAIInsightsService
    {
        Task<List<string>> GetInsightsAsync();
    }
}
