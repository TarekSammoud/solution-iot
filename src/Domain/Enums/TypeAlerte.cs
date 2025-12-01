namespace Domain.Enums;

/// <summary>
/// Type d'alerte à générer lorsqu'un seuil est dépassé.
/// Permet de différencier la criticité de la notification.
/// </summary>
public enum TypeAlerte
{
    /// <summary>
    /// Alerte critique nécessitant une action immédiate.
    /// Utilisée pour des dépassements de seuil graves qui nécessitent une intervention rapide.
    /// Exemple : température &lt; 15°C ou &gt; 30°C - risque pour l'équipement ou la santé.
    /// Priorité élevée, notifications actives (SMS, email, etc.).
    /// </summary>
    Alerte = 0,

    /// <summary>
    /// Avertissement informatif, moins critique qu'une alerte.
    /// Utilisé pour des dépassements de seuil modérés qui nécessitent une surveillance.
    /// Exemple : température &lt; 18°C ou &gt; 26°C - confort réduit mais pas de danger immédiat.
    /// Priorité modérée, notifications passives (log, dashboard).
    ///
    /// Note importante : Un avertissement n'empêche PAS la création d'une alerte pour le même TypeSeuil.
    /// Une sonde peut avoir à la fois un seuil "Minimum Avertissement" et un seuil "Minimum Alerte"
    /// avec des valeurs différentes pour créer plusieurs niveaux de notification.
    /// </summary>
    Avertissement = 1
}
