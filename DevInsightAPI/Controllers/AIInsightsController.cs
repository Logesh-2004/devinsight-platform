using DevInsightAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/ai/insights")]
    public class AIInsightsController : ControllerBase
    {
        private readonly IAIInsightsService _service;

        public AIInsightsController(IAIInsightsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetInsights()
        {
            var insights = await _service.GetInsightsAsync();
            return Ok(insights);
        }
    }
}
