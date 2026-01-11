using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtatActionneurController : ControllerBase
    {
        private readonly IEtatActionneurService _service;

        public EtatActionneurController(IEtatActionneurService service)
        {
            _service = service;
        }

        [HttpGet("actionneur/{actionneurId}")]
        public async Task<IActionResult> GetEtat(Guid actionneurId)
        {
            var result = await _service.GetEtatByActionneurIdAsync(actionneurId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("actionneur/{actionneurId}")]
        public async Task<IActionResult> UpdateEtat(Guid actionneurId, [FromBody] UpdateEtatActionneurDto dto)
        {
            try
            {
                var result = await _service.UpdateEtatAsync(actionneurId, dto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
