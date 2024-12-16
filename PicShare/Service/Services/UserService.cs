using AutoMapper;
using Common.Dto;
using Microsoft.AspNetCore.Http;
using Repository.Entity;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class UserService : IService<UserDto>
    {
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;

        public UserService(IRepository<User> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<UserDto?> AddAsync(UserDto entity)
        {
            if (!(entity.ProfileImage == null || entity.ProfileImage.Length == 0 ||
                !(entity.ProfileImage.ContentType.StartsWith("image/") || entity.ProfileImage.ContentType.StartsWith("png/"))))
                entity.ProfileImagePath = await UpdateProfileImageAsync(entity.ProfileImage!);
            return mapper.Map<UserDto>(await repository.AddAsync(mapper.Map<User>(entity)));
        }

        public async Task<UserDto?> DeleteAsync(int id)
        {
            return mapper.Map<UserDto>(await repository.DeleteByIdAsync(id));
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            return mapper.Map<List<UserDto>>(await repository.GetAllAsync());
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            return mapper.Map<UserDto>(await repository.GetByIdAsync(id));
        }

        public async Task<UserDto?> UpdateAsync(int id, UserDto entity)
        {
            if (entity.ProfileImage != null && entity.ProfileImage.Length != 0 &&
                (entity.ProfileImage.ContentType.StartsWith("image/") || entity.ProfileImage.ContentType.StartsWith("png/")))
            {
                entity.ProfileImagePath = await UpdateProfileImageAsync(entity.ProfileImage, entity.ProfileImagePath ?? "");
            }

            var user = mapper.Map<User>(entity);
            await repository.UpdateAsync(id, user);
            return entity;
        }


        private async Task<string> UpdateProfileImageAsync(IFormFile newImage, string oldImagePath="")
        {
            if (newImage == null || newImage.Length == 0 ||
                !(newImage.ContentType.StartsWith("image/") || newImage.ContentType.StartsWith("png/")))
            {
                return oldImagePath; // אין צורך להחליף תמונה כי המשתמש לא העלה תמונה חדשה
            }

            // מחק את התמונה הישנה
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                var oldImageFileName = Path.GetFileName(oldImagePath);
                var oldImageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images", oldImageFileName);
                if (File.Exists(oldImageFullPath))
                {
                    File.Delete(oldImageFullPath);
                }
            }

            // העלה את התמונה החדשה
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + newImage.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await newImage.CopyToAsync(stream);
            }

            var baseUrl = "https://localhost:44357";
            var imageUrl = $"{baseUrl}/profile_images/{uniqueFileName}";

            return imageUrl;
        }

    }
}
