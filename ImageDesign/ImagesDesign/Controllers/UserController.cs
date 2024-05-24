using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImagesDesign.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IService<UserDto> service;

        public UserController(IService<UserDto> service, IConfiguration _config)
        {
            this._config = _config;
            this.service = service;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult?> SignIn([FromBody] UserLogin userLogin)
        {
            var user = await Authenticate(userLogin);
            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }
            return NoContent();
        }
        // POST api/<UserController>
        [HttpPost("SignUp")]
        public async Task<int?> SignUp([FromBody] UserDto entity)
        {
            return (await service.AddAsync(entity))?.Id;
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<UserDto> Put(int id, UserDto entity)
        {
            return await service.UpdateAsync(id, entity);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<UserDto?> Delete(int id)
        {
            return await service.DeleteAsync(id);
        }

        private async Task<UserDto?> Authenticate(UserLogin userLogin)
        {
            return (await service.GetAllAsync()).FirstOrDefault(
                user => userLogin.GetType().GetProperties().All(prop => prop.GetValue(userLogin)?.ToString() ==
                user.GetType().GetProperty(prop.Name)?.GetValue(user)?.ToString()));
        }

        private string Generate(UserDto user)
        {
            //מפתח להצפנה
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            //אלגוריתם להצפנה
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(ClaimTypes.Name,user.FullName),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.UserData,user.Password)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
 
        [HttpGet]
        [Authorize]
        public object? GetByToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                return new  {
                    FullName = userClaim.FirstOrDefault(x=>x.Type==ClaimTypes.Name)!.Value,
                    Id = int.Parse(userClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value),
                };
            }
            return null;
        }
    }
}
