Étape 1 : CRUD UniteMesure
Completion requirements
🎯 Objectif
Implémenter la gestion complète (Create, Read, Update, Delete) des Unités de Mesure en suivant le pattern de l'exemple fourni pour les Localisations.

Cette première étape vous permet de vous familiariser avec l'architecture du projet en répliquant un pattern simple sur une entité similaire.

📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité UniteMesure dans la couche Domain
✅ Repository IUniteMesureRepository et son implémentation
✅ Base de données SQLite avec migrations appliquées
✅ Données de test pour les unités de mesure
✅ Exemple complet : CRUD Localisation (à utiliser comme référence)
Connaissances requises
Structure Clean Architecture (Domain, Application, Infrastructure, Presentation)
Pattern Repository
Mapperly pour le mapping objet-objet
ASP.NET Core Web API
Blazor Server
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── UniteMesure/
   └── Services/
       └── Interfaces/
           └── IUniteMesureService.cs
       └── UniteMesureService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── UniteMesureController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── UniteMesure/
           └── Index.razor
           └── Create.razor
           └── Edit.razor
           └── Delete.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/UniteMesure/) :

UniteMesureDto : pour lecture et affichage
CreateUniteMesureDto : pour création
UpdateUniteMesureDto : pour modification
Service à créer (dans Application/Services/) :

Interfaces/IUniteMesureService.cs : interface du service
UniteMesureService.cs : implémentation avec logique métier
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

UniteMesureController : endpoints REST pour CRUD
GET /api/unitemesure : liste toutes les unités
GET /api/unitemesure/{id} : détail d'une unité
POST /api/unitemesure : créer une unité
PUT /api/unitemesure/{id} : modifier une unité
DELETE /api/unitemesure/{id} : supprimer une unité
3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/UniteMesure/) :

Index.razor : liste des unités avec tableau
Create.razor : formulaire de création
Edit.razor : formulaire de modification
Delete.razor : confirmation de suppression
Navigation : Ajouter un lien "Unités de mesure" dans le menu principal

🔍 Spécifications fonctionnelles
Entité UniteMesure - Rappel
public class UniteMesure
{
    public Guid Id { get; set; }
    public string Symbole { get; set; }        // Ex: "°C", "%", "ppm"
    public string Nom { get; set; }            // Ex: "Degré Celsius"
    public TypeSonde TypeSonde { get; set; }   // Température, Hydrométrie, QualitéAir
}
Règles de validation
✅ Le Symbole est obligatoire (max 10 caractères)
✅ Le Nom est obligatoire (max 100 caractères)
✅ Le TypeSonde est obligatoire (enum)
✅ Pas de doublon : une unité avec le même symbole et type ne peut exister deux fois
Comportements attendus
Liste (Index) :

Affichage en tableau : Symbole, Nom, TypeSonde
Filtrage possible par TypeSonde (dropdown)
Liens vers Create, Edit, Delete pour chaque ligne
Création (Create) :

Formulaire avec 3 champs : Symbole, Nom, TypeSonde (select)
Validation côté client et serveur
Message de succès après création
Redirection vers Index
Modification (Edit) :

Formulaire pré-rempli
Validation identique à Create
Message de succès après modification
Redirection vers Index
Suppression (Delete) :

Page de confirmation avec détails de l'unité
Message d'avertissement si l'unité est utilisée par des sondes
Après suppression : redirection vers Index
💡 Conseils d'implémentation
📖 Utilisez l'exemple Localisation comme référence
Le projet contient déjà une implémentation complète du CRUD Localisation. Utilisez-la comme modèle :

Comparez les entités : Localisation vs UniteMesure

Les deux sont des entités simples sans relations complexes
Localisations a 3 champs (Nom, Batiment, Etage)
UniteMesure a 3 champs (Symbole, Nom, TypeSonde)
Adaptez les DTOs :

Reprenez la structure des DTOs Localisation
Remplacez les propriétés par celles d'UniteMesure
Attention : TypeSonde est un enum, pas un string
Reprenez la logique du Service :

Même pattern de méthodes (GetAll, GetById, Create, Update, Delete)
Utilisez Mapperly pour les conversions
Gardez la gestion des erreurs (try/catch)
Copiez le Controller :

Mêmes verbes HTTP (GET, POST, PUT, DELETE)
Même gestion des codes retour (200, 201, 404, 400)
Injection du service via constructeur
Adaptez les pages Blazor :

Même structure de formulaire
Remplacez les champs Localisation par UniteMesure
Pour TypeSonde : utilisez un <InputSelect> avec les valeurs de l'enum
⚙️ Configuration Mapperly
Ajoutez les mappings nécessaires pour UniteMesure dans votre classe de mapping (en vous inspirant de l'exemple Localisation).

🎨 Structure des pages Blazor
Index.razor :

Affichage en tableau avec les colonnes : Symbole, Nom, TypeSonde, Actions
Dropdown de filtrage par TypeSonde (avec option "Tous")
Liens vers Create, Edit, Delete
Bouton "Créer une unité" vers la page Create
Create.razor / Edit.razor :

Formulaire avec les champs : Symbole (input text), Nom (input text), TypeSonde (select avec les valeurs de l'enum)
Boutons : Enregistrer et Annuler
Messages de validation
Delete.razor :

Affichage des détails de l'unité à supprimer
Message de confirmation
Boutons : Confirmer et Annuler
✅ Critères de validation
Tests manuels à effectuer
Démarrer l'application (API + Blazor)

L'API doit être accessible sur https://localhost:5001
Blazor doit être accessible sur https://localhost:7001
Tester via l'API (avec Swagger ou Postman)

✅ GET /api/unitemesure retourne toutes les unités
✅ GET /api/unitemesure/{id} retourne une unité spécifique
✅ POST /api/unitemesure crée une nouvelle unité
✅ PUT /api/unitemesure/{id} modifie une unité
✅ DELETE /api/unitemesure/{id} supprime une unité
Tester via l'interface Blazor

✅ Accéder à /unitemesure affiche la liste
✅ Le filtrage par TypeSonde fonctionne
✅ Créer une unité avec données valides
✅ Modifier une unité existante
✅ Supprimer une unité
✅ Validation : impossible de créer sans Symbole/Nom
✅ Validation : pas de doublon (même Symbole + TypeSonde)
Checklist de code
[ ] DTOs créés avec propriétés appropriées
[ ] Service implémenté avec toutes les méthodes CRUD
[ ] Mapping Mapperly configuré
[ ] Controller REST avec tous les endpoints
[ ] Injection de dépendances correcte (constructeurs)
[ ] Pages Blazor avec formulaires et validation
[ ] Navigation ajoutée au menu
[ ] Gestion des erreurs (try/catch dans le service)
[ ] Messages de succès/erreur dans Blazor