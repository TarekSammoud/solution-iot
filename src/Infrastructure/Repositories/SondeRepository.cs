using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité Sonde.
/// Utilise Entity Framework Core pour les opérations CRUD.
/// Toutes les méthodes de récupération incluent le eager loading de Localisation et UniteMesure.
/// </summary>
public class SondeRepository : ISondeRepository
{
    private readonly AppDbContext _context;

    public SondeRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère une sonde par son identifiant avec eager loading.
    /// </summary>
    public async Task<Sonde?> GetByIdAsync(Guid id)
    {
        // Eager loading de Localisation et UniteMesure pour éviter les requêtes N+1
        return await _context.Sondes
            .Include(s => s.Localisation)
            .Include(s => s.UniteMesure)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Récupère toutes les sondes avec eager loading.
    /// Triées par Nom.
    /// </summary>
    public async Task<IEnumerable<Sonde>> GetAllAsync()
    {
        return await _context.Sondes
            .Include(s => s.Localisation)
            .Include(s => s.UniteMesure)
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les sondes d'une localisation spécifique avec eager loading.
    /// Triées par Nom.
    /// </summary>
    public async Task<IEnumerable<Sonde>> GetByLocalisationAsync(Guid localisationId)
    {
        return await _context.Sondes
            .Include(s => s.Localisation)
            .Include(s => s.UniteMesure)
            .Where(s => s.LocalisationId == localisationId)
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les sondes d'un type spécifique avec eager loading.
    /// Triées par Nom.
    /// </summary>
    public async Task<IEnumerable<Sonde>> GetByTypeAsync(TypeSonde type)
    {
        return await _context.Sondes
            .Include(s => s.Localisation)
            .Include(s => s.UniteMesure)
            .Where(s => s.TypeSonde == type)
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les sondes actives avec eager loading.
    /// Triées par Nom.
    /// </summary>
    public async Task<IEnumerable<Sonde>> GetActivesAsync()
    {
        return await _context.Sondes
            .Include(s => s.Localisation)
            .Include(s => s.UniteMesure)
            .Where(s => s.EstActif)
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Ajoute une nouvelle sonde en base de données.
    /// L'Id, DateCreation et DateInstallation sont générés automatiquement si non fournis.
    /// </summary>
    public async Task<Sonde> AddAsync(Sonde sonde)
    {
        // Génération de l'Id si non fourni
        if (sonde.Id == Guid.Empty)
        {
            sonde.Id = Guid.NewGuid();
        }

        // Génération de la DateCreation si non fournie
        if (sonde.DateCreation == default)
        {
            sonde.DateCreation = DateTime.UtcNow;
        }

        // Génération de la DateInstallation si non fournie
        if (sonde.DateInstallation == default)
        {
            sonde.DateInstallation = DateTime.UtcNow;
        }

        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        return sonde;
    }

    /// <summary>
    /// Met à jour une sonde existante.
    /// </summary>
    public async Task UpdateAsync(Sonde sonde)
    {
        _context.Sondes.Update(sonde);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une sonde par son identifiant.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var sonde = await _context.Sondes.FindAsync(id);
        if (sonde != null)
        {
            _context.Sondes.Remove(sonde);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si une sonde existe.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Sondes.AnyAsync(s => s.Id == id);
    }
}
