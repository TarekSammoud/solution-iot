using Application.DTOs.Sonde;
using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IotPlatform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SondeController : ControllerBase
    {
        private readonly ISondeService _sondeService;
        private readonly IDeviceCommunicationService _deviceCommunicationService;

        public SondeController(ISondeService sondeService, IDeviceCommunicationService deviceCommunicationService)
        {
            _sondeService = sondeService;
            _deviceCommunicationService = deviceCommunicationService;
        }

        // GET: api/sonde
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SondeDto>>> GetAll()
        {
            var sondes = await _sondeService.GetAllAsync();
            return Ok(sondes);
        }

        // GET: api/sonde/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SondeDto>> GetById(Guid id)
        {
            var sonde = await _sondeService.GetByIdAsync(id);
            if (sonde == null) return NotFound();
            return Ok(sonde);
        }

        // GET: api/sonde/localisation/{localisationId}
        [HttpGet("localisation/{localisationId:guid}")]
        public async Task<ActionResult<IEnumerable<SondeDto>>> GetByLocalisation(Guid localisationId)
        {
            var sondes = await _sondeService.GetByLocalisationAsync(localisationId);
            return Ok(sondes);
        }

        // GET: api/sonde/type/{typeSonde}
        [HttpGet("type/{typeSonde}")]
        public async Task<ActionResult<IEnumerable<SondeDto>>> GetByType(TypeSonde typeSonde)
        {
            var sondes = await _sondeService.GetByTypeAsync(typeSonde);
            return Ok(sondes);
        }

        // POST: api/sonde
        [HttpPost]
        public async Task<ActionResult<SondeDto>> Create([FromBody] CreateSondeDto dto)
        {
            try
            {
                var sonde = await _sondeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = sonde.Id }, sonde);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/sonde/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SondeDto>> Update(Guid id, [FromBody] UpdateSondeDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            try
            {
                var sonde = await _sondeService.UpdateAsync(dto);
                if (sonde == null) return NotFound();
                return Ok(sonde);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/sonde/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _sondeService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:guid}/test-communication")]
        public async Task<IActionResult> TestCommunication(Guid id)
        {
            var sonde = await _sondeService.GetByIdAsync(id);
            if (sonde == null) return NotFound();

            var result = await _deviceCommunicationService.TestConnection(sonde);
            return Ok(result);
        }

        [HttpPost("{id:guid}/force-pull")]
        public async Task<IActionResult> ForcePull(Guid id)
        {
            var sonde = await _sondeService.GetByIdAsync(id);
            if (sonde == null) return NotFound();

            var result = await _deviceCommunicationService.PullDataFromDevice(sonde);
            if (result == null) return BadRequest("Échec de la récupération ou device inactif/non-HttpPull");

            return Ok(result);
        }
    }
}
