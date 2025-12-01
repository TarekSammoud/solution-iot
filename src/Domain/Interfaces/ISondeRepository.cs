using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

/// <summary>
/// Interface du repository pour l'entité Sonde.
/// Fournit les opérations CRUD et des méthodes de filtrage par localisation, type et statut actif.
/// Toutes les méthodes de récupération incluent le eager loading de Localisation et UniteMesure.
/// </summary>
public interface ISondeRepository
{
    /// <summary>
    /// Récupère une sonde par son identifiant.
    /// Inclut le eager loading de Localisation et UniteMesure.
    /// </summary>
    /// <param name="id">Identifiant de la sonde.</param>
    /// <returns>La sonde trouvée ou null si inexistante.</returns>
    Task<Sonde?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère toutes les sondes.
    /// Inclut le eager loading de Localisation et UniteMesure.
    /// Triées par Nom.
    /// </summary>
    /// <returns>Liste de toutes les sondes.</returns>
    Task<IEnumerable<Sonde>> GetAllAsync();

    /// <summary>
    /// Récupère toutes les sondes d'une localisation spécifique.
    /// Inclut le eager loading de Localisation et UniteMesure.
    /// Triées par Nom.
    /// </summary>
    /// <param name="localisationId">Identifiant de la localisation.</param>
    /// <returns>Liste des sondes de cette localisation.</returns>
    Task<IEnumerable<Sonde>> GetByLocalisationAsync(Guid localisationId);

    /// <summary>
    /// Récupère toutes les sondes d'un type spécifique.
    /// Inclut le eager loading de Localisation et UniteMesure.
    /// Triées par Nom.
    /// </summary>
    /// <param name="type">Type de sonde à filtrer.</param>
    /// <returns>Liste des sondes de ce type.</returns>
    Task<IEnumerable<Sonde>> GetByTypeAsync(TypeSonde type);

    /// <summary>
    /// Récupère toutes les sondes actives (EstActif = true).
    /// Inclut le eager loading de Localisation et UniteMesure.
    /// Triées par Nom.
    /// </summary>
    /// <returns>Liste des sondes actives.</returns>
    Task<IEnumerable<Sonde>> GetActivesAsync();

    /// <summary>
    /// Ajoute une nouvelle sonde en base de données.
    /// L'Id, DateCreation et DateInstallation sont générés automatiquement si non fournis.
    /// </summary>
    /// <param name="sonde">La sonde à ajouter.</param>
    /// <returns>La sonde créée avec Id, DateCreation et DateInstallation.</returns>
    Task<Sonde> AddAsync(Sonde sonde);

    /// <summary>
    /// Met à jour une sonde existante.
    /// </summary>
    /// <param name="sonde">La sonde à mettre à jour.</param>
    Task UpdateAsync(Sonde sonde);

    /// <summary>
    /// Supprime une sonde par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de la sonde à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si une sonde existe.
    /// </summary>
    /// <param name="id">Identifiant de la sonde.</param>
    /// <returns>True si la sonde existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
