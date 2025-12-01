using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

/// <summary>
/// Interface du repository pour l'entité Actionneur.
/// Fournit les opérations CRUD et des méthodes de filtrage par localisation, type et statut actif.
/// Toutes les méthodes de récupération incluent le eager loading de Localisation.
/// </summary>
public interface IActionneurRepository
{
    /// <summary>
    /// Récupère un actionneur par son identifiant.
    /// Inclut le eager loading de Localisation.
    /// </summary>
    /// <param name="id">Identifiant de l'actionneur.</param>
    /// <returns>L'actionneur trouvé ou null si inexistant.</returns>
    Task<Actionneur?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère tous les actionneurs.
    /// Inclut le eager loading de Localisation.
    /// Triés par Nom.
    /// </summary>
    /// <returns>Liste de tous les actionneurs.</returns>
    Task<IEnumerable<Actionneur>> GetAllAsync();

    /// <summary>
    /// Récupère tous les actionneurs d'une localisation spécifique.
    /// Inclut le eager loading de Localisation.
    /// Triés par Nom.
    /// </summary>
    /// <param name="localisationId">Identifiant de la localisation.</param>
    /// <returns>Liste des actionneurs de cette localisation.</returns>
    Task<IEnumerable<Actionneur>> GetByLocalisationAsync(Guid localisationId);

    /// <summary>
    /// Récupère tous les actionneurs d'un type spécifique.
    /// Inclut le eager loading de Localisation.
    /// Triés par Nom.
    /// </summary>
    /// <param name="type">Type d'actionneur à filtrer.</param>
    /// <returns>Liste des actionneurs de ce type.</returns>
    Task<IEnumerable<Actionneur>> GetByTypeAsync(TypeActionneur type);

    /// <summary>
    /// Récupère tous les actionneurs actifs (EstActif = true).
    /// Inclut le eager loading de Localisation.
    /// Triés par Nom.
    /// </summary>
    /// <returns>Liste des actionneurs actifs.</returns>
    Task<IEnumerable<Actionneur>> GetActifsAsync();

    /// <summary>
    /// Ajoute un nouvel actionneur en base de données.
    /// L'Id, DateCreation et DateInstallation sont générés automatiquement si non fournis.
    /// </summary>
    /// <param name="actionneur">L'actionneur à ajouter.</param>
    /// <returns>L'actionneur créé avec Id, DateCreation et DateInstallation.</returns>
    Task<Actionneur> AddAsync(Actionneur actionneur);

    /// <summary>
    /// Met à jour un actionneur existant.
    /// </summary>
    /// <param name="actionneur">L'actionneur à mettre à jour.</param>
    Task UpdateAsync(Actionneur actionneur);

    /// <summary>
    /// Supprime un actionneur par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'actionneur à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si un actionneur existe.
    /// </summary>
    /// <param name="id">Identifiant de l'actionneur.</param>
    /// <returns>True si l'actionneur existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
