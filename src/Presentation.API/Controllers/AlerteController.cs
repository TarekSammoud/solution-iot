using Application.DTOs.Alertes;
using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IotPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlerteController : ControllerBase
    {
        private readonly IAlerteService _alerteService;

        public AlerteController(IAlerteService alerteService)
        {
            _alerteService = alerteService;
        }

        // ---------------- DASHBOARD ----------------
        [HttpGet("dashboard")]
        public async Task<ActionResult<IEnumerable<AlerteDto>>> GetDashboard()
        {
            var alertes = await _alerteService.GetDashboardAsync();
            return Ok(alertes);
        }

        // ---------------- BY SONDE ----------------
        [HttpGet("by-sonde/{sondeId:guid}")]
        public async Task<ActionResult<IEnumerable<AlerteDto>>> GetBySonde(
            Guid sondeId,
            [FromQuery] StatutAlerte? statut = null,
            [FromQuery] TypeSeuil? typeSeuil = null)
        {
            var alertes = await _alerteService.GetBySondeAsync(sondeId, statut, typeSeuil);
            return Ok(alertes);
        }

        // ---------------- DETAILS ----------------
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AlerteDetailsDto>> GetDetails(Guid id)
        {
            var alerte = await _alerteService.GetDetailsAsync(id);
            if (alerte == null) return NotFound();
            return Ok(alerte);
        }

        // ---------------- COMMANDS ----------------
        [HttpPost("{id:guid}/acquitter")]
        public async Task<IActionResult> Acquitter(Guid id, [FromQuery] string? commentaire = null)
        {
            await _alerteService.AcquitterAsync(id, commentaire);
            return NoContent();
        }

        [HttpPost("{id:guid}/resoudre")]
        public async Task<IActionResult> Resoudre(Guid id, [FromQuery] string? commentaire = null)
        {
            await _alerteService.ResoudreAsync(id, commentaire);
            return NoContent();
        }
    }
}
