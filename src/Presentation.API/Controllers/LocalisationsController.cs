using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

/// <summary>
/// Contrôleur API REST pour la gestion des localisations.
/// Expose les endpoints CRUD HTTP pour les opérations sur les localisations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LocalisationsController : ControllerBase
{
    private readonly ILocalisationService _service;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur Localisations.
    /// </summary>
    /// <param name="service">Le service de gestion des localisations.</param>
    public LocalisationsController(ILocalisationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Récupère toutes les localisations.
    /// </summary>
    /// <returns>Liste de toutes les localisations.</returns>
    /// <response code="200">Retourne la liste des localisations.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LocalisationDto>>> GetAll()
    {
        var localisations = await _service.GetAllAsync();
        return Ok(localisations);
    }

    /// <summary>
    /// Récupère une localisation par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la localisation.</param>
    /// <returns>La localisation correspondante.</returns>
    /// <response code="200">Retourne la localisation trouvée.</response>
    /// <response code="404">La localisation n'existe pas.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocalisationDto>> GetById(Guid id)
    {
        var localisation = await _service.GetByIdAsync(id);

        if (localisation == null)
        {
            return NotFound();
        }

        return Ok(localisation);
    }

    /// <summary>
    /// Crée une nouvelle localisation.
    /// </summary>
    /// <param name="dto">Les données de la localisation à créer.</param>
    /// <returns>La localisation créée avec Id et DateCreation générés.</returns>
    /// <response code="201">La localisation a été créée avec succès.</response>
    /// <response code="400">Les données fournies sont invalides.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LocalisationDto>> Create([FromBody] CreateLocalisationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Met à jour une localisation existante.
    /// </summary>
    /// <param name="id">L'identifiant de la localisation à mettre à jour.</param>
    /// <param name="dto">Les nouvelles données de la localisation.</param>
    /// <returns>La localisation mise à jour.</returns>
    /// <response code="200">La localisation a été mise à jour avec succès.</response>
    /// <response code="400">Les données fournies sont invalides ou l'ID ne correspond pas.</response>
    /// <response code="404">La localisation n'existe pas.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocalisationDto>> Update(Guid id, [FromBody] UpdateLocalisationDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans le body");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _service.UpdateAsync(dto);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /// <summary>
    /// Supprime une localisation.
    /// </summary>
    /// <param name="id">L'identifiant de la localisation à supprimer.</param>
    /// <returns>Aucun contenu si la suppression réussit.</returns>
    /// <response code="204">La localisation a été supprimée avec succès.</response>
    /// <response code="404">La localisation n'existe pas.</response>
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
}
