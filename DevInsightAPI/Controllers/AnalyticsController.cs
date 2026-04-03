using Microsoft.AspNetCore.Mvc;
using DevInsightAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace DevInsightAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _service;

        public AnalyticsController(IAnalyticsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalytics()
        {
            var result = await _service.GetAnalytics();
            return Ok(result);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectAnalytics(int projectId)
        {
            var result = await _service.GetProjectAnalytics(projectId);
            return Ok(result);
        }

        [HttpGet("developer/{userId}")]
        public async Task<IActionResult> GetDeveloperAnalytics(int userId)
        {
            var result = await _service.GetDeveloperAnalytics(userId);
            return Ok(result);
        }
    }
}
