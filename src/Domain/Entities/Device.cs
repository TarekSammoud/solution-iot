using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Classe abstraite représentant un device IoT (sonde ou actionneur).
/// Contient les propriétés communes à tous les devices connectés de la plateforme.
/// Utilise Table Per Hierarchy (TPH) pour stocker Sonde et Actionneur dans la même table.
/// </summary>
public abstract class Device
{
    /// <summary>
    /// Identifiant unique du device.
    /// Généré automatiquement lors de la création.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nom descriptif du device (ex: "Capteur Température Salon", "Ampoule Chambre").
    /// Requis, maximum 200 caractères.
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant de la localisation où se trouve le device.
    /// Clé étrangère vers Localisation.
    /// Permet de grouper les devices par pièce/zone.
    /// </summary>
    public Guid LocalisationId { get; set; }

    /// <summary>
    /// Indique si le device est actif et opérationnel.
    /// False si le device est hors service, en maintenance ou désactivé.
    /// </summary>
    public bool EstActif { get; set; }

    /// <summary>
    /// Date d'installation physique du device sur le terrain (UTC).
    /// Permet de suivre l'âge et l'historique du matériel.
    /// </summary>
    public DateTime DateInstallation { get; set; }

    /// <summary>
    /// Date de création de l'enregistrement en base de données (UTC).
    /// Générée automatiquement lors de l'ajout.
    /// </summary>
    public DateTime DateCreation { get; set; }

    /// <summary>
    /// Canal de communication utilisé pour échanger avec le device.
    /// Détermine le protocole et le mode de communication (push, pull, MQTT, SignalR).
    /// </summary>
    public CanalCommunication CanalCommunication { get; set; }

    /// <summary>
    /// URL du device pour la communication HTTP (optionnel).
    /// Utilisé pour HttpPull (notre système appelle cette URL) ou HttpPush (webhook callback).
    /// Null si le canal est MQTT ou SignalR.
    /// Exemple : "https://192.168.1.50:8080/api/sensor"
    /// Maximum 500 caractères.
    /// </summary>
    public string? UrlDevice { get; set; }

    /// <summary>
    /// Credentials chiffrés pour s'authentifier auprès du device (optionnel).
    /// Utilisé pour HttpPull quand notre système doit s'authentifier vers le device.
    /// Stocké sous forme chiffrée (réversible) pour permettre l'authentification.
    /// Null si pas d'authentification requise ou si le device nous appelle (HttpPush).
    /// Maximum 1000 caractères (le chiffrement prend de la place).
    /// </summary>
    public string? CredentialsDevice { get; set; }

    /// <summary>
    /// Valeur minimale de mesure pour les sondes (optionnel).
    /// Utilisé pour définir la plage de mesure valide de la sonde.
    /// Null si aucune limite minimale n'est définie.
    /// </summary>
    public decimal? ValeurMin { get; set; }

    /// <summary>
    /// Valeur maximale de mesure pour les sondes (optionnel).
    /// Utilisé pour définir la plage de mesure valide de la sonde.
    /// Null si aucune limite maximale n'est définie.
    /// </summary>
    public decimal? ValeurMax { get; set; }

    // Navigation properties

    /// <summary>
    /// Localisation où se trouve le device.
    /// Relation Many-to-One : plusieurs devices peuvent être dans une même localisation.
    /// </summary>
    public Localisation Localisation { get; set; } = null!;
}
