using Microsoft.AspNetCore.Http;

namespace Common.Dto
{
    public class PhotoUploadDto
    {
        public IFormFile Photo { get; set; }
        public int UserId { get; set; }
    }
}
