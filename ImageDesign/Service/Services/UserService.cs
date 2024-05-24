using AutoMapper;
using Common.Dto;
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
            await repository.UpdateAsync(id, mapper.Map<User>(entity));
            return entity;
        }
    }
}
