using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImagesDesign.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignedImageController : ControllerBase
    {
        private readonly IService<DesignedImageDto> service;

        public DesignedImageController(IService<DesignedImageDto> service)
        {
            this.service = service;
        }

        // GET: api/<DesignedImageController>
        [HttpGet]
        public async Task<List<DesignedImageDto>> Get()
        {
            return await service.GetAllAsync();
        }

        // GET api/<DesignedImageController>/5
        [HttpGet("{id}")]
        public async Task<DesignedImageDto?> Get(int id)
        {
            return await service.GetByIdAsync(id);
        }

        // POST api/<DesignedImageController>
        [HttpPost]
        public async Task<DesignedImageDto> Post([FromBody] DesignedImageDto entity)
        {
            return await service.AddAsync(entity);
        }

        // PUT api/<DesignedImageController>/5
        [HttpPut("{id}")]
        public async Task<DesignedImageDto> Put(int id, DesignedImageDto entity)
        {
            return await service.UpdateAsync(id, entity);
        }

        // DELETE api/<DesignedImageController>/5
        [HttpDelete("{id}")]
        public async Task<DesignedImageDto?> Delete(int id)
        {
            return await service.DeleteAsync(id);
        }
    }
}
