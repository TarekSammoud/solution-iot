using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Représente une sonde IoT (capteur) qui mesure des données physiques.
/// Hérite de Device et ajoute des propriétés spécifiques aux capteurs (type, unité de mesure).
/// Exemples : capteur de température, capteur d'humidité, capteur de qualité de l'air.
/// </summary>
public class Sonde : Device
{
    /// <summary>
    /// Type de sonde (Température, Hydrométrie, Qualité Air).
    /// Détermine le type de mesure effectuée par la sonde.
    /// </summary>
    public TypeSonde TypeSonde { get; set; }

    /// <summary>
    /// Identifiant de l'unité de mesure utilisée par cette sonde.
    /// Clé étrangère vers UniteMesure.
    /// Exemple : Une sonde de température peut utiliser °C, °F ou K.
    /// </summary>
    public Guid UniteMesureId { get; set; }

    // Navigation properties

    /// <summary>
    /// Unité de mesure utilisée par cette sonde.
    /// Relation Many-to-One : plusieurs sondes peuvent utiliser la même unité de mesure.
    /// </summary>
    public UniteMesure UniteMesure { get; set; } = null!;

    // TODO: Décommenter ces navigation properties quand les entités seront créées

    /// <summary>
    /// Collection des relevés de mesure effectués par cette sonde.
    /// Relation One-to-Many : une sonde peut avoir de nombreux relevés dans le temps.
    /// </summary>
    // public ICollection<Releve> Releves { get; set; } = new List<Releve>();

    /// <summary>
    /// Collection des seuils d'alerte configurés pour cette sonde.
    /// Relation One-to-Many : une sonde peut avoir plusieurs seuils (min, max, critique).
    /// </summary>
    // public ICollection<SeuilAlerte> SeuilsAlerte { get; set; } = new List<SeuilAlerte>();

    /// <summary>
    /// Collection des alertes déclenchées par cette sonde.
    /// Relation One-to-Many : une sonde peut déclencher de nombreuses alertes dans le temps.
    /// </summary>
    // public ICollection<Alerte> Alertes { get; set; } = new List<Alerte>();
}
