# Rapport Personnel de Projet : Solution IoT ESEO
**Auteur : Tarek SAMMMOUD**

---

## 1. Description Technique de l'Implémentation

Au sein du groupe, ma responsabilité principale a porté sur la conception et l'implémentation du **Système d'Alertes et de Monitoring**. Cette fonctionnalité constitue le "cerveau" réactif de la plateforme, transformant les données de capteurs en événements critiques gérables par les utilisateurs.

### 🚨 Architecture du Système d'Alertes
Le système repose sur un mécanisme de surveillance en temps réel intégré au flux de données. J'ai structuré cette fonctionnalité autour de trois piliers :

*   **Détection Automatique** : Surveillance proactive de chaque nouveau relevé par rapport aux seuils configurés.
*   **Gestion du Cycle de Vie** : Passage d'une alerte à travers différents états (`Active`, `Acquittée`, `Résolue`).
*   **Monitoring et Statistiques** : Agrégation des données d'alertes pour le dashboard via le [AlerteRepository.cs](file:///c:/Users/Oussama%20JERIDI/Documents/GitHub/ESEO/solution-iot/src/Infrastructure/Repositories/AlerteRepository.cs).

### 🛠️ Focus sur des Implémentations Complexes

#### A. Déclenchement Intelligent et Prévention des Doublons
L'un des défis était d'éviter la création d'alertes redondantes pour une même anomalie persistante. Cette logique est orchestrée lors de la création d'un relevé.

```csharp
// Extrait de la logique dans ReleveService.cs
private async Task VerifierEtGererAlertes(Releve releve)
{
    var seuils = await _seuilAlerteService.GetActiveBySondeAsync(releve.SondeId);
    foreach (var seuil in seuils)
    {
        bool estEnAlerte = (seuil.TypeSeuil == TypeSeuil.Minimum && releve.Valeur < seuil.Valeur) ||
                           (seuil.TypeSeuil == TypeSeuil.Maximum && releve.Valeur > seuil.Valeur);

        if (estEnAlerte)
        {
            // Vérification si une alerte est déjà en cours pour ce seuil précis
            var alerteExistante = await _alerteService.GetActiveBySondeAndSeuilAsync(releve.SondeId, seuil.Id);
            if (alerteExistante == null)
            {
                await _alerteService.CreerAlerteAsync(releve.SondeId, releve.Valeur, seuil);
            }
        }
        else
        {
            // Tentative de résolution automatique si la valeur est revenue à la normale
            await _alerteService.ResoudreAlertesSiNecessaireAsync(releve.SondeId, releve.Valeur, seuil);
        }
    }
}
```
**Analyse technique :** L'intelligence ici réside dans le couplage entre `ReleveService` et `AlerteService`. Le système ne se contente pas de crier à l'anomalie ; il vérifie d'abord si le problème est déjà connu (`alerteExistante == null`), évitant ainsi de saturer la base de données et l'interface utilisateur de notifications inutiles.

#### B. Gestion de la Résolution Automatique
La résolution automatique permet au système de se "réparer" tout seul d'un point de vue informationnel dès que les conditions environnementales redeviennent normales.

```csharp
// Extrait de AlerteService.cs
public async Task ResoudreAlertesSiNecessaireAsync(Guid sondeId, decimal valeurMesuree, SeuilAlerteDto seuil)
{
    var alertesActives = await _repository.GetActiveAlertesBySondeAndTypeAsync(sondeId, seuil.TypeSeuil);

    foreach (var alerte in alertesActives)
    {
        bool retourNormal = (seuil.TypeSeuil == TypeSeuil.Minimum && valeurMesuree >= seuil.Valeur) ||
                            (seuil.TypeSeuil == TypeSeuil.Maximum && valeurMesuree <= seuil.Valeur);

        if (retourNormal)
        {
            alerte.Statut = StatutAlerte.Resolue;
            alerte.DateResolution = DateTime.Now;
            alerte.Message += $" (Résolue automatiquement à {DateTime.Now:HH:mm})";
            await _repository.UpdateAsync(alerte);
        }
    }
}
```
**Analyse technique :** Cette méthode assure la fermeture de la boucle d'alerte. J'ai pris le soin de ne pas écraser le message initial, mais d'y apposer une mention temporelle, garantissant une traçabilité totale sur la durée de l'incident directement dans l'entité [Alerte](file:///c:/Users/Oussama%20JERIDI/Documents/GitHub/ESEO/solution-iot/src/Domain/Entities/Alerte.cs).

---

## 2. Suivi des Défis et Problèmes Rencontrés

*   **Cohérence du Mapping** : Pour ce module, j'ai délibérément évité l'utilisation de `Mapperly` pour certaines transformations complexes (notamment la construction de messages dynamiques). Cela a augmenté la verbosité du code mais a permis d'injecter une logique métier contextuelle plus fine lors de la création d'alertes.
*   **Concurrence de Traitement** : Dans un environnement où les relevés arrivent par rafales (via HttpPull), il y avait un risque de race condition. J'ai dû implémenter des vérifications d'existence robustes (`AnyAsync`) au niveau de l'infrastructure pour garantir l'unicité des alertes actives.
*   **Relations SQL Complexes** : La gestion des relations entre `Alerte`, `Sonde` et `SeuilAlerte` nécessitait une attention particulière sur le *eager loading*. J'ai optimisé les requêtes dans le Repository pour éviter le problème de performance "N+1" lors de l'affichage du dashboard.

---

## 3. Environnement de Développement [Hors notation]

*   **Outil choisi** : **Visual Studio 2022**.
*   **Raison du choix** : C'est l'IDE que je maîtrise le mieux pour l'écosystème .NET. Sa puissance pour le refactoring et l'intégration native avec SQL Server Object Explorer ont été des facteurs déterminants.
*   **Avis Critique** :
    *   **Points Positifs** : L'explorateur de tests et les outils de diagnostic de performance ont été cruciaux pour valider la réactivité du système d'alertes sous charge simulée.
    *   **Points Négatifs** : La lourdeur de l'application au démarrage et la consommation mémoire parfois excessive sur de longs cycles de développement.

---

## 4. Bilan de l'Apprentissage

Ce projet a été une étape clé dans mon parcours de développeur .NET :

1.  **Maîtrise de la Clean Architecture** : Comprendre comment la logique métier d'un service peut influencer un autre de manière propre (via les interfaces).
2.  **Logique d'État (FSM)** : Concevoir un cycle de vie d'entité robuste (`Active` -> `Acquittée` -> `Résolue`) avec des transitions automatiques et manuelles.
3.  **Optimisation SQL** : Apprendre à utiliser `Include()` et les projections de manière judicieuse pour maintenir la fluidité d'un dashboard de monitoring.
