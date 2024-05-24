using AutoMapper;
using Common.Dto;
using Repository.Entity;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DesignTemplateService:IService<DesignTemplateDto>
    {
        private readonly IRepository<DesignTemplate> repository;
        private readonly IMapper mapper;
        public DesignTemplateService(IRepository<DesignTemplate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<DesignTemplateDto> AddAsync(DesignTemplateDto entity)
        {
            await repository.AddAsync(mapper.Map<DesignTemplate>(entity));
            return entity;
        }

        public async Task<DesignTemplateDto?> DeleteAsync(int id)
        {
            return mapper.Map<DesignTemplateDto>( await repository.DeleteByIdAsync(id));
        }

        public async Task<List<DesignTemplateDto>> GetAllAsync()
        {
            return mapper.Map<List<DesignTemplateDto>>(await repository.GetAllAsync());
        }

        public async Task<DesignTemplateDto?> GetByIdAsync(int id)
        {
            return mapper.Map<DesignTemplateDto>(await repository.GetByIdAsync(id));
        }

        public async Task<DesignTemplateDto> UpdateAsync(int id, DesignTemplateDto entity)
        {
            await repository.UpdateAsync(id, mapper.Map<DesignTemplate>(entity));
            return entity;
        }

    }
}
