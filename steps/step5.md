Étape 5 : Gestion des Relevés
Completion requirements
🎯 Objectif
Implémenter la gestion complète des Relevés (mesures effectuées par les sondes) avec une logique métier de validation.

Cette étape est guidée car elle introduit :

Relation forte avec l'entité Sonde
Validation métier : cohérence entre la valeur du relevé et l'unité de mesure de la sonde
Distinction entre relevés manuels et automatiques
Affichage chronologique et graphiques
Logique de déclenchement d'alertes (préparation pour l'étape suivante)
📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité Releve dans la couche Domain
✅ Repository IReleveRepository et son implémentation
✅ Base de données SQLite avec migrations appliquées
✅ Données de test pour les relevés
✅ CRUD Sonde fonctionnel
✅ Exemples complets : CRUD précédents
Connaissances requises
Pattern Repository et Clean Architecture
Relations entre entités (Foreign Key)
Validation métier complexe
Gestion des dates et timestamps
Mapperly pour le mapping
ASP.NET Core Web API
Blazor Server avec formulaires et graphiques
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── Releve/
   └── Services/
       └── Interfaces/
           └── IReleveService.cs
       └── ReleveService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── ReleveController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── Releve/
           └── Index.razor
           └── BySonde.razor
           └── Create.razor
           └── Delete.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/Releve/) :

ReleveDto : pour lecture et affichage
CreateReleveDto : pour création
ReleveWithSondeDto : pour affichage avec détails de la sonde
Service à créer (dans Application/Services/) :

Interfaces/IReleveService.cs : interface du service
ReleveService.cs : implémentation avec logique métier et validation
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

ReleveController : endpoints REST pour CRUD

GET /api/releve : liste tous les relevés (paginé)
GET /api/releve/{id} : détail d'un relevé
GET /api/releve/sonde/{sondeId} : relevés d'une sonde spécifique
GET /api/releve/sonde/{sondeId}/recent?count=10 : N derniers relevés d'une sonde
POST /api/releve : créer un relevé
DELETE /api/releve/{id} : supprimer un relevé
Note : Pas de PUT (pas de modification de relevé une fois créé)

3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/Releve/) :

Index.razor : liste de tous les relevés (toutes sondes confondues)
BySonde.razor : relevés d'une sonde spécifique avec graphique
Create.razor : formulaire de création d'un relevé
Delete.razor : confirmation de suppression
Navigation : Ajouter un lien "Relevés" dans le menu principal

🔍 Spécifications fonctionnelles
Entité Releve - Rappel
public class Releve
{
    public Guid Id { get; set; }
    public Guid SondeId { get; set; }
    public Sonde Sonde { get; set; }
    public decimal Valeur { get; set; }
    public DateTime DateHeure { get; set; }
    public TypeReleve TypeReleve { get; set; }
}

public enum TypeReleve
{
    Manuel,
    Automatique
}
Règles de validation
Propriétés obligatoires :

✅ SondeId : obligatoire (Guid valide, sonde existante et active)
✅ Valeur : obligatoire (decimal)
✅ DateHeure : obligatoire, ne peut pas être dans le futur
✅ TypeReleve : obligatoire (enum)
Validation métier complexe :

✅ Sonde active : on ne peut créer un relevé que pour une sonde active (EstActif = true)
✅ Cohérence de la valeur :
Si la sonde a une ValeurMin définie : Valeur >= ValeurMin (sinon warning)
Si la sonde a une ValeurMax définie : Valeur <= ValeurMax (sinon warning)
Ces warnings n'empêchent PAS la création du relevé, mais doivent être signalés
✅ Cohérence de l'unité : La valeur doit être cohérente avec l'UniteMesure de la sonde (information pour affichage)
Règles de suppression :

✅ Un relevé peut être supprimé uniquement s'il n'a pas généré d'alerte
✅ Afficher un message d'erreur si tentative de suppression d'un relevé lié à une alerte
Comportements attendus
Liste globale (Index) :

Affichage en tableau : Sonde (nom), Type Relevé, Valeur + Unité, Date/Heure, Actions
Tri par défaut : par DateHeure décroissant (plus récent en premier)
Filtres :
Par TypeReleve (dropdown : Tous, Manuel, Automatique)
Par période (date de début, date de fin)
Pagination : 50 relevés par page
Lien vers la page BySonde pour voir tous les relevés de cette sonde
Bouton "Créer un relevé"
Relevés par sonde (BySonde) :

Paramètre de route : /releve/sonde/{sondeId}
Affichage du nom de la sonde en en-tête
Tableau des relevés : Type, Valeur + Unité, Date/Heure, Actions
Graphique : Courbe d'évolution de la valeur dans le temps (X = DateHeure, Y = Valeur)
Affichage des seuils (ValeurMin et ValeurMax de la sonde) en lignes horizontales sur le graphique
Bouton "Créer un relevé pour cette sonde"
Bouton "Retour à la liste des sondes"
Création (Create) :

Formulaire avec :
SondeId (select avec liste des sondes actives uniquement)
Valeur (input number avec décimales)
DateHeure (date + time picker, par défaut = maintenant)
TypeReleve (select : Manuel, Automatique)
Affichage conditionnel :
Quand une sonde est sélectionnée, afficher :
Le type de sonde
L'unité de mesure (symbole)
Les plages ValeurMin/ValeurMax si définies
Validation en temps réel :
Si Valeur < ValeurMin de la sonde → afficher warning (jaune) "Valeur inférieure au minimum attendu"
Si Valeur > ValeurMax de la sonde → afficher warning (jaune) "Valeur supérieure au maximum attendu"
Ces warnings n'empêchent PAS la soumission
Message de succès après création
Redirection vers BySonde (pour voir le nouveau relevé dans le contexte de la sonde)
Suppression (Delete) :

Page de confirmation avec tous les détails du relevé
Afficher le nom de la sonde
Vérification : si le relevé a généré une alerte, afficher erreur et bloquer la suppression
Après suppression : redirection vers Index
💡 Conseils d'implémentation - GUIDAGE
🔑 Validation métier dans le Service
Vérifications à effectuer dans Create :

Vérifier que la sonde existe :

Récupérer la sonde via le repository
Si null → retourner erreur 404
Vérifier que la sonde est active :

Si sonde.EstActif == false → retourner erreur 400 avec message explicite
Vérifier DateHeure :

Si DateHeure > DateTime.UtcNow → retourner erreur 400
Générer warnings (mais ne pas bloquer) :

Si sonde.ValeurMin != null et Valeur < sonde.ValeurMin → warning
Si sonde.ValeurMax != null et Valeur > sonde.ValeurMax → warning
Ces warnings peuvent être retournés dans un champ Warnings du résultat
Vérifications à effectuer dans Delete :

Vérifier que le relevé n'a pas d'alerte associée :
Utiliser le repository pour vérifier s'il existe une alerte avec ce ReleveId
Si oui → retourner erreur 400 avec message "Impossible de supprimer un relevé ayant généré une alerte"
🔗 Chargement des relations
Dans le Service :

Lors de GetById ou GetBySondeId, charger la relation Sonde avec Include
Cela permet d'avoir accès à Sonde.Nom, Sonde.UniteMesure, etc.
Dans les DTOs :

ReleveDto : contient SondeId uniquement
ReleveWithSondeDto : contient SondeId + SondeNom + UniteMesureSymbole + TypeSonde
Utiliser ce DTO pour l'affichage dans Index et BySonde
📊 Graphique d'évolution
Dans BySonde.razor :

Utiliser une bibliothèque de graphiques compatible Blazor (Chart.js, ApexCharts, BlazorExpress.ChartJS ou MudBlazor )
Type de graphique : Line Chart (courbe)
Axe X : DateHeure (format dd/MM HH:mm)
Axe Y : Valeur
Afficher les seuils :
Ligne horizontale rouge pour ValeurMax (si définie)
Ligne horizontale bleue pour ValeurMin (si définie)
Données pour le graphique :

Récupérer tous les relevés de la sonde (ou limiter aux 100 derniers pour performance)
Trier par DateHeure croissant (plus ancien → plus récent)
🎨 Comportement dynamique dans Create
Quand l'utilisateur sélectionne une sonde :

Charger les détails de la sonde (via API ou service)
Afficher :
Type : "Température"
Unité : "°C"
Plage : "Min: 15°C, Max: 30°C" (si définies)
Validation en temps réel de la valeur :

À chaque modification du champ Valeur (@onchange ou @oninput)
Comparer avec ValeurMin et ValeurMax de la sonde
Afficher les warnings dans un bloc dédié (alert-warning Bootstrap)
📋 Pagination dans Index
Logique de pagination :

Charger uniquement 50 relevés à la fois
Utiliser Skip() et Take() dans le repository
Afficher les boutons Précédent / Suivant
Afficher le numéro de page actuelle et le total de pages
✅ Critères de validation
Tests manuels à effectuer
1. Tester via l'API (avec Swagger ou Postman)

✅ GET /api/releve retourne les relevés paginés
✅ GET /api/releve/{id} retourne un relevé avec détails de la sonde
✅ GET /api/releve/sonde/{sondeId} retourne tous les relevés d'une sonde
✅ GET /api/releve/sonde/{sondeId}/recent?count=5 retourne les 5 derniers
✅ POST /api/releve crée un nouveau relevé
✅ POST avec SondeId d'une sonde inactive retourne erreur 400
✅ POST avec DateHeure dans le futur retourne erreur 400
✅ POST avec valeur hors plage retourne warnings (mais crée quand même)
✅ DELETE /api/releve/{id} supprime un relevé
✅ DELETE d'un relevé avec alerte retourne erreur 400
2. Tester via l'interface Blazor

✅ Accéder à /releve affiche la liste paginée
✅ Filtrer par TypeReleve fonctionne
✅ Filtrer par période fonctionne
✅ Pagination fonctionne
✅ Accéder à /releve/sonde/{id} affiche les relevés de cette sonde
✅ Le graphique s'affiche correctement avec les données
✅ Les seuils (ValeurMin/Max) sont visibles sur le graphique
✅ Créer un relevé pour une sonde active
✅ La liste des sondes dans Create ne contient que les sondes actives
✅ Sélectionner une sonde affiche ses détails (type, unité, plages)
✅ Saisir une valeur hors plage affiche les warnings
✅ Les warnings n'empêchent pas la création
✅ Créer un relevé avec DateHeure dans le futur affiche erreur
✅ Supprimer un relevé sans alerte
✅ Tentative de suppression d'un relevé avec alerte affiche erreur
Checklist de code
[ ] DTOs créés (ReleveDto, CreateReleveDto, ReleveWithSondeDto)
[ ] Service implémenté avec toutes les méthodes
[ ] Validation : sonde existante et active
[ ] Validation : DateHeure pas dans le futur
[ ] Génération de warnings pour valeurs hors plage
[ ] Vérification alerte associée avant suppression
[ ] Mapping Mapperly configuré
[ ] Controller REST avec tous les endpoints
[ ] Endpoint de pagination fonctionnel
[ ] Injection de dépendances correcte
[ ] Page Index avec filtres et pagination
[ ] Page BySonde avec graphique
[ ] Graphique avec affichage des seuils
[ ] Page Create avec sélection de sonde
[ ] Affichage dynamique des détails de la sonde
[ ] Validation en temps réel avec warnings
[ ] Page Delete avec vérification alerte
[ ] Navigation ajoutée au menu
[ ] Gestion des erreurs (try/catch)
[ ] Messages de succès/erreur/warning dans Blazor