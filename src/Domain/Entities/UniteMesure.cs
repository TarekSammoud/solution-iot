using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Représente une unité de mesure pour un type de sonde spécifique.
/// Chaque unité de mesure est liée à un TypeSonde (ex: °C pour Température, % pour Hydrométrie).
/// Une même unité peut avoir différents symboles selon le type (ex: % pour humidité vs % pour qualité air).
/// </summary>
public class UniteMesure
{
    /// <summary>
    /// Identifiant unique de l'unité de mesure.
    /// Généré automatiquement lors de la création.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nom complet de l'unité de mesure (ex: "Celsius", "Fahrenheit", "Pourcentage").
    /// Requis, maximum 100 caractères.
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Symbole de l'unité de mesure (ex: "°C", "°F", "%", "µg/m³", "ppm").
    /// Requis, maximum 20 caractères.
    /// Peut contenir des caractères Unicode spéciaux (°, µ, ³, etc.).
    /// </summary>
    public string Symbole { get; set; } = string.Empty;

    /// <summary>
    /// Type de sonde auquel cette unité de mesure est associée.
    /// Permet de filtrer les unités disponibles selon le type de sonde.
    /// </summary>
    public TypeSonde TypeSonde { get; set; }

    /// <summary>
    /// Date de création de l'unité de mesure en UTC.
    /// Générée automatiquement lors de l'ajout en base de données.
    /// </summary>
    public DateTime DateCreation { get; set; }

    // Navigation properties
    // TODO: Ajouter la navigation vers Sonde quand l'entité sera créée
    // public ICollection<Sonde> Sondes { get; set; } = new List<Sonde>();
}
