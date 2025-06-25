using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
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

        [HttpPost("SignIn")]
        public async Task<IActionResult?> SignIn([FromBody] UserSignInDto userSignIn)
        {
            var token = await service.SignInAsync(userSignIn);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(token);
        }

        // POST api/<UserController>
        [HttpPost("SignUp")]
        public async Task<int?> SignUp([FromBody] UserSignUpDto userSignUp)
        {
            return (await service.AddAsync(userSignUp))?.Id;
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<UserResponseDto?> Delete(int id)
        {
            return await service.DeleteAsync(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult?> UpdateProfile(int id, [FromForm] UserUpdateDto userUpdate)
        {
            return Ok(await service.UpdateAsync(id, userUpdate));
        }

        [HttpGet]
        [Authorize]
        public async Task<UserResponseDto?> GetByToken()
        {
            return await service.GetUserFromClaimsAsync(HttpContext.User);
        }
    }
}
