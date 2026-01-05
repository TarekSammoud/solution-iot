using Application.DTOs.Releve;
using Application.DTOs.Sonde;

namespace Application.Services.Interfaces;

/// <summary>
/// Interface pour le service de gestion des Releves.
/// Orchestre la logique métier CRUD en utilisant le repository et le mapper.
/// Toutes les méthodes retournent des DTOs, jamais des entités Domain.
/// </summary>
public interface IReleveService
{
    /// <summary>
    /// Récupère une Releve par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la Releve.</param>
    /// <returns>Le DTO de la Releve si trouvée, sinon null.</returns>
    Task<ReleveDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère toutes les Releves.
    /// </summary>
    /// <returns>Une collection de DTOs de toutes les Releves.</returns>
    Task<IEnumerable<ReleveDto>> GetAllAsync();

    /// <summary>
    /// Crée une nouvelle Releve.
    /// </summary>
    /// <param name="dto">Le DTO contenant les données de création.</param>
    /// <returns>Le DTO de la Releve créée avec Id et DateCreation générés.</returns>
    Task<ReleveDto> CreateAsync(CreateReleveDto dto);

    /// <summary>
    /// Met à jour une Releve existante.
    /// </summary>
    /// <param name="dto">Le DTO contenant les données de mise à jour.</param>
    /// <returns>Le DTO de la Releve mise à jour si trouvée, sinon null.</returns>
    Task<ReleveDto?> UpdateAsync(UpdateReleveDto dto);

    /// <summary>
    /// Supprime une Releve.
    /// </summary>
    /// <param name="id">L'identifiant de la Releve à supprimer.</param>
    /// <returns>True si la suppression a réussi, false si la Releve n'existe pas.</returns>
    Task<bool> DeleteAsync(Guid id);


    /// <summary>
    /// Releves d'une Sonde Specifique.
    /// </summary>
    /// <param name="id">L'identifiant de la Sonde.</param>
    /// <returns>Collection des Releve si la Releve existe, sinon [].</returns>
    Task<IEnumerable<ReleveDto>> GetBySondeAync(Guid id);

    /// <summary>
    /// N Releves recentes d'une Sonde Specifique.
    /// </summary>
    /// <param name="id">L'identifiant de la Sonde.</param>
    /// <returns>Releve si la Releve existe, sinon null.</returns>
    Task<IEnumerable<ReleveDto>> GetRecentBySondeAync(Guid id,int n=10);

    Task VerifierEtGererAlertes(Guid releveId);
}
