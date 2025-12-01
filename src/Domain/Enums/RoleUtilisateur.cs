namespace Domain.Enums;

/// <summary>
/// Énumération définissant les différents rôles utilisateur dans le système IoT.
/// </summary>
public enum RoleUtilisateur
{
    /// <summary>
    /// Utilisateur standard avec accès limité aux fonctionnalités de base.
    /// Peut consulter les données des capteurs et gérer ses propres paramètres.
    /// </summary>
    Utilisateur = 0,

    /// <summary>
    /// Administrateur avec accès complet au système.
    /// Peut gérer les utilisateurs, configurer les capteurs et accéder à toutes les fonctionnalités.
    /// </summary>
    Administrateur = 1
}
