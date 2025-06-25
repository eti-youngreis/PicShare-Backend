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
        private const string IdClaimType = "id";
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;
        private readonly IConfiguration config;

        public UserService(IRepository<User> repository, IMapper mapper, IConfiguration config)
        {
            this.config = config;
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<UserResponseDto?> AddAsync(UserSignUpDto entity)
        {
            // Hash the password before storing it
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password);

            // Map the DTO to the User entity and set the hashed password
            var user = mapper.Map<User>(entity);
            user.Password = hashedPassword;

            // Save the user to the database
            return mapper.Map<UserResponseDto>(await repository.AddAsync(user));
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
        
        public async Task<string?> SignInAsync(UserSignInDto userSignIn)
        {
            var user = await Authenticate(userSignIn);
            if (user != null)
            {
                return GenerateJwtToken(user);
            }
            return null;
        }

        public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto userToUpdate)
        {
            var user = await repository.GetByIdAsync(id);
            if (user == null)
            {
                return null; // Return null if the user does not exist
            }

            // Update profile picture if provided and valid
            if (userToUpdate.ProfilePicture != null && userToUpdate.ProfilePicture.Length > 0 &&
                userToUpdate.ProfilePicture.ContentType.StartsWith("image/"))
            {
                user.ProfilePictureUrl = await UpdateProfilePictureAsync(userToUpdate.ProfilePicture, user.ProfilePictureUrl ?? "");
            }

            // Update other fields if provided
            if (!string.IsNullOrEmpty(userToUpdate.FullName))
            {
                user.FullName = userToUpdate.FullName;
            }

            if (!string.IsNullOrEmpty(userToUpdate.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userToUpdate.Password);
            }

            await repository.UpdateAsync(id, user);

            return mapper.Map<UserResponseDto>(user);
        }


        private static async Task<string> UpdateProfilePictureAsync(IFormFile newPicture, string oldPicturePath = "")
        {
            if (newPicture == null || newPicture.Length == 0 || !newPicture.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException("Invalid profile picture.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(newPicture.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Unsupported file format.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_pictures");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
            }

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!string.IsNullOrEmpty(oldPicturePath))
            {
                var oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(oldPicturePath));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath); // Delete the old profile picture
                }
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await newPicture.CopyToAsync(stream);
            }

            return $"/profile_pictures/{uniqueFileName}";
        }

        private async Task<UserResponseDto?> Authenticate(UserSignInDto userLogin)
        {
            var user = (await repository.GetAllAsync()).FirstOrDefault(u => u.Email == userLogin.Email); // Use a query to fetch the user by email
            if (user == null || !VerifyPassword(userLogin.Password, user.Password)) // Verify hashed password
            {
                return null;
            }

            return mapper.Map<UserResponseDto>(user);
        }
        private static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHashedPassword);
        }

        private string GenerateJwtToken(UserResponseDto user)
        {
            // Check if the required JWT configuration values are present
            if (string.IsNullOrEmpty(config["Jwt:Key"]) || string.IsNullOrEmpty(config["Jwt:Issuer"]) || string.IsNullOrEmpty(config["Jwt:Audience"]))
            {
                throw new InvalidOperationException("JWT configuration values are missing.");
            }

            // Define the claims to include in the JWT
            var claims = new[]
            {                new Claim(JwtRegisteredClaimNames.Sub, user.Email), // Subject claim (user's email)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique identifier for the token
                new Claim(IdClaimType, user.Id.ToString()) // Custom claim for the user's ID
            };

            // Create a symmetric security key using the configured JWT key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

            // Define the signing credentials using the security key and HMAC-SHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token with the specified issuer, audience, claims, expiration, and signing credentials
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"], // The entity that issued the token
                audience: config["Jwt:Audience"], // The intended recipient of the token
                claims: claims, // The claims included in the token
                expires: DateTime.Now.AddMinutes(int.Parse(config["Jwt:ExpiryMinutes"] ?? "30")), // Token expiration time
                signingCredentials: creds // Signing credentials to ensure token integrity
            );

            // Serialize the JWT token into a string format and return it
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserResponseDto?> GetUserFromClaimsAsync(ClaimsPrincipal userClaims)
        {
            // Ensure we have valid claims identity
            if (userClaims?.Identity is not ClaimsIdentity identity)
            {
                return null;
            }

            // Try to get the user ID from claims
            var idClaim = identity.Claims.FirstOrDefault(x => x.Type == IdClaimType);
            if (idClaim == null || string.IsNullOrEmpty(idClaim.Value))
            {
                return null;
            }

            // Parse and validate the user ID
            if (!int.TryParse(idClaim.Value, out int userId))
            {
                return null;
            }

            // Retrieve the user from the repository
            return await GetByIdAsync(userId);
        }
    }
}
