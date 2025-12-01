using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interface du repository pour l'entité SeuilAlerte.
/// Définit les opérations de lecture/écriture pour les seuils d'alerte configurés sur les sondes.
/// </summary>
public interface ISeuilAlerteRepository
{
    /// <summary>
    /// Récupère un seuil d'alerte par son identifiant avec eager loading de la Sonde.
    /// </summary>
    /// <param name="id">Identifiant du seuil d'alerte.</param>
    /// <returns>Le seuil d'alerte trouvé ou null si non trouvé.</returns>
    Task<SeuilAlerte?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère tous les seuils d'alerte avec eager loading de la Sonde.
    /// Les résultats sont triés par SondeId, puis TypeSeuil, puis TypeAlerte.
    /// </summary>
    /// <returns>Collection de tous les seuils d'alerte.</returns>
    Task<IEnumerable<SeuilAlerte>> GetAllAsync();

    /// <summary>
    /// Récupère tous les seuils d'alerte d'une sonde spécifique.
    /// Les résultats sont triés par TypeSeuil puis TypeAlerte.
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <returns>Collection des seuils d'alerte de la sonde (actifs et inactifs).</returns>
    Task<IEnumerable<SeuilAlerte>> GetBySondeAsync(Guid sondeId);

    /// <summary>
    /// Récupère tous les seuils d'alerte actifs (EstActif = true).
    /// Les résultats sont triés par SondeId, puis TypeSeuil, puis TypeAlerte.
    /// </summary>
    /// <returns>Collection des seuils d'alerte actifs.</returns>
    Task<IEnumerable<SeuilAlerte>> GetActifsAsync();

    /// <summary>
    /// Récupère tous les seuils d'alerte actifs d'une sonde spécifique.
    /// Les résultats sont triés par TypeSeuil puis TypeAlerte.
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <returns>Collection des seuils d'alerte actifs de la sonde.</returns>
    Task<IEnumerable<SeuilAlerte>> GetBySondeActifsAsync(Guid sondeId);

    /// <summary>
    /// Ajoute un nouveau seuil d'alerte dans la base de données.
    /// Génère automatiquement un Id si non fourni.
    /// Définit DateCreation à DateTime.UtcNow si non définie.
    /// </summary>
    /// <param name="seuilAlerte">Le seuil d'alerte à ajouter.</param>
    /// <returns>Le seuil d'alerte ajouté avec son Id généré.</returns>
    Task<SeuilAlerte> AddAsync(SeuilAlerte seuilAlerte);

    /// <summary>
    /// Met à jour un seuil d'alerte existant.
    /// </summary>
    /// <param name="seuilAlerte">Le seuil d'alerte à mettre à jour.</param>
    Task UpdateAsync(SeuilAlerte seuilAlerte);

    /// <summary>
    /// Supprime un seuil d'alerte par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du seuil d'alerte à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si un seuil d'alerte existe par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du seuil d'alerte.</param>
    /// <returns>True si le seuil existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
