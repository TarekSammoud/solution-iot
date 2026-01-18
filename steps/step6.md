Étape 6 : Configuration des SeuilAlerte
Completion requirements
🎯 Objectif
Implémenter la gestion complète des Seuils d'Alerte pour les sondes, avec validation des règles métier.

Cette étape est guidée car elle introduit :

Validation métier complexe : un seul seuil actif de chaque type (Min/Max) par sonde
Relation forte avec l'entité Sonde
Gestion de l'activation/désactivation de seuils
Interface de configuration intuitive depuis la page de la sonde
📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité SeuilAlerte dans la couche Domain
✅ Repository ISeuilAlerteRepository et son implémentation
✅ Base de données SQLite avec migrations appliquées
✅ Données de test pour les seuils d'alerte
✅ CRUD Sonde fonctionnel
✅ Gestion des Relevés fonctionnelle
✅ Exemples complets : CRUD et features précédentes
Connaissances requises
Pattern Repository et Clean Architecture
Relations entre entités (Foreign Key)
Validation métier avec règles complexes
Gestion de l'état (actif/inactif)
Mapperly pour le mapping
ASP.NET Core Web API
Blazor Server avec formulaires
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── SeuilAlerte/
   └── Services/
       └── Interfaces/
           └── ISeuilAlerteService.cs
       └── SeuilAlerteService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── SeuilAlerteController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── SeuilAlerte/
           └── BySonde.razor
           └── Create.razor
           └── Edit.razor
           └── Delete.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/SeuilAlerte/) :

SeuilAlerteDto : pour lecture et affichage
CreateSeuilAlerteDto : pour création
UpdateSeuilAlerteDto : pour modification
Service à créer (dans Application/Services/) :

Interfaces/ISeuilAlerteService.cs : interface du service
SeuilAlerteService.cs : implémentation avec logique métier et validation
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

SeuilAlerteController : endpoints REST pour CRUD

GET /api/seuilalerte/sonde/{sondeId} : seuils d'une sonde spécifique
GET /api/seuilalerte/{id} : détail d'un seuil
POST /api/seuilalerte : créer un seuil
PUT /api/seuilalerte/{id} : modifier un seuil
PUT /api/seuilalerte/{id}/toggle : activer/désactiver un seuil
DELETE /api/seuilalerte/{id} : supprimer un seuil
3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/SeuilAlerte/) :

BySonde.razor : configuration des seuils pour une sonde
Create.razor : formulaire de création d'un seuil
Edit.razor : formulaire de modification d'un seuil
Delete.razor : confirmation de suppression
Intégration : Ajouter un onglet "Seuils d'alerte" dans la page Details de Sonde (ou bouton depuis BySonde de Releve)

🔍 Spécifications fonctionnelles
Entité SeuilAlerte - Rappel
public class SeuilAlerte
{
    public Guid Id { get; set; }
    public Guid SondeId { get; set; }
    public Sonde Sonde { get; set; }
    public TypeSeuil TypeSeuil { get; set; }
    public TypeAlerte TypeAlerte { get; set; }
    public decimal Valeur { get; set; }
    public bool EstActif { get; set; }
    public DateTime DateCreation { get; set; }
}

public enum TypeSeuil
{
    Minimum,
    Maximum
}

public enum TypeAlerte
{
    Alerte,
    Avertissement
}
Règles de validation
Propriétés obligatoires :

✅ SondeId : obligatoire (Guid valide, sonde existante)
✅ TypeSeuil : obligatoire (enum : Minimum ou Maximum)
✅ TypeAlerte : obligatoire (enum : Alerte ou Avertissement)
✅ Valeur : obligatoire (decimal)
✅ EstActif : booléen (par défaut true à la création)
✅ DateCreation : générée automatiquement (non modifiable)
Validation métier complexe :

✅ Unicité du seuil actif : Une sonde ne peut avoir qu'un seul seuil actif de chaque type

Si on crée/active un seuil Minimum alors qu'un autre Minimum, avec le même typeAlerte, est déjà actif → désactiver l'ancien automatiquement
Idem pour Maximum
Plusieurs seuils inactifs du même type peuvent coexister (historique)
✅ Cohérence des valeurs :

Si la sonde a un seuil avec TypeSeuil = Minimum et TypeAlerte = Alerte → la valeur du seuil avec avec TypeSeuil = Minimum et TypeAlerte = Avertissement doit être >= à la valeur du seuil avec TypeSeuil = Minimum et TypeAlerte = Alerte
Si la sonde a un seuil avec TypeSeuil = Maximum et TypeAlerte = Alerte → la valeur du seuil avec avec TypeSeuil = Maximum et TypeAlerte = Avertissement doit être <= à la valeur du seuil avec TypeSeuil = Maximum et TypeAlerte = Alerte
Si les une sonde a des seuils avec TypeSeuil = Minimum et TypeSeuil = Maximum les valeurs des seuils avec TypeSeuil = Minimum doivent être < aux valeurs des seuils avec TypeSeuil = Maximum
Règles de suppression :

✅ Un seuil peut être supprimé uniquement s'il n'a pas généré d'alertes actives
✅ Si des alertes résolues existent : afficher warning mais permettre la suppression
✅ Si des alertes actives existent : bloquer la suppression
Comportements attendus
Configuration par sonde (BySonde) :

Paramètre de route : /seuilalerte/sonde/{sondeId}
Affichage du nom de la sonde en en-tête
Deux sections distinctes :

Seuil Minimum :
Affichage du seuil actif (si existe) : Valeur, DateCreation, Statut (badge vert)
Bouton "Modifier" (si existe)
Bouton "Désactiver" (si actif)
Bouton "Créer un seuil minimum" (si aucun actif)
Liste des seuils inactifs (historique) avec bouton "Réactiver"
Seuil Maximum :
Même structure que Seuil Minimum
Bouton "Retour à la sonde"

Création (Create) :

Paramètre de route : /seuilalerte/create/{sondeId}/{typeSeuil}
Affichage du nom de la sonde et du type de seuil en en-tête
Formulaire avec :
SondeId (pré-rempli, en lecture seule)
TypeSeuil (pré-rempli, en lecture seule)
Valeur (input number avec décimales)
EstActif (checkbox, coché par défaut)
Affichage informatif :
Unité de mesure de la sonde
Plages ValeurMin/ValeurMax de la sonde (si définies)
Warning si un seuil actif du même type existe déjà : "Un seuil [type] est déjà actif. Il sera automatiquement désactivé."
Validation côté client et serveur
Message de succès après création
Redirection vers BySonde
Modification (Edit) :

Formulaire pré-rempli avec :
Valeur (modifiable)
EstActif (modifiable)
DateCreation (lecture seule)
Même affichage informatif que Create
Validation identique
Message de succès après modification
Redirection vers BySonde
Suppression (Delete) :

Page de confirmation avec tous les détails du seuil
Affichage du nombre d'alertes générées par ce seuil
Vérification :
Si alertes actives → bloquer avec message d'erreur
Si alertes résolues uniquement → warning mais permettre
Après suppression : redirection vers BySonde
Toggle Actif/Inactif :

Action rapide depuis BySonde
Bouton "Désactiver" sur un seuil actif → passe EstActif à false
Bouton "Réactiver" sur un seuil inactif → passe EstActif à true
Si un autre seuil du même type est actif → le désactiver automatiquement
Message de succès
Rafraîchissement de la page
💡 Conseils d'implémentation - GUIDAGE
🔑 Validation métier dans le Service
Logique de création/activation :

Dans Create et dans Update (quand on passe EstActif de false à true) :

Vérifier si un seuil actif du même type existe déjà :
Interroger le repository pour trouver un SeuilAlerte avec :
Même SondeId
Même TypeSeuil
EstActif = true
Id différent (pour Update)
Si un seuil actif existe :
Le désactiver automatiquement (EstActif = false)
Logger l'opération
Valider la cohérence des valeurs :
Récupérer la sonde avec ses ValeurMin/ValeurMax
Si TypeSeuil = Minimum et sonde.ValeurMin != null :
Vérifier que Valeur >= sonde.ValeurMin
Si TypeSeuil = Maximum et sonde.ValeurMax != null :
Vérifier que Valeur <= sonde.ValeurMax
Vérifier la cohérence entre Min et Max :
Récupérer le seuil actif de l'autre type (si existe)
Si les deux sont actifs, vérifier que SeuilMin.Valeur < SeuilMax.Valeur
Logique de suppression :

Compter les alertes actives liées à ce seuil :
Utiliser le repository d'Alerte pour compter les alertes avec :
SeuilAlerteId = id du seuil
Statut = Active ou Acquittee
Si alertes actives > 0 :
Retourner erreur 400 avec message "Impossible de supprimer un seuil ayant des alertes actives"
Sinon :
Permettre la suppression
🎨 Interface BySonde - Organisation visuelle
Structure de la page :

┌─────────────────────────────────────────────────┐
│ Configuration des seuils - [Nom de la sonde]    │
│ Unité: °C | Type: Température                   │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│ 🔵 Seuil MINIMUM                                │
├─────────────────────────────────────────────────┤
│ ✅ Actif: 15.0°C (créé le 15/11/2024)           │
│ [Modifier] [Désactiver]                         │
│                                                 │
│ Historique (inactifs):                          │
│ • 10.0°C (créé le 01/10/2024) [Réactiver]       │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│ 🔴 Seuil MAXIMUM                                │
├─────────────────────────────────────────────────┤
│ ✅ Actif: 30.0°C (créé le 15/11/2024)           │
│ [Modifier] [Désactiver]                         │
└─────────────────────────────────────────────────┘

[Retour à la sonde]
Conseils d'implémentation :

Charger tous les seuils de la sonde au chargement de la page
Filtrer côté client pour séparer actifs et inactifs
Utiliser des badges Bootstrap pour le statut
Couleurs : vert pour actif, gris pour inactif
🔄 Endpoint Toggle
Créer une méthode spécifique dans le service :

ToggleSeuilAlerte(Guid id)
Cette méthode :

Récupère le seuil
Inverse EstActif
Si passage de false à true → appliquer la logique de désactivation de l'ancien seuil actif
Sauvegarder
Dans le controller :

Endpoint : PUT /api/seuilalerte/{id}/toggle
Appeler la méthode ToggleSeuilAlerte du service
Retourner 200 OK
Dans Blazor :

Bouton avec événement @onclick qui appelle l'API
Rafraîchir les données après succès
📋 Validation en temps réel dans Create/Edit
Afficher les informations de la sonde :

Charger les détails de la sonde au chargement de la page
Afficher : Type, Unité, ValeurMin, ValeurMax
Vérifier si un seuil actif existe :

Appeler l'API pour récupérer les seuils de la sonde
Filtrer par TypeSeuil et EstActif = true
Si existe → afficher le warning
Validation de la valeur :

À la saisie de la valeur, vérifier les plages
Afficher erreur si hors plage de la sonde
✅ Critères de validation
Tests manuels à effectuer
1. Tester via l'API (avec Swagger ou Postman)

✅ GET /api/seuilalerte/sonde/{sondeId} retourne tous les seuils de la sonde
✅ GET /api/seuilalerte/{id} retourne un seuil spécifique
✅ POST /api/seuilalerte crée un nouveau seuil
✅ POST d'un seuil Minimum alors qu'un autre Minimum est actif → l'ancien est désactivé
✅ POST avec valeur hors plage de la sonde retourne erreur 400
✅ POST avec SeuilMin > SeuilMax actif retourne erreur 400
✅ PUT /api/seuilalerte/{id} modifie un seuil
✅ PUT /api/seuilalerte/{id}/toggle change le statut actif
✅ DELETE /api/seuilalerte/{id} supprime un seuil
✅ DELETE d'un seuil avec alertes actives retourne erreur 400
2. Tester via l'interface Blazor

✅ Accéder à /seuilalerte/sonde/{id} affiche la configuration
✅ Les deux sections (Minimum et Maximum) sont distinctes
✅ Créer un seuil Minimum pour une sonde
✅ Créer un second seuil Minimum actif → l'ancien est automatiquement désactivé
✅ L'ancien seuil apparaît dans l'historique (inactifs)
✅ Modifier un seuil existant
✅ Désactiver un seuil actif
✅ Réactiver un seuil inactif → l'ancien actif est désactivé
✅ Créer un seuil avec valeur hors plage affiche erreur
✅ Créer un SeuilMin > SeuilMax affiche erreur
✅ Supprimer un seuil sans alertes
✅ Tentative de suppression d'un seuil avec alertes actives affiche erreur
Checklist de code
[ ] DTOs créés (SeuilAlerteDto, CreateSeuilAlerteDto, UpdateSeuilAlerteDto)
[ ] Service implémenté avec toutes les méthodes
[ ] Logique de désactivation automatique de l'ancien seuil actif
[ ] Validation : valeur cohérente avec plages de la sonde
[ ] Validation : cohérence SeuilMin < SeuilMax
[ ] Vérification alertes actives avant suppression
[ ] Méthode ToggleSeuilAlerte implémentée
[ ] Mapping Mapperly configuré
[ ] Controller REST avec tous les endpoints
[ ] Endpoint toggle fonctionnel
[ ] Injection de dépendances correcte
[ ] Page BySonde avec deux sections distinctes
[ ] Affichage des seuils actifs et historique
[ ] Page Create avec validation en temps réel
[ ] Page Edit avec même logique que Create
[ ] Page Delete avec vérification alertes
[ ] Boutons Toggle (Désactiver/Réactiver) fonctionnels
[ ] Intégration avec page Sonde (lien ou onglet)
[ ] Gestion des erreurs (try/catch)
[ ] Messages de succès/erreur/warning dans Blazor
