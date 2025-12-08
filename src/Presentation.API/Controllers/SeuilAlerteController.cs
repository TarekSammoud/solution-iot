using System;
using System.Threading.Tasks;
using Application.DTOs.SeuilAlerte;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IotPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeuilAlerteController : ControllerBase
    {
        private readonly ISeuilAlerteService _service;

        public SeuilAlerteController(ISeuilAlerteService service)
        {
            _service = service;
        }

        [HttpGet("sonde/{sondeId}")]
        public async Task<IActionResult> GetBySonde(Guid sondeId)
        {
            try
            {
                var list = await _service.GetBySondeAsync(sondeId);
                return Ok(list);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSeuilAlerteDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSeuilAlerteDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            try
            {
                await _service.ToggleAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
