using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Représente une alerte générée lorsqu'un relevé dépasse un seuil configuré.
/// Une alerte est créée automatiquement par la logique métier dans les conditions suivantes :
/// - Un relevé dépasse un seuil actif
/// - ET aucune alerte du même TypeAlerte n'est déjà Active pour ce seuil
///
/// Règles métier importantes :
/// - Un Avertissement actif n'empêche PAS la création d'une Alerte pour le même TypeSeuil.
/// - Une sonde peut avoir simultanément un Avertissement actif ET une Alerte active pour un même TypeSeuil.
/// - Les alertes suivent un cycle de vie : Active → Acquittee → Resolue.
/// - Une alerte Active peut être directement Resolue sans passer par Acquittee.
///
/// Note CRITIQUE : Il n'y a PAS de relation physique (FK) vers Releve.
/// Un relevé DÉCLENCHE une alerte via la logique métier, mais l'alerte ne stocke pas de ReleveId.
/// </summary>
public class Alerte
{
    /// <summary>
    /// Identifiant unique de l'alerte.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifiant de la sonde qui a généré l'alerte.
    /// </summary>
    public Guid SondeId { get; set; }

    /// <summary>
    /// Identifiant du seuil d'alerte qui a été dépassé.
    /// </summary>
    public Guid SeuilAlerteId { get; set; }

    /// <summary>
    /// Type de seuil dépassé (Minimum ou Maximum).
    /// Copie de SeuilAlerte.TypeSeuil pour historique (au cas où le seuil est modifié/supprimé).
    /// </summary>
    public TypeSeuil TypeSeuil { get; set; }

    /// <summary>
    /// Type d'alerte générée (Alerte ou Avertissement).
    /// Copie de SeuilAlerte.TypeAlerte pour historique (au cas où le seuil est modifié/supprimé).
    /// </summary>
    public TypeAlerte TypeAlerte { get; set; }

    /// <summary>
    /// Statut actuel de l'alerte (Active, Acquittee, Resolue).
    /// Détermine l'état dans le cycle de vie de l'alerte.
    /// </summary>
    public StatutAlerte Statut { get; set; }

    /// <summary>
    /// Date et heure de création de l'alerte.
    /// Définie automatiquement lors de la création.
    /// </summary>
    public DateTime DateCreation { get; set; }

    /// <summary>
    /// Date et heure d'acquittement de l'alerte (nullable).
    /// Définie lorsqu'un utilisateur prend en compte l'alerte.
    /// Null si l'alerte n'a pas encore été acquittée.
    /// </summary>
    public DateTime? DateAcquittement { get; set; }

    /// <summary>
    /// Date et heure de résolution de l'alerte (nullable).
    /// Définie lorsque le relevé revient dans les limites normales.
    /// Null si l'alerte n'a pas encore été résolue.
    /// </summary>
    public DateTime? DateResolution { get; set; }

    /// <summary>
    /// Message optionnel de l'alerte.
    /// Peut contenir des informations supplémentaires sur l'alerte.
    /// Exemple : "Température de 12°C détectée, en dessous du seuil de 15°C".
    /// </summary>
    public string? Message { get; set; }

    // Navigation properties

    /// <summary>
    /// Sonde qui a généré l'alerte.
    /// </summary>
    public Sonde Sonde { get; set; } = null!;

    /// <summary>
    /// Seuil d'alerte qui a été dépassé et qui a déclenché cette alerte.
    /// </summary>
    public SeuilAlerte SeuilAlerte { get; set; } = null!;

    // NOTE CRITIQUE : PAS de navigation property vers Releve.
    // Un relevé peut DÉCLENCHER une alerte via la logique métier,
    // mais il n'y a pas de relation physique (FK) entre Alerte et Releve.
}
