Étape 3 : CRUD SystemePartenaire
Completion requirements
🎯 Objectif
Implémenter la gestion complète (Create, Read, Update, Delete) des Systèmes Partenaires en suivant le même pattern que les instructions précédentes.

Cette étape introduit la notion de communication inter-systèmes : votre plateforme IoT pourra référencer d'autres systèmes étudiants pour échanger des données de sondes.

Note importante : À ce stade, vous allez uniquement gérer les credentials comme des chaînes de caractères simples. Le chiffrement/hachage sera implémenté ultérieurement.

📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité SystemePartenaire dans la couche Domain
✅ Repository ISystemePartenaireRepository et son implémentation
✅ Base de données SQLite avec migrations appliquées
✅ Données de test pour les systèmes partenaires
✅ Exemples complets : CRUD Localisation, UniteMesure et User
Connaissances requises
Pattern Repository et Clean Architecture
Gestion des champs optionnels (nullable)
Mapperly pour le mapping
ASP.NET Core Web API
Blazor Server avec formulaires
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── SystemePartenaire/
   └── Services/
       └── Interfaces/
           └── ISystemePartenaireService.cs
       └── SystemePartenaireService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── SystemePartenaireController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── SystemePartenaire/
           └── Index.razor
           └── Create.razor
           └── Edit.razor
           └── Delete.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/SystemePartenaire/) :

SystemePartenaireDto : pour lecture et affichage
CreateSystemePartenaireDto : pour création
UpdateSystemePartenaireDto : pour modification
Service à créer (dans Application/Services/) :

Interfaces/ISystemePartenaireService.cs : interface du service
SystemePartenaireService.cs : implémentation avec logique métier
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

SystemePartenaireController : endpoints REST pour CRUD
GET /api/systemepartenaire : liste tous les systèmes partenaires
GET /api/systemepartenaire/{id} : détail d'un système partenaire
POST /api/systemepartenaire : créer un système partenaire
PUT /api/systemepartenaire/{id} : modifier un système partenaire
DELETE /api/systemepartenaire/{id} : supprimer un système partenaire
3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/SystemePartenaire/) :

Index.razor : liste des systèmes partenaires avec tableau
Create.razor : formulaire de création
Edit.razor : formulaire de modification
Delete.razor : confirmation de suppression
Navigation : Ajouter un lien "Systèmes Partenaires" dans le menu principal

🔍 Spécifications fonctionnelles
Entité SystemePartenaire - Rappel
public class SystemePartenaire
{
    public Guid Id { get; set; }
    public string Nom { get; set; }
    public string UrlBase { get; set; }

    // Pour APPELER le système partenaire
    public string? UsernameAppel { get; set; }
    public string? PasswordChiffre { get; set; }

    // Pour ÊTRE APPELÉ par le système partenaire
    public string? UsernameAcces { get; set; }
    public string? PasswordHashAcces { get; set; }
}
Contexte métier
Un SystemePartenaire représente un autre système IoT (développé par un autre groupe d'étudiants) avec lequel votre plateforme peut communiquer.

Deux modes de communication :

Mode "Appelant" : Votre système appelle l'API du partenaire

Vous avez besoin de : UsernameAppel + PasswordChiffre
Exemple : Récupérer la liste des sondes du partenaire
Mode "Appelé" : Le système partenaire appelle votre API

Le partenaire utilise : UsernameAcces + PasswordHashAcces
Exemple : Le partenaire récupère vos sondes
Un système peut être à la fois appelant ET appelé (les deux modes actifs simultanément).

Règles de validation
✅ Le Nom est obligatoire (max 100 caractères)
✅ L'UrlBase est obligatoire et doit être une URL valide (format https://)
✅ Les champs credentials sont optionnels (peuvent être null)
✅ Si UsernameAppel est renseigné, alors PasswordChiffre doit l'être aussi (et vice versa)
✅ Si UsernameAcces est renseigné, alors PasswordHashAcces doit l'être aussi (et vice versa)
✅ Pas de doublon : un système avec le même nom ne peut exister deux fois
Comportements attendus
Liste (Index) :

Affichage en tableau : Nom, UrlBase, Mode(s) actif(s), Actions
Colonne "Mode(s)" affiche :
"Appelant" si UsernameAppel est renseigné
"Appelé" si UsernameAcces est renseigné
"Appelant + Appelé" si les deux sont renseignés
"Aucun" si aucun credential n'est configuré
Liens vers Create, Edit, Delete pour chaque ligne
Création (Create) :

Formulaire avec 6 champs :
Nom (obligatoire)
UrlBase (obligatoire)
Section "Configuration Appelant" : UsernameAppel, PasswordChiffre (optionnels)
Section "Configuration Appelé" : UsernameAcces, PasswordHashAcces (optionnels)
Validation côté client et serveur
Message de succès après création
Redirection vers Index
Modification (Edit) :

Formulaire pré-rempli avec tous les champs
Possibilité de modifier tous les champs
Important : Les mots de passe ne sont PAS affichés (champs vides)
Si l'utilisateur laisse vide → conserver l'ancienne valeur
Si l'utilisateur saisit quelque chose → remplacer par la nouvelle valeur
Validation identique à Create
Message de succès après modification
Redirection vers Index
Suppression (Delete) :

Page de confirmation avec tous les détails du système partenaire
Les mots de passe ne sont PAS affichés (remplacés par "***")
Message d'avertissement clair
Après suppression : redirection vers Index
💡 Conseils d'implémentation
📖 Utilisez les exemples précédents comme référence
Inspirez-vous des CRUD Localisation, UniteMesure et User déjà implémentés :

Structure des DTOs :
Toutes les propriétés sont présentes dans les DTOs
Les champs credentials sont de type string? (nullable)
Logique du Service :
Validation de l'URL (format)
Validation de cohérence : si UsernameAppel renseigné → PasswordChiffre aussi
Validation de cohérence : si UsernameAcces renseigné → PasswordHashAcces aussi
Gestion des erreurs avec try/catch
Gestion des mots de passe en Edit :
Dans UpdateSystemePartenaireDto, les champs password peuvent être null
Dans le service Update :
Si le DTO contient un password null → ne pas modifier le password existant
Si le DTO contient un password non-null → remplacer par la nouvelle valeur
Controller :
Même structure que les controllers précédents
Codes retour HTTP appropriés
Pages Blazor :
Organiser le formulaire en deux sections visuelles : "Configuration Appelant" et "Configuration Appelé"
Type password pour les champs de mot de passe
En Edit : champs password vides par défaut (placeholder "Laisser vide pour conserver")
⚙️ Points d'attention spécifiques
Validation de l'URL :

Vérifier que l'URL commence par http:// ou https://
Utiliser Uri.TryCreate() pour valider le format
Gestion des champs optionnels :

Tous les champs credentials sont string? (nullable)
Ne pas forcer l'utilisateur à remplir ces champs
Mais si un username est renseigné, le password correspondant doit l'être aussi
Affichage sécurisé :

Dans Index : afficher uniquement si le mode est actif (pas les valeurs)
Dans Delete : masquer les mots de passe avec "***"
Dans Edit : champs vides (ne jamais afficher les mots de passe en clair)
Logique de mise à jour :

Si password vide en Edit → conserver l'ancienne valeur en base
Si password renseigné en Edit → remplacer par la nouvelle valeur
🎨 Structure des pages Blazor
Index.razor :

Tableau avec colonnes : Nom, URL, Mode(s), Actions
Colonne "Mode(s)" : badge avec texte dynamique selon credentials renseignés
Bouton "Créer un système partenaire"
Create.razor :

Section 1 : Informations générales (Nom, UrlBase)
Section 2 : Configuration Appelant (UsernameAppel, PasswordChiffre) - optionnel
Section 3 : Configuration Appelé (UsernameAcces, PasswordHashAcces) - optionnel
Validation avec messages d'erreur
Boutons : Enregistrer et Annuler
Edit.razor :

Même structure que Create
Champs password vides avec placeholder explicite
Validation identique à Create
Boutons : Enregistrer et Annuler
Delete.razor :

Affichage de tous les détails sauf les mots de passe (afficher "***" à la place)
Message : "Êtes-vous sûr de vouloir supprimer ce système partenaire ?"
Boutons : Confirmer la suppression et Annuler
✅ Critères de validation
Tests manuels à effectuer
Tester via l'API (avec Swagger ou Postman)
✅ GET /api/systemepartenaire retourne tous les systèmes
✅ GET /api/systemepartenaire/{id} retourne un système spécifique
✅ POST /api/systemepartenaire crée un nouveau système
✅ POST avec URL invalide retourne erreur 400
✅ POST avec UsernameAppel mais sans PasswordChiffre retourne erreur 400
✅ PUT /api/systemepartenaire/{id} modifie un système
✅ PUT avec password vide conserve l'ancien password
✅ DELETE /api/systemepartenaire/{id} supprime un système
Tester via l'interface Blazor
✅ Accéder à /systemepartenaire affiche la liste
✅ Créer un système en mode "Appelant" uniquement
✅ Créer un système en mode "Appelé" uniquement
✅ Créer un système en mode "Appelant + Appelé"
✅ Créer un système sans aucun credential
✅ Validation : UsernameAppel sans PasswordChiffre affiche erreur
✅ Modifier un système en laissant les passwords vides (conserve les anciens)
✅ Modifier un système en changeant un password
✅ Supprimer un système
✅ Les mots de passe ne sont jamais affichés en clair
Checklist de code
[ ] DTOs créés avec propriétés appropriées (nullable pour credentials)
[ ] Service implémenté avec toutes les méthodes CRUD
[ ] Validation URL (format)
[ ] Validation cohérence credentials (username ↔ password)
[ ] Logique Update : conservation des passwords si champs vides
[ ] Mapping Mapperly configuré
[ ] Controller REST avec tous les endpoints
[ ] Injection de dépendances correcte
[ ] Pages Blazor avec formulaires et validation
[ ] Organisation visuelle en sections (Appelant / Appelé)
[ ] Champs password de type "password"
[ ] Masquage des mots de passe dans Index et Delete
[ ] Navigation ajoutée au menu
[ ] Gestion des erreurs (try/catch)
[ ] Messages de succès/erreur dans Blazor