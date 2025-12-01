using Application.DTOs;

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
}
