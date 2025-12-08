using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interface définissant les opérations de persistance pour l'entité User.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Récupère un utilisateur par son identifiant unique.
    /// </summary>
    /// <param name="id">Identifiant unique de l'utilisateur.</param>
    /// <returns>L'utilisateur trouvé ou null si aucun utilisateur ne correspond.</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère un utilisateur par son nom d'utilisateur.
    /// </summary>
    /// <param name="username">Nom d'utilisateur à rechercher.</param>
    /// <returns>L'utilisateur trouvé ou null si aucun utilisateur ne correspond.</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Récupère un utilisateur par son adresse email.
    /// </summary>
    /// <param name="email">Adresse email à rechercher.</param>
    /// <returns>L'utilisateur trouvé ou null si aucun utilisateur ne correspond.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Récupère tous les utilisateurs du système.
    /// </summary>
    /// <returns>Collection de tous les utilisateurs.</returns>
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> SearchQuery(string query);

    /// <summary>
    /// Ajoute un nouvel utilisateur dans le système.
    /// </summary>
    /// <param name="user">Utilisateur à ajouter.</param>
    /// <returns>L'utilisateur ajouté avec son identifiant généré.</returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Met à jour les informations d'un utilisateur existant.
    /// </summary>
    /// <param name="user">Utilisateur avec les informations mises à jour.</param>
    Task UpdateAsync(User user);

    /// <summary>
    /// Supprime un utilisateur par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si un utilisateur existe avec l'identifiant spécifié.
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur à vérifier.</param>
    /// <returns>True si l'utilisateur existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
