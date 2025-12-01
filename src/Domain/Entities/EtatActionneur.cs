namespace Domain.Entities;

/// <summary>
/// Représente l'état actuel d'un actionneur (ON/OFF et pourcentage d'intensité/vitesse).
/// IMPORTANT : Cette entité stocke UNIQUEMENT l'état actuel de l'actionneur, PAS l'historique.
/// Relation 1-to-1 avec Actionneur : un actionneur a au maximum un état actuel.
/// </summary>
/// <remarks>
/// Utilisation du Pourcentage selon le type d'actionneur :
/// - AmpouleSimple : Pourcentage n'est pas utilisé (toujours 0 si OFF, 100 si ON)
/// - AmpouleVariometre : Pourcentage = intensité lumineuse (0-100%)
/// - Moteur : Pourcentage = vitesse de rotation (0-100%)
///
/// DerniereModification est automatiquement mise à jour lors de chaque changement d'état
/// via la couche Application ou directement dans le repository.
/// </remarks>
public class EtatActionneur
{
    /// <summary>
    /// Identifiant unique de l'état.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifiant de l'actionneur associé (relation 1-to-1).
    /// Un actionneur ne peut avoir qu'un seul état actuel.
    /// </summary>
    public Guid ActionneurId { get; set; }

    /// <summary>
    /// État de l'actionneur : true = ON (actif), false = OFF (inactif).
    /// </summary>
    public bool EstActif { get; set; }

    /// <summary>
    /// Pourcentage d'intensité (pour variomètres) ou de vitesse (pour moteurs).
    /// Valeur entre 0 et 100.
    ///
    /// Utilisation :
    /// - AmpouleSimple : toujours 0 (OFF) ou 100 (ON), pas de variation
    /// - AmpouleVariometre : 0-100 représente l'intensité lumineuse
    /// - Moteur : 0-100 représente la vitesse de rotation
    /// </summary>
    public int Pourcentage { get; set; }

    /// <summary>
    /// Date et heure de la dernière modification de l'état de l'actionneur.
    /// Mise à jour automatiquement lors de chaque changement d'état.
    /// Stockée en UTC pour cohérence internationale.
    /// </summary>
    public DateTime DerniereModification { get; set; }

    /// <summary>
    /// Navigation property vers l'actionneur associé (relation 1-to-1).
    /// </summary>
    public Actionneur Actionneur { get; set; } = null!;
}
