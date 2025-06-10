using Microsoft.AspNetCore.Http;

namespace Common.Dto
{
    public class UserUpdateDto
    {
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
