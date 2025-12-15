using Application.DTOs.Releve;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

/// <summary>
/// Contrôleur API REST pour la gestion des Releves.
/// Expose les endpoints CRUD HTTP pour les opérations sur les Releves.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RelevesController : ControllerBase
{
    private readonly IReleveService _service;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur Releves.
    /// </summary>
    /// <param name="service">Le service de gestion des Releves.</param>
    public RelevesController(IReleveService service)
    {
        _service = service;
    }

    /// <summary>
    /// Récupère toutes les Releves.
    /// </summary>
    /// <returns>Liste de toutes les Releves.</returns>
    /// <response code="200">Retourne la liste des Releves.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReleveDto>>> GetAll([FromQuery(Name = "limit")] int limit = int.MaxValue, [FromQuery(Name = "page")] int page = 0  )
    {
        var Releves = await _service.GetAllAsync() ;
        return Ok(Releves.Skip(page*limit).Take(limit));
    }

    /// <summary>
    /// Récupère une Releve par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la Releve.</param>
    /// <returns>La Releve correspondante.</returns>
    /// <response code="200">Retourne la Releve trouvée.</response>
    /// <response code="404">La Releve n'existe pas.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReleveDto>> GetById(Guid id)
    {
        var Releve = await _service.GetByIdAsync(id);

        if (Releve == null)
        {
            return NotFound();
        }

        return Ok(Releve);
    }

    /// <summary>
    /// Crée une nouvelle Releve.
    /// </summary>
    /// <param name="dto">Les données de la Releve à créer.</param>
    /// <returns>La Releve créée avec Id et DateCreation générés.</returns>
    /// <response code="201">La Releve a été créée avec succès.</response>
    /// <response code="400">Les données fournies sont invalides.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReleveDto>> Create([FromBody] CreateReleveDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

   

    /// <summary>
    /// Supprime une Releve.
    /// </summary>
    /// <param name="id">L'identifiant de la Releve à supprimer.</param>
    /// <returns>Aucun contenu si la suppression réussit.</returns>
    /// <response code="204">La Releve a été supprimée avec succès.</response>
    /// <response code="404">La Releve n'existe pas.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }


    /// <summary>
    /// Récupère toutes les Releves pour une sonde.
    /// </summary>
    /// <returns>Liste de toutes les Releves.</returns>
    /// <response code="200">Retourne la liste des Releves.</response>
    [HttpGet("/sonde/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReleveDto>>> GetBySondeAsync(Guid id)
    {
        var Releves = await _service.GetBySondeAync(id);
        return Ok(Releves);
    }
    /// <summary>
    /// Récupère les N Releves pour une sonde .
    /// </summary>
    /// <returns>Liste de toutes les Releves.</returns>
    /// <response code="200">Retourne la liste des Releves.</response>
    [HttpGet("/sonde/{id}/recent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReleveDto>>> GetRecentBySondeAsync(Guid id,
        [FromQuery(Name = "n")] int n =10)
    {
        var Releves = await _service.GetRecentBySondeAync(id,n);
        return Ok(Releves);
    }

}
