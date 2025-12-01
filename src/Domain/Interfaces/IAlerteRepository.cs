using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

/// <summary>
/// Interface du repository pour l'entité Alerte.
/// Définit les opérations de lecture/écriture pour les alertes générées par dépassement de seuils.
/// </summary>
public interface IAlerteRepository
{
    /// <summary>
    /// Récupère une alerte par son identifiant avec eager loading de la Sonde et du SeuilAlerte.
    /// </summary>
    /// <param name="id">Identifiant de l'alerte.</param>
    /// <returns>L'alerte trouvée ou null si non trouvée.</returns>
    Task<Alerte?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère toutes les alertes avec eager loading de la Sonde et du SeuilAlerte.
    /// Les résultats sont triés par DateCreation descendant (plus récentes en premier).
    /// </summary>
    /// <returns>Collection de toutes les alertes.</returns>
    Task<IEnumerable<Alerte>> GetAllAsync();

    /// <summary>
    /// Récupère toutes les alertes d'une sonde spécifique.
    /// Les résultats sont triés par DateCreation descendant (plus récentes en premier).
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <returns>Collection des alertes de la sonde.</returns>
    Task<IEnumerable<Alerte>> GetBySondeAsync(Guid sondeId);

    /// <summary>
    /// Récupère toutes les alertes ayant un statut spécifique.
    /// Les résultats sont triés par DateCreation descendant (plus récentes en premier).
    /// </summary>
    /// <param name="statut">Statut des alertes à récupérer (Active, Acquittee, Resolue).</param>
    /// <returns>Collection des alertes avec le statut spécifié.</returns>
    Task<IEnumerable<Alerte>> GetByStatutAsync(StatutAlerte statut);

    /// <summary>
    /// Récupère toutes les alertes actives (Statut = Active).
    /// Les résultats sont triés par DateCreation descendant (plus récentes en premier).
    /// </summary>
    /// <returns>Collection des alertes actives.</returns>
    Task<IEnumerable<Alerte>> GetActivesAsync();

    /// <summary>
    /// Vérifie si une alerte du même TypeAlerte est déjà active pour un seuil donné.
    /// CRITIQUE pour la logique métier : empêche la création de doublons d'alertes.
    ///
    /// Filtre par :
    /// - SondeId : la sonde concernée
    /// - SeuilAlerteId : le seuil d'alerte concerné
    /// - TypeAlerte : le type d'alerte (Alerte ou Avertissement)
    /// - Statut = Active : uniquement les alertes actives
    ///
    /// Note importante : Cette méthode filtre par TypeAlerte pour permettre la coexistence
    /// d'un Avertissement actif ET d'une Alerte active pour le même seuil (même TypeSeuil).
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <param name="seuilAlerteId">Identifiant du seuil d'alerte.</param>
    /// <param name="typeAlerte">Type d'alerte à rechercher (Alerte ou Avertissement).</param>
    /// <returns>L'alerte active trouvée ou null si aucune alerte active de ce type n'existe.</returns>
    Task<Alerte?> GetAlerteActiveForSeuilAsync(Guid sondeId, Guid seuilAlerteId, TypeAlerte typeAlerte);

    /// <summary>
    /// Ajoute une nouvelle alerte dans la base de données.
    /// Génère automatiquement un Id si non fourni.
    /// Définit DateCreation à DateTime.UtcNow si non définie.
    /// Définit Statut à Active si non défini.
    /// </summary>
    /// <param name="alerte">L'alerte à ajouter.</param>
    /// <returns>L'alerte ajoutée avec son Id généré.</returns>
    Task<Alerte> AddAsync(Alerte alerte);

    /// <summary>
    /// Met à jour une alerte existante.
    /// Utilisé notamment pour changer le statut (Active → Acquittee → Resolue).
    /// </summary>
    /// <param name="alerte">L'alerte à mettre à jour.</param>
    Task UpdateAsync(Alerte alerte);

    /// <summary>
    /// Supprime une alerte par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'alerte à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si une alerte existe par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'alerte.</param>
    /// <returns>True si l'alerte existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
