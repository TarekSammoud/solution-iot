using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

/// <summary>
/// Interface du repository pour l'entité UniteMesure.
/// Fournit les opérations CRUD et des méthodes de filtrage par type de sonde.
/// </summary>
public interface IUniteMesureRepository
{
    /// <summary>
    /// Récupère une unité de mesure par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'unité de mesure.</param>
    /// <returns>L'unité de mesure trouvée ou null si inexistante.</returns>
    Task<UniteMesure?> GetByIdAsync(Guid id);

    /// <summary>
    /// Récupère toutes les unités de mesure.
    /// Triées par TypeSonde puis par Nom.
    /// </summary>
    /// <returns>Liste de toutes les unités de mesure.</returns>
    Task<IEnumerable<UniteMesure>> GetAllAsync();

    /// <summary>
    /// Récupère toutes les unités de mesure pour un type de sonde spécifique.
    /// Triées par Nom.
    /// </summary>
    /// <param name="typeSonde">Type de sonde à filtrer.</param>
    /// <returns>Liste des unités de mesure pour ce type de sonde.</returns>
    Task<IEnumerable<UniteMesure>> GetByTypeSondeAsync(TypeSonde typeSonde);

    /// <summary>
    /// Ajoute une nouvelle unité de mesure en base de données.
    /// L'Id et la DateCreation sont générés automatiquement.
    /// </summary>
    /// <param name="uniteMesure">L'unité de mesure à ajouter.</param>
    /// <returns>L'unité de mesure créée avec Id et DateCreation.</returns>
    Task<UniteMesure> AddAsync(UniteMesure uniteMesure);

    /// <summary>
    /// Met à jour une unité de mesure existante.
    /// </summary>
    /// <param name="uniteMesure">L'unité de mesure à mettre à jour.</param>
    Task UpdateAsync(UniteMesure uniteMesure);

    /// <summary>
    /// Supprime une unité de mesure par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de l'unité de mesure à supprimer.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Vérifie si une unité de mesure existe.
    /// </summary>
    /// <param name="id">Identifiant de l'unité de mesure.</param>
    /// <returns>True si l'unité existe, false sinon.</returns>
    Task<bool> ExistsAsync(Guid id);
}
