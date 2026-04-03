using DevInsightAPI.DTOs;

namespace DevInsightAPI.Services
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardData();
    }
}