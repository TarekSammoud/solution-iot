using Application.DTOs.SystemePartenaire;
using IotPlatform.Application.DTOs.External;

namespace Application.Services.Interfaces;

/// <summary>
/// Interface pour le service de gestion des SystemePartenaires.
/// Orchestre la logique métier CRUD en utilisant le repository et le mapper.
/// Toutes les méthodes retournent des DTOs, jamais des entités Domain.
/// </summary>
public interface ISystemePartenaireService
{
    /// <summary>
    /// Récupère une SystemePartenaire par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la SystemePartenaire.</param>
    /// <returns>Le DTO de la SystemePartenaire si trouvée, sinon null.</returns>
    Task<SystemePartenaireDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère toutes les SystemePartenaires.
    /// </summary>
    /// <returns>Une collection de DTOs de toutes les SystemePartenaires.</returns>
    Task<IEnumerable<SystemePartenaireDto>> GetAllAsync();

    /// <summary>
    /// Crée une nouvelle SystemePartenaire.
    /// </summary>
    /// <param name="dto">Le DTO contenant les données de création.</param>
    /// <returns>Le DTO de la SystemePartenaire créée avec Id et DateCreation générés.</returns>
    Task<SystemePartenaireDto> CreateAsync(CreateSystemePartenaireDto dto);

    /// <summary>
    /// Met à jour une SystemePartenaire existante.
    /// </summary>
    /// <param name="dto">Le DTO contenant les données de mise à jour.</param>
    /// <returns>Le DTO de la SystemePartenaire mise à jour si trouvée, sinon null.</returns>
    Task<SystemePartenaireDto?> UpdateAsync(UpdateSystemePartenaireDto dto);

    /// <summary>
    /// Supprime une SystemePartenaire.
    /// </summary>
    /// <param name="id">L'identifiant de la SystemePartenaire à supprimer.</param>
    /// <returns>True si la suppression a réussi, false si la SystemePartenaire n'existe pas.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si une SystemePartenaire existe.
    /// </summary>
    /// <param name="id">L'identifiant de la SystemePartenaire.</param>
    /// <returns>True si la SystemePartenaire existe, sinon false.</returns>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Récupère les sondes disponibles depuis un système partenaire.
    /// </summary>
    /// <param name="systemePartenaireId">L'identifiant du système partenaire.</param>
    /// <returns>La liste des sondes disponibles chez le partenaire.</returns>
    Task<List<ExternalSondeDto>> GetSondesFromPartenaire(Guid systemePartenaireId);

    /// <summary>
    /// Importe les sondes depuis un système partenaire vers une localisation cible.
    /// </summary>
    /// <param name="systemePartenaireId">L'identifiant du système partenaire.</param>
    /// <param name="localisationCibleId">L'identifiant de la localisation où importer les sondes.</param>
    /// <param name="sondeIds">Liste des IDs de sondes à importer (si null ou vide, toutes les sondes disponibles seront importées).</param>
    /// <returns>Le résultat de l'import avec le nombre de sondes importées et les erreurs éventuelles.</returns>
    Task<ImportSondeResultDto> ImportSondesFromPartenaire(Guid systemePartenaireId, Guid localisationCibleId, List<Guid>? sondeIds = null);
}
