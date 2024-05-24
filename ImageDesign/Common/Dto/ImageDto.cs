using Microsoft.AspNetCore.Http;

namespace Common.Dto
{
    public class ImageDto
    {
        public int? Id { get; set; }
        public IFormFile Image { get; set; }
        public int UserId { get; set; }
        public string? ImagePath { get; set; }
    }
}
