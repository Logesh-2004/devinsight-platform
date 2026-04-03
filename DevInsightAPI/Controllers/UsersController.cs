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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _service.GetUserById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            try
            {
                var user = await _service.CreateUser(dto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                var user = await _service.UpdateUser(id, dto);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _service.DeleteUser(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
