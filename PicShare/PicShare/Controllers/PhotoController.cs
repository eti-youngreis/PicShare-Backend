using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] PhotoUploadDto photoUpload)
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

        [HttpGet("my-photos")]
        [Authorize] // Ensures the user is authenticated
        public async Task<IActionResult> GetCurrentUserPhotos()
        {
            // Get the authenticated user from the token
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.Claims.FirstOrDefault(x => x.Type == "id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Retrieve all photos for the current user
            var photos = await service.GetPhotosByUserIdAsync(userId);
            return Ok(photos);
        }
    }
}
