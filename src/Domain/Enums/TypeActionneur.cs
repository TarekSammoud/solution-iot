namespace Domain.Enums;

/// <summary>
/// Type d'actionneur IoT supporté par le système.
/// Définit le type de device contrôlable et ses capacités (on/off, variateur, vitesse).
/// </summary>
public enum TypeActionneur
{
    /// <summary>
    /// Ampoule simple : contrôle on/off uniquement.
    /// Ampoule basique pouvant être allumée ou éteinte (état binaire).
    /// Pas de contrôle d'intensité lumineuse.
    /// Exemple : Ampoule LED standard connectée.
    /// </summary>
    AmpouleSimple = 0,

    /// <summary>
    /// Ampoule avec variateur : contrôle on/off et intensité 0-100%.
    /// Ampoule dimmable permettant de régler l'intensité lumineuse de 0% (éteint) à 100% (luminosité maximale).
    /// Exemple : Ampoule LED dimmable Philips Hue.
    /// </summary>
    AmpouleVariometre = 1,

    /// <summary>
    /// Moteur : contrôle on/off et vitesse de rotation 0-100%.
    /// Moteur électrique avec contrôle de vitesse variable de 0% (arrêt) à 100% (vitesse maximale).
    /// Exemple : Ventilateur connecté, moteur de volet roulant avec vitesse variable.
    /// </summary>
    Moteur = 2
}
