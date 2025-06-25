using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Threading.Tasks;

namespace PicShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService service;

        public PhotoController(IPhotoService service)
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
        public async Task<IActionResult> Post([FromForm] PhotoUploadDto photoUpload)
        {
            if (photoUpload == null || photoUpload.Photo == null || photoUpload.Photo.Length == 0)
            {
                return BadRequest("Invalid file");
            }
            if (!(photoUpload.Photo.ContentType.StartsWith("image/") || photoUpload.Photo.ContentType.StartsWith("png/")))
            {
                return BadRequest("Uploaded file is not an image");
            }
            var image = await service.AddAsync(photoUpload);
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
