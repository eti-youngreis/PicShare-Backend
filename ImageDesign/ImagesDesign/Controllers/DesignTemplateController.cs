using AutoMapper;
using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImagesDesign.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignTemplateController : ControllerBase
    {
        private readonly IService<DesignTemplateDto> service;

        public DesignTemplateController(IService<DesignTemplateDto> service)
        {
            this.service = service;
        }

        // GET: api/<DesignTemplateController>
        [HttpGet]
        public async Task<List<DesignTemplateDto>> Get()
        {
            return await service.GetAllAsync();
        }

        // GET api/<DesignTemplateController>/5
        [HttpGet("{id}")]
        public async Task<DesignTemplateDto?> Get(int id)
        {
            return await service.GetByIdAsync(id);
        }

        // POST api/<DesignTemplateController>
        [HttpPost]
        public async Task<DesignTemplateDto> Post([FromBody] DesignTemplateDto entity)
        {
            return await service.AddAsync(entity);
        }

        // PUT api/<DesignTemplateController>/5
        [HttpPut("{id}")]
        public async Task<DesignTemplateDto> Put(int id, DesignTemplateDto entity)
        {
            return await service.UpdateAsync(id, entity);
        }

        // DELETE api/<DesignTemplateController>/5
        [HttpDelete("{id}")]
        public async Task<DesignTemplateDto?> Delete(int id)
        {
            return await service.DeleteAsync(id);
        }
    }
}
