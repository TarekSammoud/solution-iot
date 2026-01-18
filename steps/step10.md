Étape 10 : Dashboard temps réel
Completion requirements
🎯 Objectif
Créer un Dashboard de synthèse qui affiche en temps réel une vue d'ensemble de l'état de la plateforme IoT.

Cette étape est autonome car elle réutilise tous les services déjà créés. C'est un exercice de synthèse qui consolide vos connaissances.

Le dashboard doit offrir une vue globale permettant de surveiller rapidement :

Les alertes actives
Les derniers relevés des sondes
L'état des actionneurs
Des statistiques globales
📚 Prérequis
Éléments déjà en place dans le projet
✅ Tous les services précédents (Sonde, Releve, Alerte, Actionneur, etc.)
✅ Tous les DTOs nécessaires
✅ API REST complète
✅ Dashboard Alerte (Étape 7) comme référence
Connaissances requises
Tous les concepts vus dans les étapes précédentes
Blazor avec rafraîchissement automatique
Agrégation de données
Graphiques et visualisations
🏗️ Architecture
Vous allez travailler sur 2 couches :

📁 IotPlatform.Application
   └── DTOs/
       └── Dashboard/
           └── DashboardSummaryDto.cs
   └── Services/
       └── Interfaces/
           └── IDashboardService.cs
       └── DashboardService.cs

📁 IotPlatform.Api
   └── Controllers/
       └── DashboardController.cs

📁 IotPlatform.Web (Blazor)
   └── Pages/
       └── Dashboard/
           └── Index.razor
   └── Components/
       └── Dashboard/
           └── AlertesWidget.razor
           └── RelevesWidget.razor
           └── ActionneursWidget.razor
           └── StatistiquesWidget.razor
📝 Livrables attendus
1️⃣ Couche Application - DTOs et Service
DTOs à créer (dans Application/DTOs/Dashboard/) :

DashboardSummaryDto : agrégation de toutes les données du dashboard
StatistiquesDto : statistiques globales (nombres, moyennes)
Service à créer (dans Application/Services/) :

Interfaces/IDashboardService.cs : interface du service
DashboardService.cs : agrégation des données depuis les autres services
2️⃣ Couche API - Controller REST
Controller à créer (dans Api/Controllers/) :

DashboardController : endpoint unique

GET /api/dashboard/summary : retourne toutes les données du dashboard
3️⃣ Couche Presentation - Page et Composants Blazor
Page principale (dans Web/Pages/Dashboard/) :

Index.razor : page principale du dashboard avec layout en grille
Composants (dans Web/Components/Dashboard/) :

AlertesWidget.razor : widget des alertes actives
RelevesWidget.razor : widget des derniers relevés
ActionneursWidget.razor : widget de l'état des actionneurs
StatistiquesWidget.razor : widget des statistiques globales
Navigation : Faire du Dashboard la page d'accueil (route "/")

🔍 Spécifications fonctionnelles
Données à afficher
DashboardSummaryDto doit contenir :

Statistiques globales :

Nombre total de sondes (actives / inactives)
Nombre total d'actionneurs (actifs / inactifs)
Nombre d'alertes (actives / acquittées / résolues aujourd'hui)
Nombre de relevés enregistrés aujourd'hui
Alertes actives (maximum 10 les plus récentes) :

Sonde, Type seuil, Valeur, Date création
Derniers relevés (maximum 20 les plus récents) :

Sonde, Valeur + Unité, Date/Heure
État des actionneurs actifs (tous) :

Nom, Type, État actuel (ON/OFF + pourcentage si applicable)
Layout du Dashboard
Organisation en grille 2x2 :

┌─────────────────────────────────────────────────┐
│                   DASHBOARD IoT                 │
│            Dernière mise à jour: HH:MM:SS       │
└─────────────────────────────────────────────────┘

┌─────────────────────┬───────────────────────────┐
│  📊 STATISTIQUES    │   🚨 ALERTES ACTIVES     │
│                     │                           │
│  Sondes: 15 (12⚡3) │  • Sonde Temp Bureau      │
│  Actionneurs: 8 (6) │    Max dépassé: 32°C      │
│  Alertes: 3 actives │    il y a 5 minutes       │
│  Relevés: 156 auj.  │                           │
│                     │  • Sonde Humid Salle 1    │
│                     │    Min dépassé: 25%       │
│                     │    il y a 12 minutes      │
├─────────────────────┼───────────────────────────┤
│  📈 DERNIERS        │   💡 ACTIONNEURS         │
│     RELEVÉS         │                           │
│                     │  💡 Ampoule Bureau: ON    │
│  • Temp Bureau      │  💡 Ampoule Salle: OFF    │
│    23.5°C           │  💡 Variomètre: ON (75%)  │
│    il y a 2 min     │  ⚙️ Moteur 1: ON (50%)    │
│                     │                           │
│  • Humid Salle 1    │                           │
│    45%              │                           │
│    il y a 5 min     │                           │
└─────────────────────┴───────────────────────────┘

[Rafraîchir maintenant]
Comportements attendus
Au chargement :

Charger toutes les données via l'API
Afficher les 4 widgets
Démarrer un timer de rafraîchissement automatique (toutes les 30 secondes)
Rafraîchissement automatique :

Timer qui recharge les données toutes les 30 secondes
Afficher "Dernière mise à jour : [heure]"
Animation subtile pendant le rechargement
Bouton "Rafraîchir maintenant" :

Recharger immédiatement les données
Réinitialiser le timer
Liens vers les détails :

Cliquer sur une alerte → page Details de l'alerte
Cliquer sur un relevé → page BySonde des relevés
Cliquer sur un actionneur → page ControleEtat
Cliquer sur les stats → pages correspondantes (Index Sonde, Index Actionneur, etc.)
💡 Conseils d'implémentation
🔑 Service Dashboard
DashboardService doit agréger les données :

Le DashboardService injecte tous les autres services nécessaires :

IAlerteService
IReleveService
ISondeService
IActionneurService
IEtatActionneurService
Méthode GetSummary() :

1. Appeler AlerteService.GetStatistiques()
2. Appeler AlerteService.GetActive() et limiter à 10
3. Appeler ReleveService.GetRecents(20)
4. Appeler SondeService.GetAll() et compter actifs/inactifs
5. Appeler ActionneurService.GetAll() avec états
6. Appeler ReleveService.CountToday()
7. Agréger tout dans DashboardSummaryDto
8. Retourner
🎨 Organisation des composants Blazor
Index.razor (page principale) :

Layout en grille CSS (2 colonnes x 2 lignes)
Importer et utiliser les 4 widgets
Gérer le timer de rafraîchissement
Passer les données en paramètres aux widgets
Widgets (composants réutilisables) :

Chaque widget est un composant Blazor avec :

[Parameter] pour recevoir les données
Affichage spécifique selon le type de données
Liens vers les pages détaillées
⏱️ Timer de rafraîchissement
Dans Index.razor.cs (ou @code) :

private System.Threading.Timer? _timer;
private DateTime _lastUpdate;

protected override void OnInitialized()
{
    LoadData();
    _timer = new Timer(async _ => await RefreshData(), null, 30000, 30000);
}

private async Task RefreshData()
{
    await LoadData();
    _lastUpdate = DateTime.Now;
    await InvokeAsync(StateHasChanged);
}

public void Dispose()
{
    _timer?.Dispose();
}
📊 Statistiques
Calcul des statistiques :

Sondes actives : sondes.Count(s => s.EstActif)
Sondes inactives : sondes.Count(s => !s.EstActif)
Relevés aujourd'hui : compter les relevés avec DateHeure >= début de journée
Alertes résolues aujourd'hui : compter les alertes avec DateResolution aujourd'hui
🎨 Badges et icônes
Utiliser des icônes pour améliorer l'UX :

📊 pour Statistiques
🚨 pour Alertes
📈 pour Relevés
💡 pour Ampoules
⚙️ pour Moteurs
✅ pour état ON
❌ pour état OFF
Badges colorés :

Alertes actives : rouge (bg-danger)
Alertes acquittées : jaune (bg-warning)
Sondes actives : vert (bg-success)
Sondes inactives : gris (bg-secondary)
📱 Responsive
Adapter le layout pour mobile :

Sur petits écrans, passer d'une grille 2x2 à une colonne unique :

┌─────────────┐
│ Stats       │
├─────────────┤
│ Alertes     │
├─────────────┤
│ Relevés     │
├─────────────┤
│ Actionneurs │
└─────────────┘
Utiliser les media queries CSS ou les classes Bootstrap responsive.

✅ Critères de validation
Tests manuels à effectuer
1. Tester via l'API (avec Swagger ou Postman)

✅ GET /api/dashboard/summary retourne toutes les données
✅ Les statistiques sont correctes
✅ Les alertes actives sont limitées à 10
✅ Les relevés récents sont limités à 20
✅ Les états des actionneurs sont présents
2. Tester via l'interface Blazor

✅ Accéder à / (page d'accueil) affiche le dashboard
✅ Les 4 widgets sont affichés
✅ Les statistiques sont correctes
✅ Les alertes actives sont affichées avec détails
✅ Les derniers relevés sont affichés
✅ Les actionneurs actifs sont affichés avec leur état
✅ Le timer de rafraîchissement fonctionne (30s)
✅ L'heure de dernière mise à jour s'affiche
✅ Le bouton "Rafraîchir maintenant" fonctionne
✅ Les liens vers les détails fonctionnent
✅ Le layout est responsive (mobile et desktop)
3. Tester en conditions réelles

✅ Créer un relevé dépassant un seuil → l'alerte apparaît dans le widget
✅ Acquitter une alerte → elle disparaît du widget
✅ Créer un nouveau relevé → il apparaît dans le widget
✅ Changer l'état d'un actionneur → il se met à jour dans le widget
✅ Laisser le dashboard ouvert 1 minute → vérifier que les données se rafraîchissent
Checklist de code
[ ] DashboardSummaryDto créé avec toutes les propriétés
[ ] StatistiquesDto créé
[ ] DashboardService créé et injecte tous les services nécessaires
[ ] Méthode GetSummary implémentée avec agrégation
[ ] Statistiques calculées correctement
[ ] Mapping Mapperly configuré
[ ] Controller REST avec endpoint summary
[ ] Injection de dépendances correcte
[ ] Page Index.razor créée avec layout en grille
[ ] 4 widgets créés (Alertes, Relevés, Actionneurs, Statistiques)
[ ] Timer de rafraîchissement implémenté (30s)
[ ] Affichage de la dernière mise à jour
[ ] Bouton "Rafraîchir maintenant" fonctionnel
[ ] Liens vers les pages détaillées
[ ] Badges et icônes pour améliorer l'UX
[ ] Layout responsive (mobile friendly)
[ ] Dashboard défini comme page d'accueil
[ ] Gestion des erreurs (try/catch)
[ ] Dispose du timer (IDisposable)
