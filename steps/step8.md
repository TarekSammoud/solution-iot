Étape 8 : CRUD Actionneur
Completion requirements
🎯 Objectif
Implémenter la gestion complète (Create, Read, Update, Delete) des Actionneurs en suivant le même pattern que les Sondes.

Cette étape est autonome car vous avez déjà implémenté le CRUD Sonde qui utilise également l'héritage depuis Device. Vous pouvez donc réutiliser la même approche.

Particularité : Les actionneurs ont un état actuel qui dépend de leur type (AmpouleSimple, AmpouleVariometre, Moteur).

📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité abstraite Device dans la couche Domain
✅ Entité Actionneur héritant de Device dans la couche Domain
✅ Entité EtatActionneur dans la couche Domain
✅ Repositories IActionneurRepository et IEtatActionneurRepository avec implémentations
✅ Base de données SQLite avec migrations appliquées (TPH configuré)
✅ Données de test pour les actionneurs
✅ CRUD Sonde fonctionnel (référence pour l'héritage Device)
✅ CRUD Localisation fonctionnel
Connaissances requises
Pattern Repository et Clean Architecture
Héritage en C# et Entity Framework Core (TPH)
Relations entre entités (1-1 pour l'état)
Enums avec comportement conditionnel
Mapperly pour le mapping
ASP.NET Core Web API
Blazor Server avec formulaires
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── Actionneur/
       └── EtatActionneur/
   └── Services/
       └── Interfaces/
           └── IActionneurService.cs
           └── IEtatActionneurService.cs
       └── ActionneurService.cs
       └── EtatActionneurService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── ActionneurController.cs
       └── EtatActionneurController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── Actionneur/
           └── Index.razor
           └── Create.razor
           └── Edit.razor
           └── Delete.razor
           └── Details.razor
           └── ControleEtat.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Services
DTOs Actionneur à créer (dans Application/DTOs/Actionneur/) :

ActionneurDto : pour lecture et affichage
CreateActionneurDto : pour création
UpdateActionneurDto : pour modification
DTOs EtatActionneur à créer (dans Application/DTOs/EtatActionneur/) :

EtatActionneurDto : pour lecture et affichage
UpdateEtatActionneurDto : pour modification de l'état
Services à créer (dans Application/Services/) :

Interfaces/IActionneurService.cs : interface du service Actionneur
ActionneurService.cs : implémentation avec logique métier
Interfaces/IEtatActionneurService.cs : interface du service EtatActionneur
EtatActionneurService.cs : implémentation pour gérer l'état
2️⃣ Couche API - Controllers REST
Controllers à créer (dans Api/Controllers/) :

ActionneurController : endpoints REST pour CRUD Actionneur

GET /api/actionneur : liste tous les actionneurs
GET /api/actionneur/{id} : détail d'un actionneur
GET /api/actionneur/localisation/{localisationId} : actionneurs par localisation
GET /api/actionneur/type/{typeActionneur} : actionneurs par type
POST /api/actionneur : créer un actionneur
PUT /api/actionneur/{id} : modifier un actionneur
DELETE /api/actionneur/{id} : supprimer un actionneur
EtatActionneurController : endpoints pour gérer l'état

GET /api/etatactionneur/actionneur/{actionneurId} : état actuel d'un actionneur
PUT /api/etatactionneur/actionneur/{actionneurId} : mettre à jour l'état
3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/Actionneur/) :

Index.razor : liste des actionneurs avec tableau et filtres
Create.razor : formulaire de création
Edit.razor : formulaire de modification
Delete.razor : confirmation de suppression
Details.razor : vue détaillée d'un actionneur avec son état
ControleEtat.razor : interface de contrôle de l'état de l'actionneur
Navigation : Ajouter un lien "Actionneurs" dans le menu principal

🔍 Spécifications fonctionnelles
Entités - Rappel
Device (classe abstraite) :

Voir Étape 4 (CRUD Sonde) pour les propriétés héritées

Actionneur (hérite de Device) :

public class Actionneur : Device
{
    public TypeActionneur TypeActionneur { get; set; }
}

public enum TypeActionneur
{
    AmpouleSimple,
    AmpouleVariometre,
    Moteur
}
EtatActionneur :

public class EtatActionneur
{
    public Guid Id { get; set; }
    public Guid ActionneurId { get; set; }
    public Actionneur Actionneur { get; set; }
    public bool? EstActif { get; set; }
    public int? Pourcentage { get; set; }
    public DateTime DerniereModification { get; set; }
}
Règles de validation
Propriétés héritées de Device :

Voir Étape 4 (CRUD Sonde) - mêmes règles

Propriétés spécifiques à Actionneur :

✅ TypeActionneur : obligatoire (enum)
Règles pour EtatActionneur :

✅ EstActif : selon le type d'actionneur

AmpouleSimple : obligatoire (true/false)
AmpouleVariometre : obligatoire (true/false)
Moteur : obligatoire (true/false)
✅ Pourcentage : selon le type d'actionneur

AmpouleSimple : doit être null
AmpouleVariometre : obligatoire si EstActif = true, entre 0 et 100
Moteur : obligatoire si EstActif = true, entre 0 et 100
✅ DerniereModification : générée automatiquement à chaque mise à jour

✅ Relation 1-1 : Un actionneur a exactement un état actuel (pas d'historique)

Comportements attendus
Liste (Index) :

Affichage en tableau : Nom, Type, Localisation, État actuel, Statut (Actif/Inactif), DateInstallation, Actions
Colonne "État actuel" affiche selon le type :
AmpouleSimple : "ON" ou "OFF"
AmpouleVariometre : "ON (X%)" ou "OFF"
Moteur : "ON (X%)" ou "OFF"
Filtres multiples :
Par TypeActionneur (dropdown : Tous, AmpouleSimple, AmpouleVariometre, Moteur)
Par Localisation (dropdown : Toutes, puis liste des localisations)
Par Statut Device (checkbox : Afficher uniquement les actifs)
Tri par défaut : par Nom
Badge coloré pour le statut Device (vert = Actif, gris = Inactif)
Liens vers Details, Edit, Delete, Contrôle pour chaque ligne
Bouton "Créer un actionneur"
Création (Create) :

Formulaire organisé en sections (similaire à Sonde) :
Informations générales : Nom, TypeActionneur, EstActif (Device)
Localisation : LocalisationId (select)
Communication : CanalCommunication (select), UrlDevice (conditionnel), CredentialsDevice (optionnel)
Installation : DateInstallation (date picker)
Comportement dynamique :
Quand CanalCommunication = HttpPush → UrlDevice optionnel (grisé)
Quand CanalCommunication ≠ HttpPush → UrlDevice obligatoire
DateCreation générée automatiquement (non affichée)
Création automatique de l'état : Lors de la création de l'actionneur, créer automatiquement un EtatActionneur avec :
EstActif = false
Pourcentage = null (ou 0 pour Variometre/Moteur)
DerniereModification = maintenant
Validation côté client et serveur
Message de succès après création
Redirection vers Index
Modification (Edit) :

Formulaire pré-rempli avec la même structure que Create
DateCreation affichée en lecture seule
Note : La modification ne change PAS l'état de l'actionneur (utiliser ControleEtat pour ça)
Validation identique à Create
Message de succès après modification
Redirection vers Index
Détails (Details) :

Affichage de toutes les informations de l'actionneur
Organisation en sections (comme le formulaire)
Affichage du nom de la Localisation
Section "État actuel" :
Affichage selon le type avec icône visuelle
Dernière modification
Boutons : Modifier, Contrôler l'état, Supprimer, Retour à la liste
Contrôle de l'état (ControleEtat) :

Route : /actionneur/{id}/controle
Affichage du nom et type de l'actionneur en en-tête
Formulaire adapté au type :

AmpouleSimple :
Toggle switch : ON / OFF
Bouton "Appliquer"
AmpouleVariometre :
Toggle switch : ON / OFF
Si ON : Slider pour le pourcentage (0-100%)
Bouton "Appliquer"
Moteur :
Toggle switch : ON / OFF
Si ON : Slider pour la vitesse (0-100%)
Bouton "Appliquer"
Validation : si EstActif = true, Pourcentage doit être défini pour Variometre et Moteur

Mise à jour de DerniereModification automatique
Message de succès après application
Retour vers Details
Suppression (Delete) :

Page de confirmation avec tous les détails de l'actionneur
Message d'avertissement
Après suppression : suppression en cascade de l'EtatActionneur
Redirection vers Index
💡 Conseils d'implémentation
📖 Utilisez l'exemple Sonde comme référence
L'actionneur hérite de Device exactement comme Sonde :

Structure des DTOs similaire (toutes les propriétés Device + Actionneur)
Validation UrlDevice conditionnelle identique
Gestion TPH identique dans EF Core
Pattern de service similaire
🔗 Gestion de l'état
Création automatique de l'état :

Dans ActionneurService.Create() :

Créer et sauvegarder l'Actionneur
Créer un EtatActionneur avec :
ActionneurId = actionneur.Id
EstActif = false
Pourcentage = null (ou 0 selon le type)
DerniereModification = DateTime.UtcNow
Sauvegarder l'EtatActionneur
Mise à jour de l'état :

Dans EtatActionneurService.UpdateEtat(actionneurId, dto) :

Récupérer l'actionneur et son état actuel
Valider selon le type :
AmpouleSimple : Pourcentage doit être null
Variometre/Moteur : Si EstActif = true, Pourcentage doit être entre 0 et 100
Mettre à jour EstActif et Pourcentage
Mettre à jour DerniereModification = DateTime.UtcNow
Sauvegarder
🎨 Interface ControleEtat
Affichage conditionnel selon le type :

Utiliser une propriété computed dans le code-behind pour déterminer quel formulaire afficher.

Toggle switch :

Utiliser un composant Blazor (InputCheckbox stylisé ou composant tiers)

Slider pour le pourcentage :

Utiliser <input type="range" min="0" max="100" />

Afficher la valeur à côté du slider

Désactivation conditionnelle :

Si EstActif = false, griser/désactiver le slider

📋 Affichage de l'état dans Index
Format de l'état actuel :

Créer une méthode helper pour formater l'affichage :

FormatEtat(actionneur, etat):
  Si TypeActionneur = AmpouleSimple:
    return etat.EstActif ? "ON" : "OFF"
  Si TypeActionneur = AmpouleVariometre:
    return etat.EstActif ? $"ON ({etat.Pourcentage}%)" : "OFF"
  Si TypeActionneur = Moteur:
    return etat.EstActif ? $"ON ({etat.Pourcentage}%)" : "OFF"
Icônes visuelles :

Ajouter des icônes pour améliorer l'UX :

💡 pour Ampoule
⚙️ pour Moteur
✅ vert si ON
❌ gris si OFF
✅ Critères de validation
Tests manuels à effectuer
1. Tester via l'API (avec Swagger ou Postman)

✅ GET /api/actionneur retourne tous les actionneurs
✅ GET /api/actionneur/{id} retourne un actionneur avec son état
✅ GET /api/actionneur/localisation/{id} retourne les actionneurs d'une localisation
✅ GET /api/actionneur/type/AmpouleSimple retourne uniquement les ampoules simples
✅ POST /api/actionneur crée un nouvel actionneur et son état initial
✅ POST avec UrlDevice manquante (si CanalCommunication ≠ HttpPush) retourne erreur 400
✅ PUT /api/actionneur/{id} modifie un actionneur
✅ DELETE /api/actionneur/{id} supprime un actionneur et son état
✅ GET /api/etatactionneur/actionneur/{id} retourne l'état actuel
✅ PUT /api/etatactionneur/actionneur/{id} met à jour l'état
✅ PUT état avec Pourcentage pour AmpouleSimple retourne erreur 400
✅ PUT état ON sans Pourcentage pour Variometre retourne erreur 400
2. Tester via l'interface Blazor

✅ Accéder à /actionneur affiche la liste
✅ Filtrer par TypeActionneur fonctionne
✅ Filtrer par Localisation fonctionne
✅ Filtrer par statut actif/inactif fonctionne
✅ L'état actuel est affiché correctement pour chaque type
✅ Créer une AmpouleSimple
✅ Créer une AmpouleVariometre
✅ Créer un Moteur
✅ Un état est automatiquement créé (OFF) après création
✅ Modifier un actionneur existant
✅ Voir les détails d'un actionneur avec son état
✅ Contrôler l'état d'une AmpouleSimple (ON/OFF)
✅ Contrôler l'état d'une AmpouleVariometre (ON/OFF + pourcentage)
✅ Contrôler l'état d'un Moteur (ON/OFF + vitesse)
✅ Le slider est grisé quand EstActif = false
✅ Tenter de mettre ON sans pourcentage affiche erreur
✅ Supprimer un actionneur
✅ DateCreation non modifiable en Edit
Checklist de code
[ ] DTOs Actionneur créés avec toutes les propriétés (Device + Actionneur)
[ ] DTOs EtatActionneur créés
[ ] ActionneurService implémenté avec toutes les méthodes CRUD
[ ] Création automatique de l'état dans Create
[ ] EtatActionneurService implémenté
[ ] Validation état selon TypeActionneur
[ ] Mise à jour automatique de DerniereModification
[ ] Validation conditionnelle UrlDevice selon CanalCommunication
[ ] DateCreation générée automatiquement dans Create
[ ] EstActif (Device) = true par défaut dans Create
[ ] Mapping Mapperly configuré
[ ] Controllers REST avec tous les endpoints
[ ] Injection de dépendances correcte
[ ] Pages Blazor avec formulaires et validation
[ ] Comportement conditionnel UrlDevice dans Create/Edit
[ ] Filtres multiples dans Index
[ ] Affichage formaté de l'état dans Index
[ ] Page Details avec section état actuel
[ ] Page ControleEtat avec formulaire adapté au type
[ ] Toggle switch et slider fonctionnels
[ ] Validation côté client dans ControleEtat
[ ] Navigation ajoutée au menu
[ ] Gestion des erreurs (try/catch)
[ ] Messages de succès/erreur dans Blazor