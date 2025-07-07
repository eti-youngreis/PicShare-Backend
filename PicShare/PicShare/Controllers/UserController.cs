using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;

namespace PicShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await service.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving users: {ex.Message}");
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult?> UpdateProfile([FromForm] UserUpdateDto userUpdate)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.Claims.FirstOrDefault(x => x.Type == "id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            return Ok(await service.UpdateAsync(userId, userUpdate));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<UserResponseDto?> GetProfile()
        {
            return await service.GetUserFromClaimsAsync(HttpContext.User);
        }
    }
}
