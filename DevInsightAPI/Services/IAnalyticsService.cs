using DevInsightAPI.DTOs;

namespace DevInsightAPI.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsDTO> GetAnalytics();

        Task<ProjectAnalyticsDTO> GetProjectAnalytics(int projectId);

        Task<DeveloperAnalyticsDTO> GetDeveloperAnalytics(int userId);
    }
}
