using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Représente un utilisateur du système IoT avec ses informations d'authentification et d'autorisation.
/// </summary>
public class User
{
    /// <summary>
    /// Identifiant unique de l'utilisateur.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nom d'utilisateur unique utilisé pour l'authentification.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Adresse email unique de l'utilisateur.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash du mot de passe (non réversible) pour la sécurité.
    /// Ne jamais stocker le mot de passe en clair.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Rôle de l'utilisateur déterminant ses droits d'accès.
    /// </summary>
    public RoleUtilisateur Role { get; set; }

    /// <summary>
    /// Date et heure de création du compte utilisateur.
    /// </summary>
    public DateTime DateCreation { get; set; }

    /// <summary>
    /// Indique si le compte utilisateur est actif.
    /// Un compte inactif ne peut pas se connecter au système.
    /// </summary>
    public bool EstActif { get; set; }


}
