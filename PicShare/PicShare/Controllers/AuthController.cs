using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace PicShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService service;

        public AuthController(IUserService service)
        {
            this.service = service;
        }

        [HttpPost("signin")]
        public async Task<IActionResult?> SignIn([FromBody] UserSignInDto userSignIn)
        {
            var token = await service.SignInAsync(userSignIn);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(token);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpDto userSignUp)
        {
            var user = await service.AddAsync(userSignUp);
            if (user == null)
            {
                return BadRequest("Failed to create user");
            }
            return Ok(user.Id);
        }

        [HttpPost("signout")]
        public new IActionResult SignOut()
        {
            // Note: With JWT authentication, the main logout action happens client-side
            // by removing the token from storage. This endpoint exists for:
            // 1. API consistency - keeping all auth-related endpoints together
            // 2. Future extensibility - e.g., token blacklisting, analytics
            // 3. Proper HTTP semantics - allowing clients to signal logout intent

            return Ok("Successfully signed out");
        }
    }
}
