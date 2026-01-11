using Application.DTOs.Actionneur;
using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionneurController : ControllerBase
    {
        private readonly IActionneurService _service;

        public ActionneurController(IActionneurService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("localisation/{localisationId}")]
        public async Task<IActionResult> GetByLocalisation(Guid localisationId) => Ok(await _service.GetByLocalisationAsync(localisationId));

        [HttpGet("type/{typeActionneur}")]
        public async Task<IActionResult> GetByType(TypeActionneur typeActionneur) => Ok(await _service.GetByTypeAsync(typeActionneur));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActionneurDto dto) => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateActionneurDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _service.DeleteAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
