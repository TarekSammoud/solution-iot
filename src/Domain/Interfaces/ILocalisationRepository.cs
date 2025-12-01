using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interface définissant les opérations de persistance pour l'entité Localisation.
/// </summary>
public interface ILocalisationRepository
{
    /// <summary>
    /// Récupère une localisation par son identifiant unique.
    /// </summary>
    /// <param name="id">Identifiant unique de la localisation.</param>
    /// <returns>La localisation trouvée ou null si aucune localisation ne correspond.</returns>
    Task<Localisation?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère toutes les localisations du système.
    /// </summary>
    /// <returns>Collection de toutes les localisations.</returns>
    Task<IEnumerable<Localisation>> GetAllAsync();

    /// <summary>
    /// Ajoute une nouvelle localisation dans le système.
    /// </summary>
    /// <param name="localisation">Localisation à ajouter.</param>
    /// <returns>La localisation ajoutée avec son identifiant généré.</returns>
    Task<Localisation> AddAsync(Localisation localisation);

    /// <summary>
    /// Met à jour les informations d'une localisation existante.
    /// </summary>
    /// <param name="localisation">Localisation avec les informations mises à jour.</param>
    Task UpdateAsync(Localisation localisation);

    /// <summary>
    /// Supprime une localisation par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de la localisation à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si une localisation existe avec l'identifiant spécifié.
    /// </summary>
    /// <param name="id">Identifiant de la localisation à vérifier.</param>
    /// <returns>True si la localisation existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
