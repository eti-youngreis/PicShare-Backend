using AutoMapper;
using Common.Dto;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Repository.Entity;
using Repository.Interfaces;
using Repository.Repository;
using Service.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ImageService : IService<ImageDto>
    {
        private readonly IRepository<Image> repository;
        private readonly IMapper mapper;

        public ImageService(IRepository<Image> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<ImageDto?> AddAsync(ImageDto entity)
        {
            if (entity.Image == null || entity.Image.Length == 0)
            {
                return null;
            }
            var imagePath = await UploadImageAsync(entity.Image);
            entity.ImagePath = imagePath;
            var imageEntity = mapper.Map<Image>(entity);
            await repository.AddAsync(imageEntity);
            return mapper.Map<ImageDto>(imageEntity);
        }

        public async Task<ImageDto?> DeleteAsync(int id)
        {
            var image = await repository.GetByIdAsync(id);
            if (image == null)
            {
                return null;
            }
            var imagePath = image.ImagePath;
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            return mapper.Map<ImageDto>(await repository.DeleteByIdAsync(id));
        }

        public async Task<List<ImageDto>> GetAllAsync()
        {
            var images = await repository.GetAllAsync();
            return mapper.Map<List<ImageDto>>(images);
        }

        public async Task<ImageDto?> GetByIdAsync(int id)
        {
            var image = await repository.GetByIdAsync(id);
            return mapper.Map<ImageDto>(image);
        }

        public async Task<ImageDto?> UpdateAsync(int id, ImageDto entity)
        {
            var imageEntity = await repository.GetByIdAsync(id);
            if (imageEntity == null)
            {
                return null;
            }
            entity.Id = id;
            var updatedImageEntity = mapper.Map(entity, imageEntity);
            await repository.UpdateAsync(id, updatedImageEntity);
            return entity;
        }

        private static async Task<string> UploadImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return filePath;
        }
    }
}
