using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interface du repository pour l'entité EtatActionneur.
/// Gère l'état actuel des actionneurs (ON/OFF et pourcentage).
/// Relation 1-to-1 : un actionneur a au maximum un état actuel.
/// </summary>
public interface IEtatActionneurRepository
{
    /// <summary>
    /// Récupère un état d'actionneur par son identifiant.
    /// Inclut l'eager loading de la navigation property Actionneur.
    /// </summary>
    /// <param name="id">Identifiant de l'état</param>
    /// <returns>L'état trouvé ou null si non trouvé</returns>
    Task<EtatActionneur?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère l'état actuel d'un actionneur spécifique.
    /// Relation 1-to-1 : retourne au maximum un seul état par actionneur.
    /// Inclut l'eager loading de la navigation property Actionneur.
    /// </summary>
    /// <param name="actionneurId">Identifiant de l'actionneur</param>
    /// <returns>L'état de l'actionneur ou null si aucun état n'existe</returns>
    Task<EtatActionneur?> GetByActionneurAsync(Guid actionneurId);

    /// <summary>
    /// Récupère tous les états d'actionneurs.
    /// Triés par DerniereModification descendant (plus récents en premier).
    /// Inclut l'eager loading de la navigation property Actionneur.
    /// </summary>
    /// <returns>Liste de tous les états</returns>
    Task<IEnumerable<EtatActionneur>> GetAllAsync();

    /// <summary>
    /// Récupère tous les états actifs (EstActif = true).
    /// Triés par DerniereModification descendant (plus récents en premier).
    /// Inclut l'eager loading de la navigation property Actionneur.
    /// Utile pour afficher tous les actionneurs actuellement allumés/en marche.
    /// </summary>
    /// <returns>Liste des états actifs uniquement</returns>
    Task<IEnumerable<EtatActionneur>> GetActifsAsync();

    /// <summary>
    /// Ajoute un nouvel état d'actionneur.
    /// Génère automatiquement l'Id (Guid) si non fourni.
    /// Définit automatiquement DerniereModification à DateTime.UtcNow si non définie.
    /// IMPORTANT : Vérifie qu'un état n'existe pas déjà pour cet actionneur (contrainte unique).
    /// </summary>
    /// <param name="etatActionneur">L'état à ajouter</param>
    /// <returns>L'état ajouté avec Id généré</returns>
    Task<EtatActionneur> AddAsync(EtatActionneur etatActionneur);

    /// <summary>
    /// Met à jour un état d'actionneur existant.
    /// Met automatiquement à jour DerniereModification à DateTime.UtcNow.
    /// Utilisé pour changer l'état ON/OFF ou le pourcentage.
    /// </summary>
    /// <param name="etatActionneur">L'état à mettre à jour</param>
    Task UpdateAsync(EtatActionneur etatActionneur);

    /// <summary>
    /// Supprime un état d'actionneur par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'état à supprimer</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si un état existe avec l'identifiant spécifié.
    /// </summary>
    /// <param name="id">Identifiant de l'état</param>
    /// <returns>true si l'état existe, false sinon</returns>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Vérifie si un état existe pour un actionneur spécifique.
    /// Utile pour vérifier la contrainte 1-to-1 avant création.
    /// </summary>
    /// <param name="actionneurId">Identifiant de l'actionneur</param>
    /// <returns>true si un état existe pour cet actionneur, false sinon</returns>
    Task<bool> ExistsByActionneurAsync(Guid actionneurId);
}
