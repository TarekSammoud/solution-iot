using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Représente un actionneur IoT (device contrôlable) qui peut effectuer des actions physiques.
/// Hérite de Device et ajoute des propriétés spécifiques aux actionneurs (type).
/// Exemples : ampoule connectée, moteur, volet roulant, chauffage.
/// </summary>
public class Actionneur : Device
{
    /// <summary>
    /// Type d'actionneur (AmpouleSimple, AmpouleVariometre, Moteur).
    /// Détermine les capacités de contrôle de l'actionneur (on/off, variateur, vitesse).
    /// </summary>
    public TypeActionneur TypeActionneur { get; set; }

    // Navigation properties

    /// <summary>
    /// État actuel de l'actionneur (allumé/éteint, intensité, etc.).
    /// Relation One-to-One optionnelle : un actionneur peut avoir un état actuel ou non.
    /// Null si aucun état n'a encore été enregistré pour cet actionneur.
    /// </summary>
    public EtatActionneur? EtatActuel { get; set; }
}
