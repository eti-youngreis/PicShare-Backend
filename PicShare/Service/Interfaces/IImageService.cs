using Common.Dto;

namespace Service.Interfaces
{
    public interface IImageService: IService<ImageDto>
    {
        Task<ImageDto?> AddAsync(ImageDto entity);
        Task<ImageDto?> UpdateAsync(int id, ImageDto entity);
        Task<ImageDto?> DeleteAsync(int id);
    }
}
