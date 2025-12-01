using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interface définissant les opérations de persistance pour l'entité SystemePartenaire.
/// </summary>
public interface ISystemePartenaireRepository
{
    /// <summary>
    /// Récupère un système partenaire par son identifiant unique.
    /// </summary>
    /// <param name="id">Identifiant unique du système partenaire.</param>
    /// <returns>Le système partenaire trouvé ou null si aucun système ne correspond.</returns>
    Task<SystemePartenaire?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère tous les systèmes partenaires.
    /// </summary>
    /// <returns>Collection de tous les systèmes partenaires.</returns>
    Task<IEnumerable<SystemePartenaire>> GetAllAsync();

    /// <summary>
    /// Récupère tous les systèmes partenaires qui peuvent nous appeler (EstAppelant = true et EstActif = true).
    /// Ces systèmes ont des credentials d'accès (UsernameAcces, PasswordHashAcces).
    /// </summary>
    /// <returns>Collection des systèmes partenaires appelants actifs.</returns>
    Task<IEnumerable<SystemePartenaire>> GetAppelantsAsync();

    /// <summary>
    /// Récupère tous les systèmes partenaires qu'on peut appeler (EstAppele = true et EstActif = true).
    /// Ces systèmes ont des credentials d'appel (UsernameAppel, PasswordChiffre).
    /// </summary>
    /// <returns>Collection des systèmes partenaires appelés actifs.</returns>
    Task<IEnumerable<SystemePartenaire>> GetAppelesAsync();

    /// <summary>
    /// Récupère tous les systèmes partenaires actifs (EstActif = true).
    /// </summary>
    /// <returns>Collection des systèmes partenaires actifs.</returns>
    Task<IEnumerable<SystemePartenaire>> GetActifsAsync();

    /// <summary>
    /// Récupère un système partenaire par son nom d'utilisateur d'accès (UsernameAcces).
    /// Utilisé pour authentifier un système partenaire qui nous appelle.
    /// </summary>
    /// <param name="username">Nom d'utilisateur d'accès à rechercher.</param>
    /// <returns>Le système partenaire trouvé ou null si aucun système ne correspond.</returns>
    Task<SystemePartenaire?> GetByUsernameAccesAsync(string username);

    /// <summary>
    /// Ajoute un nouveau système partenaire.
    /// </summary>
    /// <param name="systemePartenaire">Système partenaire à ajouter.</param>
    /// <returns>Le système partenaire ajouté avec son identifiant généré.</returns>
    Task<SystemePartenaire> AddAsync(SystemePartenaire systemePartenaire);

    /// <summary>
    /// Met à jour les informations d'un système partenaire existant.
    /// </summary>
    /// <param name="systemePartenaire">Système partenaire avec les informations mises à jour.</param>
    Task UpdateAsync(SystemePartenaire systemePartenaire);

    /// <summary>
    /// Supprime un système partenaire par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du système partenaire à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si un système partenaire existe avec l'identifiant spécifié.
    /// </summary>
    /// <param name="id">Identifiant du système partenaire à vérifier.</param>
    /// <returns>True si le système partenaire existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
