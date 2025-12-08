using Application.DTOs.Localisation;

namespace Application.Services.Interfaces;

/// <summary>
/// Interface pour le service de gestion des localisations.
/// Orchestre la logique métier CRUD en utilisant le repository et le mapper.
/// Toutes les méthodes retournent des DTOs, jamais des entités Domain.
/// </summary>
public interface ILocalisationService
{
    /// <summary>
    /// Récupère une localisation par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la localisation.</param>
    /// <returns>Le DTO de la localisation si trouvée, sinon null.</returns>
    Task<LocalisationDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère toutes les localisations.
    /// </summary>
    /// <returns>Une collection de DTOs de toutes les localisations.</returns>
    Task<IEnumerable<LocalisationDto>> GetAllAsync();

    /// <summary>
    /// Crée une nouvelle localisation.
    /// </summary>
    /// <param name="dto">Le DTO contenant les données de création.</param>
    /// <returns>Le DTO de la localisation créée avec Id et DateCreation générés.</returns>
    Task<LocalisationDto> CreateAsync(CreateLocalisationDto dto);

    /// <summary>
    /// Met à jour une localisation existante.
    /// </summary>
    /// <param name="dto">Le DTO contenant les données de mise à jour.</param>
    /// <returns>Le DTO de la localisation mise à jour si trouvée, sinon null.</returns>
    Task<LocalisationDto?> UpdateAsync(UpdateLocalisationDto dto);

    /// <summary>
    /// Supprime une localisation.
    /// </summary>
    /// <param name="id">L'identifiant de la localisation à supprimer.</param>
    /// <returns>True si la suppression a réussi, false si la localisation n'existe pas.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si une localisation existe.
    /// </summary>
    /// <param name="id">L'identifiant de la localisation.</param>
    /// <returns>True si la localisation existe, sinon false.</returns>
    Task<bool> ExistsAsync(Guid id);
}
