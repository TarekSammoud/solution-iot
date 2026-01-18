Étape 7 : Système d'Alertes intelligent
Completion requirements
🎯 Objectif
Implémenter le système d'alertes automatique qui se déclenche lors de la création de relevés dépassant les seuils configurés.

Cette étape est guidée car elle introduit une logique métier complexe :

Déclenchement automatique d'alertes lors de la création de relevés
Gestion du cycle de vie des alertes (Active → Acquittée → Résolue)
Résolution automatique quand un nouveau relevé repasse dans les limites
Prévention de doublons d'alertes
Dashboard de monitoring
📚 Prérequis
Éléments déjà en place dans le projet
✅ Entité Alerte dans la couche Domain
✅ Repository IAlerteRepository et son implémentation
✅ Base de données SQLite avec migrations appliquées
✅ Gestion des Relevés fonctionnelle
✅ Configuration des SeuilAlerte fonctionnelle
✅ Exemples complets : CRUD et features précédentes
Connaissances requises
Pattern Repository et Clean Architecture
Logique métier complexe avec états
Relations entre entités multiples
Gestion des dates et timestamps
Mapperly pour le mapping
ASP.NET Core Web API
Blazor Server avec tableaux dynamiques
🏗️ Architecture
Vous allez travailler sur 3 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── Alerte/
   └── Services/
       └── Interfaces/
           └── IAlerteService.cs
       └── ReleveService.cs (modification)
       └── AlerteService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── AlerteController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── Alerte/
           └── Dashboard.razor
           └── BySonde.razor
           └── Details.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/Alerte/) :

AlerteDto : pour lecture et affichage
AlerteDetailsDto : pour affichage détaillé avec toutes les relations
AcquitterAlerteDto : pour acquitter une alerte (peut contenir un commentaire)
Service à créer/modifier (dans Application/Services/) :

Interfaces/IAlerteService.cs : interface du service
AlerteService.cs : implémentation avec logique métier
Modification de ReleveService.cs : ajouter la logique de vérification des seuils et création d'alertes
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

AlerteController : endpoints REST

GET /api/alerte : liste toutes les alertes (avec filtres)
GET /api/alerte/{id} : détail d'une alerte
GET /api/alerte/sonde/{sondeId} : alertes d'une sonde spécifique
GET /api/alerte/active : uniquement les alertes actives
PUT /api/alerte/{id}/acquitter : acquitter une alerte
PUT /api/alerte/{id}/resoudre : résoudre manuellement une alerte
3️⃣ Couche Presentation - Pages Blazor
Pages Blazor à créer (dans Web/Pages/Alerte/) :

Dashboard.razor : vue d'ensemble des alertes avec statistiques
BySonde.razor : historique des alertes pour une sonde
Details.razor : détails complets d'une alerte
Navigation : Ajouter un lien "Alertes" dans le menu principal (avec badge du nombre d'alertes actives)

🔍 Spécifications fonctionnelles
Entité Alerte - Rappel
public class Alerte
{
    public Guid Id { get; set; }
    public Guid SondeId { get; set; }
    public Sonde Sonde { get; set; }
    public Guid SeuilAlerteId { get; set; }
    public SeuilAlerte SeuilAlerte { get; set; }
    public Guid ReleveId { get; set; }
    public Releve Releve { get; set; }
    public TypeSeuil TypeSeuil { get; set; }
    public StatutAlerte Statut { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DateAcquittement { get; set; }
    public DateTime? DateResolution { get; set; }
    public string? Message { get; set; }
}

public enum StatutAlerte
{
    Active,
    Acquittee,
    Resolue
}
Règles métier complexes
Création automatique d'alertes lors d'un relevé :

Quand un relevé est créé, vérifier si sa valeur dépasse un seuil actif :

Récupérer tous les seuils actifs de la sonde

Pour chaque seuil actif :

Si TypeSeuil = Minimum et Valeur < Seuil.Valeur → dépassement
Si TypeSeuil = Maximum et Valeur > Seuil.Valeur → dépassement
Si dépassement détecté :
Vérifier qu'il n'existe pas déjà une alerte ACTIVE pour ce même seuil et cette même sonde
Si aucune alerte active → créer une nouvelle alerte avec :
Statut = Active
Message auto-généré : "Seuil [Min/Max] dépassé : valeur mesurée [X] [unité], seuil configuré [Y] [unité]"
DateCreation = maintenant
Important : Une alerte Min et une alerte Max peuvent coexister simultanément pour la même sonde
Résolution automatique d'alertes :

Quand un relevé est créé, vérifier s'il résout des alertes actives :

Récupérer toutes les alertes ACTIVES de la sonde

Pour chaque alerte active :

Récupérer le seuil associé
Si TypeSeuil = Minimum et Valeur >= Seuil.Valeur → alerte résolue
Si TypeSeuil = Maximum et Valeur <= Seuil.Valeur → alerte résolue
Si alerte résolue :
Passer Statut = Resolue
DateResolution = maintenant
Message complété : ajouter " - Résolu automatiquement par relevé du [date]"
Acquittement manuel :

Un utilisateur peut acquitter une alerte Active
Passe le Statut de Active à Acquittee
DateAcquittement = maintenant
L'alerte peut toujours être résolue automatiquement par la suite
Résolution manuelle :

Un utilisateur peut résoudre manuellement une alerte (Active ou Acquittee)
Passe le Statut à Resolue
DateResolution = maintenant
Utile si l'alerte est une fausse alerte ou si le problème est résolu sans nouveau relevé
Comportements attendus
Dashboard (vue d'ensemble) :

Route : /alerte/dashboard
Statistiques en haut :
Nombre d'alertes actives (badge rouge)
Nombre d'alertes acquittées (badge jaune)
Nombre d'alertes résolues aujourd'hui (badge vert)
Tableau des alertes actives :
Colonnes : Sonde, Type Seuil, Valeur mesurée, Seuil, Date création, Actions
Tri par défaut : plus récentes en premier
Actions : Acquitter, Résoudre, Détails
Graphique (optionnel) : Évolution du nombre d'alertes sur les 7 derniers jours
Bouton "Voir l'historique complet"
Historique par sonde (BySonde) :

Route : /alerte/sonde/{sondeId}
Affichage du nom de la sonde en en-tête
Filtres :
Par Statut (dropdown : Toutes, Actives, Acquittées, Résolues)
Par Type Seuil (dropdown : Tous, Minimum, Maximum)
Tableau : Type Seuil, Valeur, Statut, Date création, Date résolution, Actions
Liens vers Détails
Bouton "Retour au dashboard"
Détails d'une alerte (Details) :

Route : /alerte/{id}
Affichage complet de toutes les informations :
Informations de la sonde (nom, type, localisation)
Informations du seuil (type, valeur configurée)
Informations du relevé déclencheur (valeur mesurée, date/heure, type)
Statut actuel avec badge coloré
Timeline :
Création : [date]
Acquittement : [date] (si applicable)
Résolution : [date] (si applicable)
Message de l'alerte
Actions conditionnelles :
Si Active : [Acquitter] [Résoudre]
Si Acquittée : [Résoudre]
Si Résolue : aucune action
Bouton "Retour au dashboard"
💡 Conseils d'implémentation - GUIDAGE
🔑 Modification du ReleveService
Ajouter une méthode privée dans ReleveService :

private async Task VerifierEtGererAlertes(Guid releveId)
Cette méthode sera appelée après la création d'un relevé.

Logique de VerifierEtGererAlertes :

Récupérer le relevé avec toutes ses relations :
Releve avec Sonde, UniteMesure
Récupérer tous les seuils actifs de la sonde :
Filtrer par SondeId et EstActif = true
Pour chaque seuil actif, vérifier dépassement :
Logique décrite dans les règles métier
Si dépassement → créer alerte :
Vérifier absence de doublon (alerte active pour ce seuil)
Créer et sauvegarder l'alerte
Récupérer toutes les alertes actives de la sonde :
Statut = Active
Pour chaque alerte active, vérifier résolution :
Logique décrite dans les règles métier
Si résolution → mettre à jour l'alerte :
Passer à Resolue, définir DateResolution
Dans la méthode Create du ReleveService :

Après avoir créé et sauvegardé le relevé :

await VerifierEtGererAlertes(releve.Id);
🎨 AlerteService - Méthodes principales
Méthodes à implémenter :

GetAll(filters) : liste avec filtres (statut, sondeId, dates)

GetById(id) : récupérer avec toutes les relations (Sonde, SeuilAlerte, Releve)

GetBySondeId(sondeId, filters) : alertes d'une sonde avec filtres

GetActive() : uniquement les alertes avec Statut = Active

AcquitterAlerte(id) :

Vérifier que Statut = Active
Passer à Acquittee
Définir DateAcquittement
ResoudreAlerte(id) :
Vérifier que Statut = Active ou Acquittee
Passer à Resolue
Définir DateResolution
GetStatistiques() :
Compter alertes par statut
Utile pour le dashboard
📊 Dashboard - Statistiques et affichage
Charger les statistiques au démarrage :

Appeler l'API pour récupérer les statistiques
Afficher dans des cartes (cards) en haut de page
Charger uniquement les alertes actives :

Appeler l'API /api/alerte/active
Afficher dans un tableau
Actions rapides :

Bouton "Acquitter" : appeler l'API, rafraîchir les données
Bouton "Résoudre" : idem
Bouton "Détails" : naviguer vers la page Details
Rafraîchissement automatique (optionnel) :

Timer qui rafraîchit les données toutes les 30 secondes
Afficher un indicateur "Dernière mise à jour : [heure]"
🎨 Badge dans le menu
Afficher le nombre d'alertes actives :

Dans le composant de navigation (NavMenu ou MainLayout) :

Appeler l'API pour compter les alertes actives
Afficher un badge rouge à côté du lien "Alertes"
Rafraîchir régulièrement (toutes les minutes)
🔍 Génération du message d'alerte
Message auto-généré lors de la création :

Format suggéré :

"Seuil [Minimum/Maximum] dépassé : valeur mesurée [X] [unité], seuil configuré [Y] [unité]"
Exemple :

"Seuil Maximum dépassé : valeur mesurée 35.2°C, seuil configuré 30.0°C"
Complément lors de la résolution automatique :

" - Résolu automatiquement par relevé du 15/11/2024 à 14:30"
🎨 Badges de statut
Utiliser des badges Bootstrap colorés :

Active : badge rouge (bg-danger)
Acquittée : badge jaune (bg-warning)
Résolue : badge vert (bg-success)
📋 Filtrage dans BySonde
Implémenter le filtrage côté client :

Charger toutes les alertes de la sonde au démarrage
Appliquer les filtres (Statut, TypeSeuil) sur la liste
Utiliser une propriété computed pour alertesFiltrees
✅ Critères de validation
Tests manuels à effectuer
1. Tester la création automatique d'alertes

✅ Créer un relevé dépassant le seuil Maximum → alerte créée automatiquement
✅ Créer un relevé dépassant le seuil Minimum → alerte créée automatiquement
✅ Créer un relevé dépassant les deux seuils → deux alertes créées
✅ Créer un second relevé dépassant le même seuil → pas de doublon d'alerte
✅ Le message de l'alerte est correctement généré avec les valeurs
2. Tester la résolution automatique d'alertes

✅ Créer un relevé dans les limites alors qu'une alerte Max est active → alerte résolue automatiquement
✅ Créer un relevé dans les limites alors qu'une alerte Min est active → alerte résolue automatiquement
✅ La DateResolution est définie
✅ Le message est complété avec la date de résolution
3. Tester l'acquittement manuel

✅ Acquitter une alerte Active depuis le dashboard
✅ Le statut passe à Acquittée
✅ La DateAcquittement est définie
✅ L'alerte peut encore être résolue après acquittement
4. Tester la résolution manuelle

✅ Résoudre manuellement une alerte Active
✅ Résoudre manuellement une alerte Acquittée
✅ Le statut passe à Résolue
✅ La DateResolution est définie
5. Tester le dashboard

✅ Les statistiques sont correctes
✅ Le tableau affiche uniquement les alertes actives
✅ Les actions (Acquitter, Résoudre) fonctionnent
✅ Le rafraîchissement met à jour les données
6. Tester BySonde

✅ Affichage de toutes les alertes de la sonde
✅ Filtrage par Statut fonctionne
✅ Filtrage par TypeSeuil fonctionne
7. Tester Details

✅ Toutes les informations sont affichées
✅ La timeline est correcte
✅ Les actions conditionnelles sont disponibles
8. Tester le badge dans le menu

✅ Le badge affiche le nombre d'alertes actives
✅ Le nombre se met à jour après création/résolution d'alerte
Checklist de code
[ ] DTOs créés (AlerteDto, AlerteDetailsDto, AcquitterAlerteDto)
[ ] AlerteService implémenté avec toutes les méthodes
[ ] Méthode VerifierEtGererAlertes dans ReleveService
[ ] Logique de création d'alerte sans doublon
[ ] Logique de résolution automatique
[ ] Génération du message auto
[ ] Méthode AcquitterAlerte implémentée
[ ] Méthode ResoudreAlerte implémentée
[ ] Méthode GetStatistiques implémentée
[ ] Mapping Mapperly configuré
[ ] Controller REST avec tous les endpoints
[ ] Injection de dépendances correcte
[ ] Page Dashboard avec statistiques
[ ] Tableau des alertes actives dans Dashboard
[ ] Actions rapides (Acquitter, Résoudre) fonctionnelles
[ ] Page BySonde avec filtres
[ ] Page Details avec timeline
[ ] Actions conditionnelles dans Details
[ ] Badge dans le menu avec nombre d'alertes actives
[ ] Badges colorés pour les statuts
[ ] Navigation ajoutée au menu
[ ] Gestion des erreurs (try/catch)
[ ] Messages de succès/erreur dans Blazor