namespace Domain.Enums;

/// <summary>
/// Type de seuil configuré pour une sonde.
/// Définit si le seuil représente une limite minimale ou maximale acceptable.
/// </summary>
public enum TypeSeuil
{
    /// <summary>
    /// Seuil minimal acceptable.
    /// Déclenche une alerte/avertissement si la valeur mesurée descend EN DESSOUS de ce seuil.
    /// Exemple : température minimale de 18°C - alerte si température &lt; 18°C.
    /// </summary>
    Minimum = 0,

    /// <summary>
    /// Seuil maximal acceptable.
    /// Déclenche une alerte/avertissement si la valeur mesurée dépasse ce seuil.
    /// Exemple : température maximale de 26°C - alerte si température &gt; 26°C.
    /// </summary>
    Maximum = 1
}
