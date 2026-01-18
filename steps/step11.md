Étape 11 : Communication Device via CanalCommunication
Completion requirements
🎯 Objectif
Implémenter la communication avec les devices physiques (sondes et actionneurs) en fonction du canal de communication configuré.

Cette étape est guidée car elle introduit différents protocoles de communication :

HttpPull : Interroger périodiquement un device via HTTP GET
HttpPush : Recevoir des données du device via HTTP POST
MQTT (optionnel) : Communication via broker MQTT
SignalR (optionnel) : Communication temps réel bidirectionnelle
Cette étape se concentrera principalement sur HttpPull et HttpPush, les deux modes les plus simples.

📚 Prérequis
Éléments déjà en place dans le projet
✅ Enum CanalCommunication dans Device
✅ Propriétés UrlDevice et CredentialsDevice dans Device
✅ CRUD Sonde et Actionneur fonctionnels
✅ Gestion des Relevés fonctionnelle
✅ HttpClient configuré (Étape 9)
Connaissances requises
HttpClient en C#
Services en arrière-plan (BackgroundService)
Authentification HTTP
ASP.NET Core Web API endpoints
JSON parsing
Gestion des erreurs réseau
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── Device/
           └── DeviceDataDto.cs
   └── Services/
       └── Interfaces/
           └── IDeviceCommunicationService.cs
       └── DeviceCommunicationService.cs
       └── BackgroundServices/
           └── HttpPullBackgroundService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── DeviceWebhookController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── Device/
           └── TestCommunication.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Services
DTOs à créer (dans Application/DTOs/Device/) :

DeviceDataDto : format standard pour recevoir/envoyer des données device
TestCommunicationResultDto : résultat d'un test de communication
Services à créer (dans Application/Services/) :

Interfaces/IDeviceCommunicationService.cs : interface du service
DeviceCommunicationService.cs : logique de communication avec les devices
BackgroundServices/HttpPullBackgroundService.cs : service en arrière-plan pour HttpPull
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

DeviceWebhookController : endpoints pour recevoir les données des devices (HttpPush)

POST /api/webhook/device/{deviceId} : recevoir les données d'un device
Endpoints existants à modifier :

Ajouter dans SondeController :
POST /api/sonde/{id}/test-communication : tester la communication avec une sonde
3️⃣ Couche Presentation - Page Blazor
Page Blazor à créer (dans Web/Pages/Device/) :

TestCommunication.razor : interface pour tester la communication avec un device
Intégration : Ajouter un bouton "Tester la communication" dans la page Details de Sonde

🔍 Spécifications fonctionnelles
Modes de communication
1. HttpPull : Notre système interroge le device

Le système interroge périodiquement le device via HTTP GET à son UrlDevice.

Comportement :

Toutes les X minutes (configurable, par défaut 5 min), interroger chaque sonde avec CanalCommunication = HttpPull
Appel GET sur {UrlDevice}/data ou {UrlDevice} selon la configuration du device
Authentification si CredentialsDevice est défini
Parser la réponse JSON pour extraire la valeur
Créer automatiquement un Releve avec TypeReleve = Automatique
Format attendu de la réponse du device :

{
  "value": 23.5
}
2. HttpPush : Le device envoie les données à notre système

Le device appelle notre API pour envoyer ses données.

Comportement :

Exposer un endpoint webhook : POST /api/webhook/device/{deviceId}
Recevoir les données du device
Valider que le device existe et est actif
Authentifier le device (optionnel) via un token ou credentials
Parser les données
Créer automatiquement un Releve avec TypeReleve = Automatique
Retourner 200 OK ou erreur
Format attendu du payload :

{
  "value": 23.5
}
Règles de validation
Pour HttpPull :

✅ La sonde doit avoir CanalCommunication = HttpPull
✅ La sonde doit avoir une UrlDevice définie
✅ La sonde doit être active (EstActif = true)
✅ Gérer les erreurs réseau (timeout, 404, 500)
✅ Logger les erreurs sans bloquer le processus
Pour HttpPush :

✅ Le device (sonde ou actionneur) doit exister
✅ Le device doit être actif
✅ La valeur doit être valide (nombre décimal)
✅ Le timestamp doit être valide (pas dans le futur)
✅ Créer le relevé automatiquement si c'est une sonde
Comportements attendus
Service en arrière-plan HttpPull :

Démarre automatiquement au lancement de l'application
Toutes les 5 minutes :
Récupérer toutes les sondes avec CanalCommunication = HttpPull et EstActif = true
Pour chaque sonde, appeler son UrlDevice
Si succès : créer un relevé automatique
Si erreur : logger et continuer avec la sonde suivante
Logger le nombre de sondes interrogées et le nombre de succès/échecs
Endpoint Webhook (HttpPush) :

Route : POST /api/webhook/device/{deviceId}
Valider que le device existe et est actif
Parser le payload JSON
Créer un relevé automatique
Retourner 201 Created avec le relevé créé
Si erreur : retourner 400 Bad Request avec détails
Test de communication :

Page : /device/{id}/test-communication
Bouton "Tester maintenant"
Effectue un appel immédiat au device (si HttpPull)
Affiche le résultat :
Succès : valeur reçue, timestamp
Erreur : message d'erreur détaillé
Ne crée PAS de relevé (c'est juste un test)
💡 Conseils d'implémentation - GUIDAGE
🔑 DeviceCommunicationService
Méthode PullDataFromDevice(Sonde sonde) :

Vérifier les prérequis :
CanalCommunication = HttpPull
UrlDevice défini
EstActif = true
Créer le HttpClient :

var httpClient = _httpClientFactory.CreateClient();
httpClient.BaseAddress = new Uri(sonde.UrlDevice);
httpClient.Timeout = TimeSpan.FromSeconds(10);
Ajouter l'authentification si CredentialsDevice est défini :

Similaire à l'Étape 9 (Basic Auth)

Effectuer l'appel GET :

var response = await httpClient.GetAsync("/data");

if (!response.IsSuccessStatusCode)
{
   // Logger l'erreur
   return null;
}

var json = await response.Content.ReadAsStringAsync();
var data = JsonSerializer.Deserialize<DeviceDataDto>(json);
return data;
Créer le relevé :

Si data != null, créer un Releve avec :

SondeId = sonde.Id
Valeur = data.Value
DateHeure = data.Timestamp (ou DateTime.UtcNow si absent)
TypeReleve = Automatique
Gérer les exceptions :

try
{
   // appel HTTP
}
catch (HttpRequestException ex)
{
   _logger.LogError($"Erreur HTTP avec {sonde.Nom}: {ex.Message}");
   return null;
}
catch (TaskCanceledException ex)
{
   _logger.LogError($"Timeout avec {sonde.Nom}");
   return null;
}
Méthode ReceiveDataFromDevice(Guid deviceId, DeviceDataDto data) :

Pour HttpPush, quand on reçoit des données via le webhook :

Récupérer le device :
Via le repository (peut être Sonde ou Actionneur)
Vérifier qu'il existe et est actif
Si c'est une Sonde :
Créer un Releve avec les données
Retourner le relevé créé
Si c'est un Actionneur :
Mettre à jour son EtatActionneur avec les données
Retourner l'état mis à jour
🔄 HttpPullBackgroundService
Service en arrière-plan :

Hérite de BackgroundService :

public class HttpPullBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HttpPullBackgroundService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var communicationService = scope.ServiceProvider
                    .GetRequiredService<IDeviceCommunicationService>();
                var sondeService = scope.ServiceProvider
                    .GetRequiredService<ISondeService>();

                // Récupérer les sondes HttpPull actives
                var sondes = await sondeService.GetByCanalCommunication(CanalCommunication.HttpPull);

                _logger.LogInformation($"Interrogation de {sondes.Count} sondes HttpPull");

                int success = 0;
                int errors = 0;

                foreach (var sonde in sondes)
                {
                    var result = await communicationService.PullDataFromDevice(sonde);
                    if (result != null)
                        success++;
                    else
                        errors++;
                }

                _logger.LogInformation($"Résultat: {success} succès, {errors} échecs");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur dans HttpPullBackgroundService: {ex.Message}");
            }

            // Attendre 5 minutes
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
Enregistrement du service :

Dans Program.cs :

builder.Services.AddHostedService<HttpPullBackgroundService>();
📥 DeviceWebhookController
Endpoint pour recevoir les données :

[HttpPost("device/{deviceId}")]
public async Task<IActionResult> ReceiveDeviceData(Guid deviceId, [FromBody] DeviceDataDto data)
{
    try
    {
        var result = await _deviceCommunicationService.ReceiveDataFromDevice(deviceId, data);

        if (result == null)
            return NotFound($"Device {deviceId} not found or inactive");

        return Created("", result);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Erreur webhook device {deviceId}: {ex.Message}");
        return BadRequest(ex.Message);
    }
}
🎨 Page TestCommunication.razor
Interface de test :

┌─────────────────────────────────────────────────┐
│ Test de communication - [Nom de la sonde]       │
└─────────────────────────────────────────────────┘

Type: Température
Canal: HttpPull
URL: https://device-simulator.com/sonde/123

[Tester maintenant]

┌─────────────────────────────────────────────────┐
│ Résultat du test:                               │
│                                                 │
│ ✅ Succès                                       │
│ Valeur reçue: 23.5°C                            │
│ Timestamp: 30/11/2024 14:30:00                  │
│ Durée: 245ms                                    │
└─────────────────────────────────────────────────┘

[Retour aux détails]
Logique :

Bouton "Tester maintenant" appelle l'API de test
Affichage du spinner pendant l'appel
Affichage du résultat (succès ou erreur)
Pas de création de relevé (juste un test)
🔒 Authentification des devices
Pour CredentialsDevice :

Si le champ est défini, format attendu : username:password

Parser et utiliser pour Basic Auth :

if (!string.IsNullOrEmpty(sonde.CredentialsDevice))
{
    var parts = sonde.CredentialsDevice.Split(':');
    if (parts.Length == 2)
    {
        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{parts[0]}:{parts[1]}")
        );
        httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic", credentials);
    }
}
✅ Critères de validation
Tests manuels à effectuer
1. Simuler un device HttpPull

Créer un endpoint de test (ou utiliser un outil comme Mockoon) qui répond à GET /data :

{
  "value": 23.5
}
Tests :

✅ Créer une sonde avec CanalCommunication = HttpPull et UrlDevice pointant vers le mock
✅ Démarrer l'application
✅ Vérifier que le BackgroundService interroge la sonde toutes les 5 minutes
✅ Vérifier qu'un relevé automatique est créé à chaque interrogation
✅ Vérifier les logs (nombre de sondes interrogées, succès/échecs)
2. Simuler un device HttpPush

Utiliser Postman ou curl pour envoyer des données :

POST https://localhost:5001/api/webhook/device/{guid-sonde}
Content-Type: application/json

{
  "value": 25.3
}
Tests :

✅ Créer une sonde avec CanalCommunication = HttpPush
✅ Envoyer des données via Postman
✅ Vérifier qu'un relevé automatique est créé
✅ Vérifier que l'API retourne 201 Created
✅ Envoyer des données pour un device inexistant → 404
✅ Envoyer des données pour un device inactif → 404 ou 400
3. Tester la communication

✅ Accéder à /device/{id}/test-communication
✅ Tester une sonde HttpPull fonctionnelle → succès
✅ Tester une sonde HttpPull avec URL invalide → erreur claire
✅ Tester une sonde HttpPull avec timeout → erreur de timeout
✅ Vérifier qu'aucun relevé n'est créé lors d'un test
4. Tester via l'API

✅ POST /api/sonde/{id}/test-communication retourne le résultat du test
✅ POST /api/webhook/device/{id} crée un relevé
✅ POST avec données invalides retourne 400
Checklist de code
[ ] DeviceDataDto créé
[ ] TestCommunicationResultDto créé
[ ] DeviceCommunicationService créé
[ ] Méthode PullDataFromDevice implémentée
[ ] Méthode ReceiveDataFromDevice implémentée
[ ] Authentification Basic Auth si CredentialsDevice défini
[ ] Gestion des erreurs HTTP (timeout, 404, etc.)
[ ] HttpPullBackgroundService créé et hérite de BackgroundService
[ ] Boucle d'interrogation toutes les 5 minutes
[ ] Logging des succès et échecs
[ ] Service enregistré comme HostedService
[ ] DeviceWebhookController créé
[ ] Endpoint POST webhook fonctionnel
[ ] Endpoint test communication dans SondeController
[ ] Page TestCommunication.razor créée
[ ] Interface de test fonctionnelle
[ ] Affichage du résultat (succès/erreur)
[ ] Bouton dans page Details de Sonde
[ ] Relevés automatiques créés avec TypeReleve = Automatique
[ ] Pas de relevé lors d'un test
[ ] Gestion des erreurs avec messages clairs