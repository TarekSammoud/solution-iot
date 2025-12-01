namespace Domain.Enums;

/// <summary>
/// Types de sondes IoT supportés par le système.
/// Chaque type de sonde a des unités de mesure spécifiques (ex: °C, °F pour Température).
/// </summary>
public enum TypeSonde
{
    /// <summary>
    /// Sonde de température.
    /// Unités typiques: Celsius (°C), Fahrenheit (°F), Kelvin (K).
    /// </summary>
    Temperature = 0,

    /// <summary>
    /// Sonde d'hydrométrie (humidité).
    /// Unités typiques: Pourcentage (%), Humidité relative (% HR).
    /// </summary>
    Hydrometrie = 1,

    /// <summary>
    /// Sonde de qualité de l'air.
    /// Unités typiques: Particules (µg/m³), PPM (parties par million), IQA (indice).
    /// </summary>
    QualiteAir = 2
}
