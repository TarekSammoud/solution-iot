namespace Domain.Enums;

/// <summary>
/// Type de relevé de mesure effectué par une sonde.
/// Indique si le relevé a été saisi manuellement ou collecté automatiquement.
/// </summary>
public enum TypeReleve
{
    /// <summary>
    /// Relevé saisi manuellement par un utilisateur.
    /// Utilisé lorsqu'un opérateur entre une valeur manuellement dans le système.
    /// </summary>
    Manuel = 0,

    /// <summary>
    /// Relevé effectué automatiquement par la sonde.
    /// Utilisé pour les mesures collectées automatiquement par les dispositifs IoT.
    /// </summary>
    Automatique = 1
}
