using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

/// <summary>
/// Interface du repository pour l'entité Releve.
/// Définit les opérations de lecture/écriture pour les relevés de mesure des sondes.
/// </summary>
public interface IReleveRepository
{
    /// <summary>
    /// Récupère un relevé par son identifiant avec eager loading de la Sonde.
    /// </summary>
    /// <param name="id">Identifiant du relevé.</param>
    /// <returns>Le relevé trouvé ou null si non trouvé.</returns>
    Task<Releve?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère tous les relevés avec eager loading de la Sonde, triés par date décroissante.
    /// </summary>
    /// <returns>Collection de tous les relevés.</returns>
    Task<IEnumerable<Releve>> GetAllAsync();

    /// <summary>
    /// Ajoute un nouveau relevé dans la base de données.
    /// </summary>
    /// <param name="releve">Le relevé à ajouter.</param>
    /// <returns>Le relevé ajouté avec son Id généré.</returns>
    Task<Releve> AddAsync(Releve releve);

    /// <summary>
    /// Met à jour un relevé existant.
    /// </summary>
    /// <param name="releve">Le relevé à mettre à jour.</param>
    Task UpdateAsync(Releve releve);

    /// <summary>
    /// Supprime un relevé par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant du relevé à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Récupère tous les relevés d'une sonde spécifique, triés par date décroissante.
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <returns>Collection des relevés de la sonde.</returns>
    Task<IEnumerable<Releve>> GetBySondeAsync(Guid sondeId);

    /// <summary>
    /// Récupère les relevés d'une sonde dans une plage de dates (bornes inclusives).
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <param name="dateDebut">Date de début (inclusive).</param>
    /// <param name="dateFin">Date de fin (inclusive).</param>
    /// <returns>Collection des relevés dans la plage de dates, triés par date décroissante.</returns>
    Task<IEnumerable<Releve>> GetBySondeDateRangeAsync(Guid sondeId, DateTime dateDebut, DateTime dateFin);

    /// <summary>
    /// Récupère le dernier relevé (plus récent) d'une sonde.
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <returns>Le relevé le plus récent ou null si aucun relevé.</returns>
    Task<Releve?> GetLastBySondeAsync(Guid sondeId);

    /// <summary>
    /// Récupère tous les relevés d'un type donné (Manuel ou Automatique).
    /// </summary>
    /// <param name="typeReleve">Type de relevé à filtrer.</param>
    /// <returns>Collection des relevés du type spécifié, triés par date décroissante.</returns>
    Task<IEnumerable<Releve>> GetByTypeAsync(TypeReleve typeReleve);

    /// <summary>
    /// Compte le nombre total de relevés pour une sonde donnée.
    /// </summary>
    /// <param name="sondeId">Identifiant de la sonde.</param>
    /// <returns>Nombre de relevés de la sonde.</returns>
    Task<int> CountBySondeAsync(Guid sondeId);
}
