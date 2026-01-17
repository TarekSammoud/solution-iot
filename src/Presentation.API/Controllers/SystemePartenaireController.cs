using Application.DTOs.SystemePartenaire;
using Application.Services.Interfaces;
using IotPlatform.Application.DTOs.External;
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

    /// <summary>
    /// Récupère les sondes disponibles depuis un système partenaire.
    /// </summary>
    /// <param name="id">L'identifiant du système partenaire.</param>
    /// <returns>La liste des sondes disponibles chez le partenaire.</returns>
    /// <response code="200">Retourne la liste des sondes du partenaire.</response>
    /// <response code="400">Le système partenaire n'a pas de credentials configurés ou l'URL est invalide.</response>
    /// <response code="401">Authentification échouée avec le système partenaire.</response>
    /// <response code="404">Le système partenaire n'existe pas.</response>
    /// <response code="500">Erreur de communication avec le système partenaire.</response>
    [HttpGet("{id}/sondes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExternalSondeDto>>> GetSondesFromPartenaire(Guid id)
    {
        try
        {
            var sondes = await _service.GetSondesFromPartenaire(id);
            return Ok(sondes);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex) when (ex.Message == "Authentification échouée")
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Importe les sondes depuis un système partenaire vers une localisation cible.
    /// </summary>
    /// <param name="id">L'identifiant du système partenaire.</param>
    /// <param name="localisationCibleId">L'identifiant de la localisation où importer les sondes.</param>
    /// <returns>Le résultat de l'import avec le nombre de sondes importées et les erreurs éventuelles.</returns>
    /// <response code="200">L'import a été effectué avec succès.</response>
    /// <response code="400">La localisation cible n'existe pas ou le système partenaire n'a pas de credentials configurés.</response>
    /// <response code="401">Authentification échouée avec le système partenaire.</response>
    /// <response code="404">Le système partenaire n'existe pas.</response>
    /// <response code="500">Erreur de communication avec le système partenaire.</response>
    [HttpPost("{id}/import-sondes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportSondeResultDto>> ImportSondesFromPartenaire(Guid id, [FromQuery] Guid localisationCibleId, [FromBody] List<Guid>? sondeIds = null)
    {
        try
        {
            var resultat = await _service.ImportSondesFromPartenaire(id, localisationCibleId, sondeIds);
            return Ok(resultat);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex) when (ex.Message == "Authentification échouée")
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
