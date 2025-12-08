using Application.DTOs.SystemePartenaire;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers;

/// <summary>
/// Contrôleur API REST pour la gestion des SystemePartenaires.
/// Expose les endpoints CRUD HTTP pour les opérations sur les SystemePartenaires.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SystemePartenairesController : ControllerBase
{
    private readonly ISystemePartenaireService _service;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur SystemePartenaires.
    /// </summary>
    /// <param name="service">Le service de gestion des SystemePartenaires.</param>
    public SystemePartenairesController(ISystemePartenaireService service)
    {
        _service = service;
    }

    /// <summary>
    /// Récupère toutes les SystemePartenaires.
    /// </summary>
    /// <returns>Liste de toutes les SystemePartenaires.</returns>
    /// <response code="200">Retourne la liste des SystemePartenaires.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SystemePartenaireDto>>> GetAll()
    {
        var SystemePartenaires = await _service.GetAllAsync();
        return Ok(SystemePartenaires);
    }

    /// <summary>
    /// Récupère une SystemePartenaire par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la SystemePartenaire.</param>
    /// <returns>La SystemePartenaire correspondante.</returns>
    /// <response code="200">Retourne la SystemePartenaire trouvée.</response>
    /// <response code="404">La SystemePartenaire n'existe pas.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SystemePartenaireDto>> GetById(Guid id)
    {
        var SystemePartenaire = await _service.GetByIdAsync(id);

        if (SystemePartenaire == null)
        {
            return NotFound();
        }

        return Ok(SystemePartenaire);
    }

    /// <summary>
    /// Crée une nouvelle SystemePartenaire.
    /// </summary>
    /// <param name="dto">Les données de la SystemePartenaire à créer.</param>
    /// <returns>La SystemePartenaire créée avec Id et DateCreation générés.</returns>
    /// <response code="201">La SystemePartenaire a été créée avec succès.</response>
    /// <response code="400">Les données fournies sont invalides.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SystemePartenaireDto>> Create([FromBody] CreateSystemePartenaireDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Met à jour une SystemePartenaire existante.
    /// </summary>
    /// <param name="id">L'identifiant de la SystemePartenaire à mettre à jour.</param>
    /// <param name="dto">Les nouvelles données de la SystemePartenaire.</param>
    /// <returns>La SystemePartenaire mise à jour.</returns>
    /// <response code="200">La SystemePartenaire a été mise à jour avec succès.</response>
    /// <response code="400">Les données fournies sont invalides ou l'ID ne correspond pas.</response>
    /// <response code="404">La SystemePartenaire n'existe pas.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SystemePartenaireDto>> Update(Guid id, [FromBody] UpdateSystemePartenaireDto dto)
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
    /// Supprime une SystemePartenaire.
    /// </summary>
    /// <param name="id">L'identifiant de la SystemePartenaire à supprimer.</param>
    /// <returns>Aucun contenu si la suppression réussit.</returns>
    /// <response code="204">La SystemePartenaire a été supprimée avec succès.</response>
    /// <response code="404">La SystemePartenaire n'existe pas.</response>
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
