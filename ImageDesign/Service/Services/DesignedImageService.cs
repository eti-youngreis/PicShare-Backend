using Common.Dto;
using Repository.Entity;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Service.Services
{
    public class DesignedImageService : IService<DesignedImageDto>
    {
        private readonly IRepository<DesignedImage> repository;
        private readonly IMapper mapper;

        public DesignedImageService(IRepository<DesignedImage> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<DesignedImageDto> AddAsync(DesignedImageDto entity)
        {
            await repository.AddAsync(mapper.Map<DesignedImage>(entity));
            return entity;
        }

        public async Task<DesignedImageDto?> DeleteAsync(int id)
        {
            return mapper.Map<DesignedImageDto>(await repository.DeleteByIdAsync(id));
        }

        public async Task<List<DesignedImageDto>> GetAllAsync()
        {
            return mapper.Map<List<DesignedImageDto>>(await repository.GetAllAsync());
        }

        public async Task<DesignedImageDto?> GetByIdAsync(int id)
        {
            return mapper.Map<DesignedImageDto>(await repository.GetByIdAsync(id));
        }

        public async Task<DesignedImageDto> UpdateAsync(int id, DesignedImageDto entity)
        {
            await repository.UpdateAsync(id, mapper.Map<DesignedImage>(entity));
            return entity;
        }
    }
}
