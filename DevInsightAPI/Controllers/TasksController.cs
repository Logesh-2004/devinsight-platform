using Microsoft.AspNetCore.Mvc;
using DevInsightAPI.Services;
using DevInsightAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using DevInsightAPI.Constants;

namespace DevInsightAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;

        public TasksController(ITaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _service.GetAllTasks();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _service.GetTaskById(id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
        {
            try
            {
                var task = await _service.CreateTask(dto);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDTO dto)
        {
            try
            {
                var task = await _service.UpdateTask(id, dto);

                if (task == null)
                {
                    return NotFound();
                }

                return Ok(task);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDTO dto)
        {
            try
            {
                var task = await _service.UpdateTaskStatus(id, dto.Status);

                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _service.DeleteTask(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
