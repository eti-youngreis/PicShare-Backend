using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository.Repository;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
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
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {

            List<SecureUser> secureUsers = new();
            var users = await service.GetAllAsync();
            users.ForEach(user => secureUsers.Add(
                new SecureUser
                {
                    ProfileImagePath = user.ProfileImagePath,
                    Id = user.Id ?? 0,
                    FullName = user.FullName,
                    Email = user.Email
                }));
            return Ok(secureUsers);
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
        public async Task<int?> SignUp([FromForm] UserDto entity)
        {
            return (await service.AddAsync(entity))?.Id;
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<UserDto?> Delete(int id)
        {
            return await service.DeleteAsync(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult?> UpdateProfile(int id, [FromForm] UserUpdate entity)
        {
            var userToUpdate = await service.GetByIdAsync(id);
            if (userToUpdate == null)
                return NoContent();
            userToUpdate.FullName = entity.FullName ?? userToUpdate.FullName;
            userToUpdate.ProfileImage = entity.ProfileImage ?? userToUpdate?.ProfileImage;
            return Ok(await service.UpdateAsync(id, userToUpdate));
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
            new Claim(ClaimTypes.NameIdentifier, (user.Id??0).ToString()),
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
        public async Task<SecureUser?> GetByToken()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var userClaim = identity.Claims;
                var user = await service.GetByIdAsync(int.Parse(userClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value));
                if (user != null)
                    return new SecureUser
                    {
                        ProfileImagePath = user.ProfileImagePath,
                        Email = user.Email,
                        Id = user.Id ?? 0,
                        FullName = user.FullName
                    };
            }
            return null;
        }
    }
}
