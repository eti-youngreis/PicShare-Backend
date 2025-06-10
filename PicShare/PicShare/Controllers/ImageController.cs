using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Threading.Tasks;

namespace ImageDesign.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService service;

        public ImageController(IImageService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var images = await service.GetAllAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult?> Get(int id)
        {
            var image = await service.GetByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return Ok(image);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ImageDto data)
        {
            if (data == null || data.Image == null || data.Image.Length == 0)
            {
                return BadRequest("Invalid file");
            }
            if (!(data.Image.ContentType.StartsWith("image/") || data.Image.ContentType.StartsWith("png/")))
            {
                return BadRequest("Uploaded file is not an image");
            }
            var image = await service.AddAsync(data);
            if (image == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload image");
            }
            return Ok(image);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedImage = await service.DeleteAsync(id);
            if (deletedImage == null)
            {
                return NotFound();
            }
            return Ok(deletedImage);
        }
    }
}
