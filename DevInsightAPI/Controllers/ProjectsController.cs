using Microsoft.AspNetCore.Mvc;
using DevInsightAPI.Services;
using DevInsightAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using DevInsightAPI.Constants;

namespace DevInsightAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _service.GetAllProjects();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _service.GetProjectById(id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO dto)
        {
            try
            {
                var project = await _service.CreateProject(dto);
                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDTO dto)
        {
            try
            {
                var project = await _service.UpdateProject(id, dto);

                if (project == null)
                {
                    return NotFound();
                }

                return Ok(project);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var deleted = await _service.DeleteProject(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
