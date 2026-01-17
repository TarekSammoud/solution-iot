🎯 Objectif
Implémenter la communication entre systèmes IoT pour permettre à votre plateforme de récupérer des sondes depuis d'autres systèmes étudiants (SystemePartenaire).

Cette étape est guidée car elle introduit :

Appels HTTP vers des API externes
Authentification Basic Auth
Gestion des erreurs de communication
Import de données depuis un système externe
Mapping entre formats de données différents
📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité SystemePartenaire dans la couche Domain
✅ CRUD SystemePartenaire fonctionnel
✅ CRUD Sonde fonctionnel
✅ CRUD Localisation et UniteMesure fonctionnels
✅ HttpClient configuré dans l'application
Connaissances requises
Pattern Repository et Clean Architecture
HttpClient en C#
Authentification HTTP Basic Auth
Gestion des erreurs et exceptions réseau
Parsing JSON
ASP.NET Core Web API
Blazor Server
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── External/
           └── ExternalSondeDto.cs
   └── Services/
       └── Interfaces/
           └── ISystemePartenaireService.cs (modification)
       └── SystemePartenaireService.cs (modification)

📁 IotPlatform.Api
   └── Controllers/
       └── SystemePartenaireController.cs (modification)

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── SystemePartenaire/
           └── ImportSondes.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/External/) :

ExternalSondeDto : pour recevoir les données d'une sonde externe (format peut différer du vôtre)
ImportSondeResultDto : résultat de l'import (succès, erreurs)
Service à modifier (dans Application/Services/) :

Ajouter dans ISystemePartenaireService :

Task<List<ExternalSondeDto>> GetSondesFromPartenaire(Guid systemPartenaireId)
Task<ImportSondeResultDto> ImportSondesFromPartenaire(Guid systemPartenaireId, Guid localisationCibleId)
Implémenter dans SystemePartenaireService :

Logique d'appel HTTP vers l'API du partenaire
Authentification Basic Auth
Parsing de la réponse JSON
Import des sondes dans la base locale
2️⃣ Couche API - Controller REST
Controller à modifier (dans Api/Controllers/) :

Ajouter dans SystemePartenaireController :
GET /api/systemepartenaire/{id}/sondes : liste les sondes disponibles chez le partenaire (sans les importer)
POST /api/systemepartenaire/{id}/import-sondes : importer les sondes du partenaire
3️⃣ Couche Presentation - Page Blazor
Page Blazor à créer (dans Web/Pages/SystemePartenaire/) :

ImportSondes.razor : interface pour importer des sondes depuis un partenaire
Navigation : Ajouter un bouton "Importer des sondes" dans la liste des SystemePartenaire

🔍 Spécifications fonctionnelles
Contexte
Chaque groupe d'étudiants développe son propre système IoT. Pour permettre la communication entre systèmes, vous devez pouvoir :

Consulter les sondes disponibles sur un système partenaire
Importer ces sondes dans votre propre système
Important : Les sondes importées deviennent des copies locales. Une fois importées, elles n'ont plus de lien avec le système d'origine.

Format attendu de l'API partenaire
Le système partenaire doit exposer un endpoint :

GET https://système-partenaire.com/api/sonde
Qui retourne un JSON :

[
  {
    "id": "guid",
    "nom": "Sonde Temperature Bureau",
    "typeSonde": "Temperature",
    "uniteMesureSymbole": "°C",
    "localisationNom": "Bureau A",
    "estActif": true,
    "dateInstallation": "2024-11-15T10:00:00Z"
  },
  ...
]
Note : Le format peut légèrement varier selon les groupes. Votre code doit être robuste.

Règles de validation
Pour GetSondesFromPartenaire :

✅ Le SystemePartenaire doit exister
✅ Le SystemePartenaire doit avoir UsernameAppel et PasswordChiffre configurés (mode "Appelant")
✅ L'UrlBase doit être valide et accessible
✅ Gérer les erreurs réseau (timeout, 404, 401, etc.)
Pour ImportSondesFromPartenaire :

✅ Même validations que GetSondesFromPartenaire
✅ La localisationCibleId doit exister (où importer les sondes)
✅ Vérifier que l'UniteMesure de chaque sonde existe localement (sinon, créer ou mapper)
✅ Ne pas importer les doublons (vérifier si une sonde avec le même nom et type existe déjà)
✅ Les sondes importées sont marquées comme inactives par défaut (EstActif = false)
Comportements attendus
Consultation des sondes (ImportSondes.razor - étape 1) :

Route : /systemepartenaire/{id}/import-sondes
Affichage du nom du système partenaire en en-tête
Bouton "Charger les sondes disponibles"
Quand on clique :
Appel de l'API pour récupérer les sondes du partenaire
Affichage dans un tableau : Nom, Type, Unité, Localisation d'origine, Statut
Checkbox pour sélectionner les sondes à importer
Bouton "Sélectionner tout" / "Tout déselectionner"
Si erreur réseau : afficher message d'erreur clair
Si authentification échoue (401) : afficher message spécifique
Import des sondes (ImportSondes.razor - étape 2) :

Sélection de la localisation cible (dropdown)
Bouton "Importer les sondes sélectionnées"
Quand on clique :
Appel de l'API pour importer
Affichage d'une barre de progression (optionnel)
Résultat de l'import :
Nombre de sondes importées avec succès
Liste des erreurs éventuelles (doublon, unité manquante, etc.)
Message de succès
Bouton "Retour à la liste des systèmes partenaires"
💡 Conseils d'implémentation - GUIDAGE
🔑 Configuration de HttpClient
Dans le projet API ou Application, configurer HttpClient :

Dans Program.cs ou Startup.cs :

builder.Services.AddHttpClient();
Injecter IHttpClientFactory dans SystemePartenaireService :

private readonly IHttpClientFactory _httpClientFactory;

public SystemePartenaireService(..., IHttpClientFactory httpClientFactory)
{
    _httpClientFactory = httpClientFactory;
}
🌐 Appel HTTP avec authentification
Méthode GetSondesFromPartenaire :

Récupérer le SystemePartenaire :
Vérifier qu'il a UsernameAppel et PasswordChiffre
Si manquants → retourner erreur
Créer le HttpClient :

var httpClient = _httpClientFactory.CreateClient();
httpClient.BaseAddress = new Uri(systemePartenaire.UrlBase);
httpClient.Timeout = TimeSpan.FromSeconds(30);
Ajouter l'authentification Basic Auth :

var credentials = Convert.ToBase64String(
   Encoding.ASCII.GetBytes($"{systemePartenaire.UsernameAppel}:{systemePartenaire.PasswordChiffre}")
);
httpClient.DefaultRequestHeaders.Authorization = 
   new AuthenticationHeaderValue("Basic", credentials);
Effectuer l'appel GET :

var response = await httpClient.GetAsync("/api/sonde");

if (!response.IsSuccessStatusCode)
{
   // Gérer les erreurs selon le code de statut
   if (response.StatusCode == HttpStatusCode.Unauthorized)
       throw new Exception("Authentification échouée");
   // ...
}

var json = await response.Content.ReadAsStringAsync();
var sondes = JsonSerializer.Deserialize<List<ExternalSondeDto>>(json);
return sondes;
Gérer les exceptions réseau :

try
{
   // appel HTTP
}
catch (HttpRequestException ex)
{
   throw new Exception("Erreur de communication avec le système partenaire", ex);
}
catch (TaskCanceledException ex)
{
   throw new Exception("Timeout lors de la communication", ex);
}
📥 Import des sondes
Méthode ImportSondesFromPartenaire :

Récupérer les sondes du partenaire :

var sondesExternes = await GetSondesFromPartenaire(systemPartenaireId);
Vérifier la localisation cible :

Récupérer via le repository
Si n'existe pas → erreur
Pour chaque sonde externe :

a. Vérifier si la sonde existe déjà localement :

Rechercher par Nom et TypeSonde
Si existe → skip (doublon)
b. Mapper/Créer l'UniteMesure :

Rechercher l'UniteMesure locale par Symbole et TypeSonde
Si n'existe pas → créer une nouvelle UniteMesure
c. Créer la nouvelle Sonde :

csharp var nouvelleSonde = new Sonde { Nom = sondeExterne.Nom + " (importée)", TypeSonde = sondeExterne.TypeSonde, UniteMesureId = uniteMesure.Id, LocalisationId = localisationCibleId, EstActif = false, // Inactif par défaut DateInstallation = sondeExterne.DateInstallation, DateCreation = DateTime.UtcNow, CanalCommunication = CanalCommunication.HttpPush, // Par défaut // ... autres propriétés Device avec valeurs par défaut };

d. Sauvegarder :

Via le repository Sonde
Retourner le résultat :

return new ImportSondeResultDto
{
   NombreImportees = compteurSucces,
   NombreDoublons = compteurDoublons,
   Erreurs = listeErreurs
};
🎨 Interface ImportSondes.razor
Structure de la page :

┌─────────────────────────────────────────────────┐
│ Import de sondes - [Nom du système partenaire]  │
└─────────────────────────────────────────────────┘

[Charger les sondes disponibles]

┌─────────────────────────────────────────────────┐
│ Sondes disponibles (5)                          │
├─────────────────────────────────────────────────┤
│ ☑ Sonde Temperature Bureau | Température | °C  │
│ ☑ Sonde Humidité Salle 1   | Hydrométrie | %   │
│ ☐ Sonde CO2 Open Space     | QualitéAir  | ppm │
│ ...                                             │
└─────────────────────────────────────────────────┘

[Tout sélectionner] [Tout déselectionner]

Localisation cible : [Dropdown]

[Importer les sondes sélectionnées (2)]

┌─────────────────────────────────────────────────┐
│ Résultat :                                      │
│ ✅ 2 sondes importées avec succès              │
│ ⚠️ 1 doublon ignoré                            │
└─────────────────────────────────────────────────┘
Logique Blazor :

Au chargement : afficher uniquement le bouton "Charger"

Après clic sur "Charger" :

Appeler l'API GET pour récupérer les sondes
Stocker dans une liste avec un booléen IsSelected
Afficher le tableau
Sélection :
Checkbox bindée à IsSelected
Boutons "Tout sélectionner" / "Tout déselectionner"
Import :
Vérifier qu'une localisation est sélectionnée
Filtrer les sondes sélectionnées
Appeler l'API POST pour importer
Afficher le résultat
🔒 Gestion du PasswordChiffre
Important : À ce stade, le PasswordChiffre est stocké en clair (voir Étape 3).

Pour l'utiliser dans l'authentification Basic Auth :

Le récupérer directement depuis systemePartenaire.PasswordChiffre
Pas de déchiffrement nécessaire pour le moment
Note : Dans une vraie application, il faudrait chiffrer ce mot de passe. Cela peut être une amélioration future.

✅ Critères de validation
Tests manuels à effectuer
1. Tester via l'API (avec Swagger ou Postman)

✅ GET /api/systemepartenaire/{id}/sondes retourne les sondes du partenaire
✅ GET avec système sans credentials retourne erreur 400
✅ GET avec URL invalide retourne erreur avec message clair
✅ GET avec credentials incorrects retourne erreur 401
✅ POST /api/systemepartenaire/{id}/import-sondes importe les sondes
✅ POST avec localisation invalide retourne erreur 400
✅ POST crée les UniteMesure manquantes automatiquement
✅ POST ignore les doublons
✅ POST marque les sondes importées comme inactives
2. Tester via l'interface Blazor

✅ Accéder à /systemepartenaire/{id}/import-sondes
✅ Cliquer sur "Charger les sondes" affiche les sondes du partenaire
✅ Si erreur réseau, message d'erreur clair
✅ Si erreur 401, message "Authentification échouée"
✅ Checkbox de sélection fonctionnent
✅ Boutons "Tout sélectionner" / "Tout déselectionner" fonctionnent
✅ Le nombre de sondes sélectionnées est affiché
✅ Sélectionner une localisation cible
✅ Importer les sondes sélectionnées
✅ Le résultat affiche le nombre importé et les doublons
✅ Les sondes apparaissent dans la liste locale (inactives)
3. Tester la communication entre deux systèmes étudiants

✅ Configurer un SystemePartenaire pointant vers le système d'un autre groupe
✅ Récupérer les sondes de l'autre groupe
✅ Importer ces sondes dans votre système
✅ Vérifier que les sondes sont bien créées localement
Checklist de code
[ ] ExternalSondeDto créé pour le format externe
[ ] ImportSondeResultDto créé pour le résultat
[ ] HttpClient configuré et injecté
[ ] Méthode GetSondesFromPartenaire implémentée
[ ] Authentification Basic Auth configurée
[ ] Gestion des erreurs HTTP (401, 404, timeout)
[ ] Méthode ImportSondesFromPartenaire implémentée
[ ] Vérification des doublons
[ ] Création automatique des UniteMesure manquantes
[ ] Sondes importées marquées comme inactives
[ ] Endpoints API ajoutés au controller
[ ] Page ImportSondes.razor créée
[ ] Interface de sélection fonctionnelle
[ ] Affichage des résultats d'import
[ ] Bouton dans la liste SystemePartenaire
[ ] Gestion des erreurs avec messages clairs
[ ] Messages de succès/erreur dans Blazor