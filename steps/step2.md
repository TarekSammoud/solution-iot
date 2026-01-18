
Étape 2 : CRUD User
Completion requirements
🎯 Objectif
Implémenter la gestion complète (Create, Read, Update, Delete) des Utilisateurs en suivant le même pattern que les instructions précédentes.

Cette étape vous permet de continuer à pratiquer l'architecture Clean Architecture avec une particularité : la gestion de l'enum RoleUtilisateur.

📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité User dans la couche Domain
✅ Repository IUserRepository et son implémentation
✅ Base de données SQLite avec migrations appliquées
✅ Données de test pour les utilisateurs
✅ Exemples complets : CRUD Localisation et CRUD UniteMesure
Connaissances requises
Pattern Repository et Clean Architecture
Gestion des enums en C#
Mapperly pour le mapping
ASP.NET Core Web API
Blazor Server avec formulaires
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── User/
   └── Services/
       └── Interfaces/
           └── IUserService.cs
       └── UserService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── UserController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── User/
           └── Index.razor
           └── Create.razor
           └── Edit.razor
           └── Delete.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/User/) :

UserDto : pour lecture et affichage
CreateUserDto : pour création
UpdateUserDto : pour modification
Service à créer (dans Application/Services/) :

Interfaces/IUserService.cs : interface du service
UserService.cs : implémentation avec logique métier
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

UserController : endpoints REST pour CRUD
GET /api/user : liste tous les utilisateurs
GET /api/user/{id} : détail d'un utilisateur
POST /api/user : créer un utilisateur
PUT /api/user/{id} : modifier un utilisateur
DELETE /api/user/{id} : supprimer un utilisateur
3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/User/) :

Index.razor : liste des utilisateurs avec tableau
Create.razor : formulaire de création
Edit.razor : formulaire de modification
Delete.razor : confirmation de suppression
Navigation : Ajouter un lien "Utilisateurs" dans le menu principal

🔍 Spécifications fonctionnelles
Entité User - Rappel
public class User
{
    public Guid Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public RoleUtilisateur Role { get; set; }
    public DateTime DateCreation { get; set; }
}

public enum RoleUtilisateur
{
    Utilisateur,
    Administrateur
}
Règles de validation
✅ Le Nom est obligatoire (max 100 caractères)
✅ Le Prenom est obligatoire (max 100 caractères)
✅ L'Email est obligatoire et doit être un email valide (format)
✅ Le Role est obligatoire (enum RoleUtilisateur)
✅ Pas de doublon : un utilisateur avec le même email ne peut exister deux fois
✅ La DateCreation est générée automatiquement à la création (non modifiable)
Comportements attendus
Liste (Index) :

Affichage en tableau : Nom, Prénom, Email, Rôle, Date de création
Filtrage possible par Rôle (dropdown : Tous, Utilisateur, Administrateur)
Tri par défaut : par Nom (ordre alphabétique)
Liens vers Create, Edit, Delete pour chaque ligne
Création (Create) :

Formulaire avec 4 champs : Nom, Prénom, Email, Rôle (select)
Validation côté client et serveur
DateCreation générée automatiquement (non saisie par l'utilisateur)
Message de succès après création
Redirection vers Index
Modification (Edit) :

Formulaire pré-rempli avec tous les champs sauf DateCreation (en lecture seule)
Possibilité de modifier : Nom, Prénom, Email, Rôle
Validation identique à Create
Message de succès après modification
Redirection vers Index
Suppression (Delete) :

Page de confirmation avec tous les détails de l'utilisateur
Message d'avertissement clair
Après suppression : redirection vers Index
💡 Conseils d'implémentation
📖 Utilisez les exemples précédents comme référence
Inspirez-vous des CRUD Localisation et UniteMesure déjà implémentés :

Structure des DTOs :
UserDto contient toutes les propriétés (y compris DateCreation)
CreateUserDto ne contient PAS DateCreation (sera générée côté service)
UpdateUserDto peut contenir ou non DateCreation selon votre choix (elle ne sera pas modifiable de toute façon)
Logique du Service :
Dans la méthode Create : générer DateCreation = DateTime.UtcNow
Validation de l'email (format + unicité)
Gestion des erreurs avec try/catch
Controller :
Même structure que les controllers précédents
Codes retour HTTP appropriés
Pages Blazor :
Pour le champ Rôle : utiliser un <InputSelect> avec les valeurs de l'enum RoleUtilisateur
Pour DateCreation en Edit : afficher en lecture seule (disabled ou DisplayText)
Validation email côté client avec DataAnnotations [EmailAddress]
⚙️ Points d'attention spécifiques
Gestion de l'enum RoleUtilisateur :

Dans les DTOs, le Role peut être de type RoleUtilisateur (enum)
Dans le formulaire Blazor, utiliser <InputSelect> avec @bind-Value="model.Role"
Peupler le select avec Enum.GetValues(typeof(RoleUtilisateur))
Validation de l'email :

Utiliser l'attribut [EmailAddress] dans les DTOs
Vérifier côté service qu'aucun autre utilisateur n'a le même email
DateCreation :

Générer automatiquement dans le service lors de la création
Ne jamais permettre la modification de cette date
🎨 Structure des pages Blazor
Index.razor :

Tableau avec colonnes : Nom, Prénom, Email, Rôle, Date de création, Actions
Dropdown de filtrage par Rôle (Tous, Utilisateur, Administrateur)
Bouton "Créer un utilisateur"
Formatage de la date : afficher au format court (dd/MM/yyyy)
Create.razor :

Formulaire avec : Nom, Prénom, Email, Rôle (select)
Pas de champ DateCreation (généré automatiquement)
Validation avec messages d'erreur
Boutons : Enregistrer et Annuler
Edit.razor :

Formulaire pré-rempli avec : Nom, Prénom, Email, Rôle
DateCreation affichée en lecture seule (label ou input disabled)
Validation identique à Create
Boutons : Enregistrer et Annuler
Delete.razor :

Affichage de tous les détails de l'utilisateur
Message : "Êtes-vous sûr de vouloir supprimer cet utilisateur ?"
Boutons : Confirmer la suppression et Annuler
✅ Critères de validation
Tests manuels à effectuer
Tester via l'API (avec Swagger ou Postman)
✅ GET /api/user retourne tous les utilisateurs
✅ GET /api/user/{id} retourne un utilisateur spécifique
✅ POST /api/user crée un nouvel utilisateur
✅ PUT /api/user/{id} modifie un utilisateur
✅ DELETE /api/user/{id} supprime un utilisateur
✅ POST avec email invalide retourne erreur 400
✅ POST avec email existant retourne erreur 400
Tester via l'interface Blazor
✅ Accéder à /user affiche la liste
✅ Le filtrage par Rôle fonctionne
✅ Créer un utilisateur avec données valides
✅ Créer un utilisateur avec email invalide affiche erreur
✅ Créer un utilisateur avec email existant affiche erreur
✅ Modifier un utilisateur existant
✅ La DateCreation n'est pas modifiable en Edit
✅ Supprimer un utilisateur
✅ Validation : tous les champs obligatoires sont vérifiés
Checklist de code
[ ] DTOs créés avec propriétés appropriées
[ ] CreateUserDto ne contient PAS DateCreation
[ ] Service implémenté avec toutes les méthodes CRUD
[ ] DateCreation générée automatiquement dans Create
[ ] Validation email (format + unicité)
[ ] Mapping Mapperly configuré
[ ] Controller REST avec tous les endpoints
[ ] Injection de dépendances correcte
[ ] Pages Blazor avec formulaires et validation
[ ] Select pour RoleUtilisateur fonctionnel
[ ] DateCreation en lecture seule dans Edit
[ ] Navigation ajoutée au menu
[ ] Gestion des erreurs (try/catch)
[ ] Messages de succès/erreur dans Blazor
