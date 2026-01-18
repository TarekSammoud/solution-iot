Étape 4 : CRUD Sonde (avec héritage Device)
Completion requirements
Instruction 4 : CRUD Sonde (avec héritage Device)
🎯 Objectif
Implémenter la gestion complète (Create, Read, Update, Delete) des Sondes en tenant compte de l'héritage depuis la classe abstraite Device.

Cette étape sera un peu plus guidée que les précédente car elle introduit des concepts plus avancés :

Héritage Table Per Hierarchy (TPH) dans Entity Framework Core
Gestion de propriétés communes via une classe abstraite
Relations avec d'autres entités (Localisation, UniteMesure)
Enum avec comportement spécifique (TypeSonde)
📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité abstraite Device dans la couche Domain
✅ Entité Sonde héritant de Device dans la couche Domain
✅ Repository ISondeRepository et son implémentation
✅ Base de données SQLite avec migrations appliquées (TPH configuré)
✅ Données de test pour les sondes
✅ CRUD Localisation et CRUD UniteMesure fonctionnels
✅ Exemples complets : CRUD précédents (Localisation, UniteMesure, User, SystemePartenaire)
Connaissances requises
Pattern Repository et Clean Architecture
Héritage en C# et en Entity Framework Core
Gestion des relations entre entités
Enums avec comportement conditionnel
Mapperly pour le mapping
ASP.NET Core Web API
Blazor Server avec formulaires
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── Sonde/
   └── Services/
       └── Interfaces/
           └── ISondeService.cs
       └── SondeService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── SondeController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── Sonde/
           └── Index.razor
           └── Create.razor
           └── Edit.razor
           └── Delete.razor
           └── Details.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/Sonde/) :

SondeDto : pour lecture et affichage
CreateSondeDto : pour création
UpdateSondeDto : pour modification
Service à créer (dans Application/Services/) :

Interfaces/ISondeService.cs : interface du service
SondeService.cs : implémentation avec logique métier
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

SondeController : endpoints REST pour CRUD
GET /api/sonde : liste toutes les sondes
GET /api/sonde/{id} : détail d'une sonde
GET /api/sonde/localisation/{localisationId} : sondes par localisation
GET /api/sonde/type/{typeSonde} : sondes par type
POST /api/sonde : créer une sonde
PUT /api/sonde/{id} : modifier une sonde
DELETE /api/sonde/{id} : supprimer une sonde
3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/Sonde/) :

Index.razor : liste des sondes avec tableau et filtres
Create.razor : formulaire de création
Edit.razor : formulaire de modification
Delete.razor : confirmation de suppression
Details.razor : vue détaillée d'une sonde
Navigation : Ajouter un lien "Sondes" dans le menu principal

🔍 Spécifications fonctionnelles
Entités - Rappel
Device (classe abstraite) :

public abstract class Device
{
    public Guid Id { get; set; }
    public string Nom { get; set; }
    public Guid LocalisationId { get; set; }
    public Localisation Localisation { get; set; }
    public bool EstActif { get; set; }
    public DateTime DateInstallation { get; set; }
    public DateTime DateCreation { get; set; }
    public CanalCommunication CanalCommunication { get; set; }
    public string? UrlDevice { get; set; }
    public string? CredentialsDevice { get; set; }
}
Sonde (hérite de Device) :

public class Sonde : Device
{
    public TypeSonde TypeSonde { get; set; }
    public Guid UniteMesureId { get; set; }
    public UniteMesure UniteMesure { get; set; }
    public decimal? ValeurMin { get; set; }
    public decimal? ValeurMax { get; set; }
}
Enums :

public enum TypeSonde
{
    Temperature,
    Hydrometrie,
    QualiteAir
}

public enum CanalCommunication
{
    HttpPush,
    HttpPull,
    MQTT,
    SignalR
}
Règles de validation
Propriétés héritées de Device :

✅ Le Nom est obligatoire (max 100 caractères)
✅ La LocalisationId est obligatoire (Guid valide, localisation existante)
✅ EstActif : booléen (par défaut true à la création)
✅ DateInstallation : obligatoire, ne peut pas être dans le futur
✅ DateCreation : générée automatiquement (non modifiable)
✅ CanalCommunication : obligatoire (enum)
✅ UrlDevice : optionnel, mais obligatoire si CanalCommunication = HttpPull, MQTT ou SignalR
✅ CredentialsDevice : optionnel
Propriétés spécifiques à Sonde :

✅ TypeSonde : obligatoire (enum)
✅ UniteMesureId : obligatoire (Guid valide, unité de mesure existante)
✅ Cohérence TypeSonde ↔ UniteMesure : l'unité de mesure doit correspondre au type de sonde
Exemple : Une sonde Temperature ne peut avoir qu'une unité de type Temperature (°C, °F, K)
✅ ValeurMin : optionnel, si renseigné doit être < ValeurMax
✅ ValeurMax : optionnel, si renseigné doit être > ValeurMin
Comportements attendus
Liste (Index) :

Affichage en tableau : Nom, Type, Localisation, Unité, Statut (Actif/Inactif), DateInstallation, Actions
Filtres multiples :
Par TypeSonde (dropdown : Tous, Temperature, Hydrometrie, QualiteAir)
Par Localisation (dropdown : Toutes, puis liste des localisations)
Par Statut (checkbox : Afficher uniquement les actives)
Tri par défaut : par Nom
Badge coloré pour le statut (vert = Actif, gris = Inactif)
Liens vers Details, Edit, Delete pour chaque ligne
Bouton "Créer une sonde"
Création (Create) :

Formulaire organisé en sections :
Informations générales : Nom, TypeSonde, EstActif
Localisation et mesure : LocalisationId (select), UniteMesureId (select filtré par TypeSonde)
Plages de mesure : ValeurMin, ValeurMax (optionnels)
Communication : CanalCommunication (select), UrlDevice (conditionnel), CredentialsDevice (optionnel)
Installation : DateInstallation (date picker)
Comportement dynamique :
Quand TypeSonde change → filtrer la liste des UniteMesure pour n'afficher que celles du même type
Quand CanalCommunication = HttpPush → UrlDevice devient optionnel (grisé)
Quand CanalCommunication ≠ HttpPush → UrlDevice devient obligatoire
DateCreation générée automatiquement (non affichée)
Validation côté client et serveur
Message de succès après création
Redirection vers Index
Modification (Edit) :

Formulaire pré-rempli avec la même structure que Create
DateCreation affichée en lecture seule
Même comportement dynamique (filtrage UniteMesure, UrlDevice conditionnelle)
Validation identique à Create
Message de succès après modification
Redirection vers Index
Détails (Details) :

Affichage de toutes les informations de la sonde
Organisation en sections (comme le formulaire)
Affichage du nom de la Localisation (pas juste l'Id)
Affichage du symbole de l'UniteMesure
Boutons : Modifier, Supprimer, Retour à la liste
Suppression (Delete) :

Page de confirmation avec tous les détails de la sonde
Message d'avertissement si la sonde a des relevés associés
Après suppression : redirection vers Index
💡 Conseils d'implémentation - GUIDAGE
🔑 Gestion de l'héritage Device
Comprendre le TPH (Table Per Hierarchy) :

En base de données, Device et Sonde partagent la même table
EF Core ajoute une colonne discriminante (Discriminator) pour distinguer Sonde vs Actionneur
Cette configuration est déjà faite dans les migrations
Dans les DTOs :

Vos DTOs Sonde doivent contenir toutes les propriétés (Device + Sonde)
Pas besoin de créer un DeviceDto séparé
Exemple de structure pour SondeDto :
SondeDto
├── Propriétés de Device (Id, Nom, LocalisationId, etc.)
└── Propriétés de Sonde (TypeSonde, UniteMesureId, ValeurMin, ValeurMax)
Dans le Service :

Le repository ISondeRepository retourne déjà des objets Sonde (avec toutes les propriétés héritées)
Lors de la création/modification, remplir toutes les propriétés (Device + Sonde)
DateCreation générée automatiquement dans Create
Par défaut, EstActif = true à la création
🔗 Gestion des relations
Chargement des données liées :

Dans le service, utiliser le repository pour charger les relations (Localisation, UniteMesure)
Les repositories incluent déjà les méthodes nécessaires (eager loading)
Dans les DTOs :

Inclure les Id (LocalisationId, UniteMesureId) pour les formulaires
Inclure les noms (LocalisationNom, UniteMesureSymbole) pour l'affichage
Exemple dans SondeDto : LocalisationId + LocalisationNom
Dans le Controller :

Endpoint pour filtrer par localisation : récupérer les sondes via le repository avec un filtre
Endpoint pour filtrer par type : idem
📋 Validation métier complexe
Cohérence TypeSonde ↔ UniteMesure :

Dans le service Create/Update, avant d'enregistrer :
Récupérer l'UniteMesure via son Id
Vérifier que uniteMesure.TypeSonde == sonde.TypeSonde
Si différent → retourner une erreur de validation
Validation ValeurMin/ValeurMax :

Si les deux sont renseignés : vérifier que ValeurMin < ValeurMax
Si une seule est renseignée : accepter
Validation UrlDevice conditionnelle :

Si CanalCommunication = HttpPush → UrlDevice peut être null
Sinon → UrlDevice doit être renseignée et valide (format URL)
🎨 Comportement dynamique dans Blazor
Filtrage dynamique des UniteMesure :

Dans Create/Edit, avoir une liste complète des UniteMesure chargée au démarrage
Quand TypeSonde change (événement @onchange sur le select) :
Filtrer la liste pour ne garder que les UniteMesure du même TypeSonde
Réinitialiser la sélection d'UniteMesure si l'actuelle n'est plus valide
UrlDevice conditionnelle :

Quand CanalCommunication = HttpPush :
Désactiver le champ UrlDevice (attribute disabled)
Vider la valeur
Sinon :
Activer le champ
Marquer comme requis
Exemple de logique :

Dans le code-behind de Create.razor :

Méthode OnTypeSondeChanged() appelée quand TypeSonde change
Méthode OnCanalCommunicationChanged() appelée quand CanalCommunication change
🔍 Structure de la page Index avec filtres
Filtres à implémenter :

Dropdown TypeSonde : binder à une variable selectedTypeSonde
Dropdown Localisation : binder à une variable selectedLocalisationId
Checkbox "Actives uniquement" : binder à une variable activesOnly
Logique de filtrage :

Charger toutes les sondes au démarrage
Créer une propriété computed sondesFiltrees qui applique les filtres
Rafraîchir l'affichage quand un filtre change
Affichage du statut :

Si EstActif = true → badge vert avec texte "Actif"
Si EstActif = false → badge gris avec texte "Inactif"
✅ Critères de validation
Tests manuels à effectuer
Tester via l'API (avec Swagger ou Postman)
✅ GET /api/sonde retourne toutes les sondes
✅ GET /api/sonde/{id} retourne une sonde spécifique avec Localisation et UniteMesure
✅ GET /api/sonde/localisation/{id} retourne les sondes d'une localisation
✅ GET /api/sonde/type/Temperature retourne uniquement les sondes de température
✅ POST /api/sonde crée une nouvelle sonde
✅ POST avec UniteMesure incohérente retourne erreur 400
✅ POST avec UrlDevice manquante (si CanalCommunication ≠ HttpPush) retourne erreur 400
✅ POST avec ValeurMin > ValeurMax retourne erreur 400
✅ PUT /api/sonde/{id} modifie une sonde
✅ DELETE /api/sonde/{id} supprime une sonde
Tester via l'interface Blazor
✅ Accéder à /sonde affiche la liste
✅ Filtrer par TypeSonde fonctionne
✅ Filtrer par Localisation fonctionne
✅ Filtrer par statut actif/inactif fonctionne
✅ Créer une sonde Temperature avec une unité °C
✅ Changer TypeSonde → la liste des UniteMesure se filtre automatiquement
✅ Créer une sonde avec CanalCommunication = HttpPush (sans UrlDevice) : OK
✅ Créer une sonde avec CanalCommunication = HttpPull (sans UrlDevice) : erreur
✅ Créer une sonde avec ValeurMin > ValeurMax : erreur
✅ Modifier une sonde existante
✅ Voir les détails d'une sonde
✅ Supprimer une sonde
✅ DateCreation non modifiable en Edit
Checklist de code
[ ] DTOs créés avec toutes les propriétés (Device + Sonde)
[ ] DTOs incluent les Ids ET les noms des entités liées
[ ] Service implémenté avec toutes les méthodes CRUD
[ ] Validation cohérence TypeSonde ↔ UniteMesure
[ ] Validation conditionnelle UrlDevice selon CanalCommunication
[ ] Validation ValeurMin < ValeurMax
[ ] DateCreation générée automatiquement dans Create
[ ] EstActif = true par défaut dans Create
[ ] Mapping Mapperly configuré
[ ] Controller REST avec tous les endpoints (+ filtres)
[ ] Injection de dépendances correcte
[ ] Pages Blazor avec formulaires et validation
[ ] Filtrage dynamique UniteMesure dans Create/Edit
[ ] Comportement conditionnel UrlDevice dans Create/Edit
[ ] Filtres multiples dans Index (TypeSonde, Localisation, Statut)
[ ] Badge coloré pour le statut
[ ] Page Details avec affichage complet
[ ] Navigation ajoutée au menu
[ ] Gestion des erreurs (try/catch)
[ ] Messages de succès/erreur dans Blazor
