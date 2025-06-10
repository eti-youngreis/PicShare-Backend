using Common.Dto;

namespace Service.Interfaces
{
    public interface IUserService : IService<UserResponseDto>
    {
        Task<UserResponseDto?> AddAsync(UserSignupDto entity);
        Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto entity);
        Task<UserResponseDto?> DeleteAsync(int id);
        Task<string?> LoginAsync(UserLoginDto entity);
    }
}