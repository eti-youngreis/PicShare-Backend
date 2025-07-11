﻿using Common.Dto;

namespace Service.Interfaces
{
    public interface IPhotoService: IService<PhotoResponseDto>
    {
        Task<PhotoResponseDto?> AddAsync(PhotoUploadDto entity);
        Task<List<PhotoResponseDto>> GetPhotosByUserIdAsync(int userId);
    }
}
