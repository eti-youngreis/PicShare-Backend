using Common.Dto;
using System.Security.Claims;

namespace Service.Interfaces
{
    public interface IUserService : IService<UserResponseDto>
    {
        Task<UserResponseDto?> AddAsync(UserSignUpDto entity);
        Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto entity);
        Task<string?> SignInAsync(UserSignInDto entity);
        Task<UserResponseDto?> GetUserFromClaimsAsync(ClaimsPrincipal userClaims);
    }
}