namespace Domain.Enums;

/// <summary>
/// Canal de communication utilisé pour échanger des données avec un device IoT.
/// Définit le mode de communication entre notre plateforme et le device (sonde ou actionneur).
/// </summary>
public enum CanalCommunication
{
    /// <summary>
    /// HTTP Push : Le device pousse les données vers notre API via webhook.
    /// Le device appelle notre endpoint HTTP pour envoyer ses données (mode push).
    /// Exemple : Une sonde envoie automatiquement ses relevés à notre API toutes les 5 minutes.
    /// </summary>
    HttpPush = 0,

    /// <summary>
    /// HTTP Pull : Notre système interroge le device via HTTP polling.
    /// Notre plateforme fait des requêtes HTTP régulières vers l'URL du device pour récupérer les données (mode pull).
    /// Exemple : Notre système interroge l'API du device toutes les 10 minutes pour obtenir les dernières mesures.
    /// </summary>
    HttpPull = 1,

    /// <summary>
    /// MQTT : Communication via broker MQTT (publish/subscribe).
    /// Le device et notre plateforme communiquent via un broker MQTT intermédiaire.
    /// Mode asynchrone et bidirectionnel, idéal pour les devices à faible consommation.
    /// Exemple : Le device publie sur un topic MQTT, notre plateforme s'y abonne.
    /// </summary>
    MQTT = 2,

    /// <summary>
    /// SignalR : Communication temps réel via SignalR Hub (WebSocket).
    /// Connexion persistante bidirectionnelle entre le device et notre plateforme via SignalR.
    /// Permet des échanges temps réel avec latence minimale.
    /// Exemple : Un actionneur reçoit des commandes instantanées via SignalR Hub.
    /// </summary>
    SignalR = 3
}
