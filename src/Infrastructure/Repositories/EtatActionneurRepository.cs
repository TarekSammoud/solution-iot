using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité EtatActionneur.
/// Gère la persistance des états actuels des actionneurs avec eager loading.
/// </summary>
public class EtatActionneurRepository : IEtatActionneurRepository
{
    private readonly AppDbContext _context;

    public EtatActionneurRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un état par son Id avec eager loading de l'Actionneur.
    /// </summary>
    public async Task<EtatActionneur?> GetByIdAsync(Guid id)
    {
        return await _context.EtatsActionneur
            .Include(e => e.Actionneur) // Eager loading pour éviter N+1 queries
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Récupère l'état actuel d'un actionneur spécifique (relation 1-to-1).
    /// Retourne au maximum un seul état grâce à la contrainte unique sur ActionneurId.
    /// </summary>
    public async Task<EtatActionneur?> GetByActionneurAsync(Guid actionneurId)
    {
        return await _context.EtatsActionneur
            .Include(e => e.Actionneur) // Eager loading
            .Where(e => e.ActionneurId == actionneurId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Récupère tous les états triés par date de dernière modification (plus récents en premier).
    /// Inclut l'eager loading de l'Actionneur pour chaque état.
    /// </summary>
    public async Task<IEnumerable<EtatActionneur>> GetAllAsync()
    {
        return await _context.EtatsActionneur
            .Include(e => e.Actionneur) // Eager loading
            .OrderByDescending(e => e.DerniereModification) // Plus récents en premier
            .ToListAsync();
    }

    /// <summary>
    /// Récupère uniquement les états actifs (EstActif = true).
    /// Utile pour afficher tous les actionneurs actuellement allumés/en marche.
    /// Triés par date de dernière modification descendant.
    /// </summary>
    public async Task<IEnumerable<EtatActionneur>> GetActifsAsync()
    {
        return await _context.EtatsActionneur
            .Include(e => e.Actionneur) // Eager loading
            .Where(e => e.EstActif) // Filtre sur état actif
            .OrderByDescending(e => e.DerniereModification) // Plus récents en premier
            .ToListAsync();
    }

    /// <summary>
    /// Ajoute un nouvel état d'actionneur.
    /// Génère automatiquement l'Id et définit DerniereModification si non fournis.
    /// </summary>
    public async Task<EtatActionneur> AddAsync(EtatActionneur etatActionneur)
    {
        // Génération automatique de l'Id si non fourni
        if (etatActionneur.Id == Guid.Empty)
        {
            etatActionneur.Id = Guid.NewGuid();
        }

        // Définition automatique de DerniereModification si non définie
        if (etatActionneur.DerniereModification == default)
        {
            etatActionneur.DerniereModification = DateTime.UtcNow;
        }

        await _context.EtatsActionneur.AddAsync(etatActionneur);
        await _context.SaveChangesAsync();

        return etatActionneur;
    }

    /// <summary>
    /// Met à jour un état d'actionneur existant.
    /// Met automatiquement à jour DerniereModification à DateTime.UtcNow.
    /// </summary>
    public async Task UpdateAsync(EtatActionneur etatActionneur)
    {
        // Mise à jour automatique de DerniereModification à chaque modification
        etatActionneur.DerniereModification = DateTime.UtcNow;

        _context.EtatsActionneur.Update(etatActionneur);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime un état d'actionneur par son Id.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var etatActionneur = await _context.EtatsActionneur.FindAsync(id);
        if (etatActionneur != null)
        {
            _context.EtatsActionneur.Remove(etatActionneur);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si un état existe avec l'Id spécifié.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.EtatsActionneur.AnyAsync(e => e.Id == id);
    }

    /// <summary>
    /// Vérifie si un état existe pour un actionneur spécifique.
    /// Utile pour vérifier la contrainte 1-to-1 avant création.
    /// </summary>
    public async Task<bool> ExistsByActionneurAsync(Guid actionneurId)
    {
        return await _context.EtatsActionneur.AnyAsync(e => e.ActionneurId == actionneurId);
    }
}
