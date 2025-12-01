using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Représente un seuil d'alerte configuré pour une sonde.
/// Définit une limite (minimale ou maximale) qui, si dépassée, déclenche une alerte ou un avertissement.
///
/// Concepts importants :
/// - Un avertissement n'empêche PAS la création d'une alerte pour le même TypeSeuil.
/// - Une sonde peut avoir plusieurs seuils simultanément, par exemple :
///   * Minimum Avertissement : 18°C (confort)
///   * Minimum Alerte : 15°C (critique)
///   * Maximum Avertissement : 26°C (confort)
///   * Maximum Alerte : 30°C (critique)
/// - Cela permet d'avoir plusieurs niveaux de notification avec différentes criticités.
/// </summary>
public class SeuilAlerte
{
    /// <summary>
    /// Identifiant unique du seuil d'alerte.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifiant de la sonde à laquelle ce seuil est associé.
    /// </summary>
    public Guid SondeId { get; set; }

    /// <summary>
    /// Type de seuil (Minimum ou Maximum).
    /// Détermine si le seuil représente une limite inférieure ou supérieure.
    /// </summary>
    public TypeSeuil TypeSeuil { get; set; }

    /// <summary>
    /// Type d'alerte à générer (Alerte ou Avertissement).
    /// Définit la criticité de la notification lorsque le seuil est dépassé.
    /// </summary>
    public TypeAlerte TypeAlerte { get; set; }

    /// <summary>
    /// Valeur du seuil.
    /// Précision de 10 chiffres dont 2 décimales (ex: 18.50).
    /// Exemple : pour une température minimale critique, Valeur = 15.0
    /// </summary>
    public decimal Valeur { get; set; }

    /// <summary>
    /// Indique si le seuil est actif ou non.
    /// Un seuil inactif ne génère pas d'alertes même si la condition est remplie.
    /// </summary>
    public bool EstActif { get; set; }

    /// <summary>
    /// Date et heure de création du seuil.
    /// </summary>
    public DateTime DateCreation { get; set; }

    // Navigation properties

    /// <summary>
    /// Sonde à laquelle ce seuil est associé.
    /// </summary>
    public Sonde Sonde { get; set; } = null!;

    /// <summary>
    /// Collection des alertes générées par ce seuil.
    /// Relation One-to-Many : un seuil peut générer plusieurs alertes dans le temps.
    /// NOTE : L'entité Alerte n'existe pas encore - cette propriété sera utilisée dans une future instruction.
    /// </summary>
    // TODO: Décommenter quand l'entité Alerte sera créée
    // public ICollection<Alerte> Alertes { get; set; } = new List<Alerte>();
}
