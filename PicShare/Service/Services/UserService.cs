using AutoMapper;
using Common.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entity;
using Repository.Interfaces;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;
        private readonly IConfiguration config;

        public UserService(IRepository<User> repository, IMapper mapper, IConfiguration config)
        {
            this.config = config;
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<UserResponseDto?> AddAsync(UserSignupDto entity)
        {
            return mapper.Map<UserResponseDto>(await repository.AddAsync(mapper.Map<User>(entity)));
        }

        public async Task<UserResponseDto?> DeleteAsync(int id)
        {
            return mapper.Map<UserResponseDto>(await repository.DeleteByIdAsync(id));
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            return mapper.Map<List<UserResponseDto>>(await repository.GetAllAsync());
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            return mapper.Map<UserResponseDto>(await repository.GetByIdAsync(id));
        }
        public async Task<string?> LoginAsync(UserLoginDto userLogin)
        {
            var user = await Authenticate(userLogin);
            if (user != null)
            {
                var token = Generate(user);
                return token;
            }
            return null;
        }

        public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto userToUpdate)
        {
            var user = await repository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            if (userToUpdate.ProfileImage != null && userToUpdate.ProfileImage.Length != 0 &&
                (userToUpdate.ProfileImage.ContentType.StartsWith("image/") || userToUpdate.ProfileImage.ContentType.StartsWith("png/")))
            {
                user.ProfileImagePath = await UpdateProfileImageAsync(userToUpdate.ProfileImage, user.ProfileImagePath ?? "");
            }

            if (!string.IsNullOrEmpty(userToUpdate.FullName))
            {
                user.FullName = userToUpdate.FullName;
            }

            if (!string.IsNullOrEmpty(userToUpdate.Password))
            {
                user.Password = userToUpdate.Password;
            }

            await repository.UpdateAsync(id, user);
            return mapper.Map<UserResponseDto>(user);
        }


        private static async Task<string> UpdateProfileImageAsync(IFormFile newImage, string oldImagePath = "")
        {
            if (newImage == null || newImage.Length == 0 ||
                !(newImage.ContentType.StartsWith("image/") || newImage.ContentType.StartsWith("png/")))
            {
                return oldImagePath;
            }

            if (!string.IsNullOrEmpty(oldImagePath))
            {
                var oldImageFileName = Path.GetFileName(oldImagePath);
                var oldImageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images", oldImageFileName);
                if (File.Exists(oldImageFullPath))
                {
                    File.Delete(oldImageFullPath);
                }
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + newImage.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await newImage.CopyToAsync(stream);
            }

            var baseUrl = "https://localhost:44357";
            var imageUrl = $"{baseUrl}/profile_images/{uniqueFileName}";

            return imageUrl;
        }

        private async Task<UserResponseDto?> Authenticate(UserLoginDto userLogin)
        {
            return mapper.Map<UserResponseDto>((await repository.GetAllAsync()).FirstOrDefault(
                user => userLogin.GetType().GetProperties().All(prop => prop.GetValue(userLogin)?.ToString() ==
                user.GetType().GetProperty(prop.Name)?.GetValue(user)?.ToString())));
        }

        private string Generate(UserResponseDto user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(ClaimTypes.Name,user.FullName),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.UserData, user.ProfileImagePath ?? "")
            };
            var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
