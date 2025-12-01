using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité Actionneur.
/// Utilise Entity Framework Core pour les opérations CRUD.
/// Toutes les méthodes de récupération incluent le eager loading de Localisation.
/// </summary>
public class ActionneurRepository : IActionneurRepository
{
    private readonly AppDbContext _context;

    public ActionneurRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un actionneur par son identifiant avec eager loading.
    /// </summary>
    public async Task<Actionneur?> GetByIdAsync(Guid id)
    {
        // Eager loading de Localisation pour éviter les requêtes N+1
        return await _context.Actionneurs
            .Include(a => a.Localisation)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// Récupère tous les actionneurs avec eager loading.
    /// Triés par Nom.
    /// </summary>
    public async Task<IEnumerable<Actionneur>> GetAllAsync()
    {
        return await _context.Actionneurs
            .Include(a => a.Localisation)
            .OrderBy(a => a.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les actionneurs d'une localisation spécifique avec eager loading.
    /// Triés par Nom.
    /// </summary>
    public async Task<IEnumerable<Actionneur>> GetByLocalisationAsync(Guid localisationId)
    {
        return await _context.Actionneurs
            .Include(a => a.Localisation)
            .Where(a => a.LocalisationId == localisationId)
            .OrderBy(a => a.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les actionneurs d'un type spécifique avec eager loading.
    /// Triés par Nom.
    /// </summary>
    public async Task<IEnumerable<Actionneur>> GetByTypeAsync(TypeActionneur type)
    {
        return await _context.Actionneurs
            .Include(a => a.Localisation)
            .Where(a => a.TypeActionneur == type)
            .OrderBy(a => a.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les actionneurs actifs avec eager loading.
    /// Triés par Nom.
    /// </summary>
    public async Task<IEnumerable<Actionneur>> GetActifsAsync()
    {
        return await _context.Actionneurs
            .Include(a => a.Localisation)
            .Where(a => a.EstActif)
            .OrderBy(a => a.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Ajoute un nouvel actionneur en base de données.
    /// L'Id, DateCreation et DateInstallation sont générés automatiquement si non fournis.
    /// </summary>
    public async Task<Actionneur> AddAsync(Actionneur actionneur)
    {
        // Génération de l'Id si non fourni
        if (actionneur.Id == Guid.Empty)
        {
            actionneur.Id = Guid.NewGuid();
        }

        // Génération de la DateCreation si non fournie
        if (actionneur.DateCreation == default)
        {
            actionneur.DateCreation = DateTime.UtcNow;
        }

        // Génération de la DateInstallation si non fournie
        if (actionneur.DateInstallation == default)
        {
            actionneur.DateInstallation = DateTime.UtcNow;
        }

        _context.Actionneurs.Add(actionneur);
        await _context.SaveChangesAsync();

        return actionneur;
    }

    /// <summary>
    /// Met à jour un actionneur existant.
    /// </summary>
    public async Task UpdateAsync(Actionneur actionneur)
    {
        _context.Actionneurs.Update(actionneur);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime un actionneur par son identifiant.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var actionneur = await _context.Actionneurs.FindAsync(id);
        if (actionneur != null)
        {
            _context.Actionneurs.Remove(actionneur);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si un actionneur existe.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Actionneurs.AnyAsync(a => a.Id == id);
    }
}
