using AutoMapper;
using Common.Dto;
using Microsoft.AspNetCore.Http;
using Repository.Entity;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IRepository<Photo> repository;
        private readonly IMapper mapper;

        public PhotoService(IRepository<Photo> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<PhotoResponseDto?> AddAsync(PhotoUploadDto photoUpload)
        {
            if (photoUpload.Photo == null || photoUpload.Photo.Length == 0)
            {
                return null; // Return null if no photo is provided
            }

            // Upload the photo and get the URL
            var photoUrl = await UploadPhotoAsync(photoUpload.Photo);

            // Create a new Photo entity
            var photoEntity = new Photo
            {
                UserId = photoUpload.UserId,
                Url = photoUrl
            };

            // Add the photo entity to the repository
            var addedPhoto = await repository.AddAsync(photoEntity);

            // Map the added photo entity to PhotoResponseDto and return it
            return mapper.Map<PhotoResponseDto>(addedPhoto);
        }


        public async Task<PhotoResponseDto?> DeleteAsync(int id)
        {
            // Retrieve the photo entity from the repository
            var photo = await repository.GetByIdAsync(id);
            if (photo == null)
            {
                return null; // Return null if the photo does not exist
            }

            // Extract the file path from the photo URL
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");
            var fileName = Path.GetFileName(photo.Url);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Attempt to delete the file if it exists
            if (File.Exists(filePath))
                File.Delete(filePath);

            // Delete the photo entity from the repository
            var deletedPhoto = await repository.DeleteByIdAsync(id);

            // Map the deleted entity to a DTO and return it
            return mapper.Map<PhotoResponseDto>(deletedPhoto);
        }

        public async Task<List<PhotoResponseDto>> GetAllAsync()
        {
            var photos = await repository.GetAllAsync();
            return mapper.Map<List<PhotoResponseDto>>(photos);
        }

        public async Task<PhotoResponseDto?> GetByIdAsync(int id)
        {
            var photo = await repository.GetByIdAsync(id);
            return mapper.Map<PhotoResponseDto>(photo);
        }

        private static async Task<string> UploadPhotoAsync(IFormFile photo)
        {
            // Define the folder path where photos will be stored
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");

            // Generate a unique file name to avoid overwriting existing files
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

            // Combine the folder path and file name to create the full file path
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the uploaded photo to the specified file path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            // Construct the URL for accessing the uploaded photo
            var baseUrl = "https://localhost:44357";
            var photoUrl = $"{baseUrl}/photos/{uniqueFileName}";

            return photoUrl; // Return the photo URL
        }

    }
}
