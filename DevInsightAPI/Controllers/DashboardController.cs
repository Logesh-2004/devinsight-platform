using Microsoft.AspNetCore.Mvc;
using DevInsightAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace DevInsightAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var data = await _dashboardService.GetDashboardData();
            return Ok(data);
        }
    }
}
