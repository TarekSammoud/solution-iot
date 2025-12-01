using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité UniteMesure.
/// Utilise Entity Framework Core pour les opérations CRUD.
/// </summary>
public class UniteMesureRepository : IUniteMesureRepository
{
    private readonly AppDbContext _context;

    public UniteMesureRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère une unité de mesure par son identifiant.
    /// </summary>
    public async Task<UniteMesure?> GetByIdAsync(Guid id)
    {
        return await _context.UnitesMesures
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Récupère toutes les unités de mesure.
    /// Triées par TypeSonde puis par Nom.
    /// </summary>
    public async Task<IEnumerable<UniteMesure>> GetAllAsync()
    {
        return await _context.UnitesMesures
            .OrderBy(u => u.TypeSonde)
            .ThenBy(u => u.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les unités de mesure pour un type de sonde spécifique.
    /// Triées par Nom.
    /// </summary>
    public async Task<IEnumerable<UniteMesure>> GetByTypeSondeAsync(TypeSonde typeSonde)
    {
        return await _context.UnitesMesures
            .Where(u => u.TypeSonde == typeSonde)
            .OrderBy(u => u.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Ajoute une nouvelle unité de mesure en base de données.
    /// L'Id et la DateCreation sont générés automatiquement.
    /// </summary>
    public async Task<UniteMesure> AddAsync(UniteMesure uniteMesure)
    {
        // Génération de l'Id si non fourni
        if (uniteMesure.Id == Guid.Empty)
        {
            uniteMesure.Id = Guid.NewGuid();
        }

        // Génération de la DateCreation si non fournie
        if (uniteMesure.DateCreation == default)
        {
            uniteMesure.DateCreation = DateTime.UtcNow;
        }

        _context.UnitesMesures.Add(uniteMesure);
        await _context.SaveChangesAsync();

        return uniteMesure;
    }

    /// <summary>
    /// Met à jour une unité de mesure existante.
    /// </summary>
    public async Task UpdateAsync(UniteMesure uniteMesure)
    {
        _context.UnitesMesures.Update(uniteMesure);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une unité de mesure par son identifiant.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var uniteMesure = await GetByIdAsync(id);
        if (uniteMesure != null)
        {
            _context.UnitesMesures.Remove(uniteMesure);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si une unité de mesure existe.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.UnitesMesures.AnyAsync(u => u.Id == id);
    }
}
