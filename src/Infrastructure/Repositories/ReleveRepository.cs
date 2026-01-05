using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité Releve.
/// Gère les opérations CRUD et les requêtes spécifiques pour les relevés de mesure.
/// Utilise eager loading pour charger la Sonde et trie par DateHeure descendant.
/// </summary>
public class ReleveRepository : IReleveRepository
{
    private readonly AppDbContext _context;

    public ReleveRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Releve?> GetByIdAsync(Guid id)
    {
        return await _context.Releves
            .Include(r => r.Sonde)
            .Include(r => r.Sonde.Localisation)
            .Include(r => r.Sonde.UniteMesure)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Releve>> GetAllAsync()
    {
        return await _context.Releves
            .Include(r => r.Sonde)
            .Include(r=>r.Sonde.Localisation)
            .Include(r=>r.Sonde.UniteMesure)
            .OrderByDescending(r => r.DateHeure)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les Releves par filtres.
    /// </summary>
    /// <returns>Collection de tous les relevés.</returns>
    public async Task<IEnumerable<Releve>> GetAllAsync(int page = 0, int limit = 50, TypeReleve? type = null, DateTime? StartDate = null, DateTime? EndDate = null)
    {
        IQueryable<Releve> query = _context.Releves
            .Include(r => r.Sonde)
            .Include(r => r.Sonde.Localisation)
            .Include(r => r.Sonde.UniteMesure)
            .OrderByDescending(r => r.DateHeure);
            

        if (type.HasValue)
        {
            query = query.Where(r => r.TypeReleve == type.Value);
        }

        if (StartDate.HasValue)
        {
            query = query.Where(r => r.DateHeure >= StartDate.Value);
        }

        if (EndDate.HasValue)
        {
            query = query.Where(r => r.DateHeure <= EndDate.Value);
        }

        return await query.Skip(page * limit)
            .Take(limit).ToListAsync();
    }

    public async Task<Releve> AddAsync(Releve releve)
    {
        // Génère un nouvel Id si non fourni
        if (releve.Id == Guid.Empty)
        {
            releve.Id = Guid.NewGuid();
        }

        await _context.Releves.AddAsync(releve);
        await _context.SaveChangesAsync();
        return releve;
    }

    public async Task UpdateAsync(Releve releve)
    {
        _context.Releves.Update(releve);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var releve = await _context.Releves.FindAsync(id);
        if (releve != null)
        {
            _context.Releves.Remove(releve);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Releve>> GetBySondeAsync(Guid sondeId)
    {
        return await _context.Releves
            .Include(r => r.Sonde)
            .Include(r => r.Sonde.Localisation)
            .Include(r => r.Sonde.UniteMesure)
            .Where(r => r.SondeId == sondeId)
            .OrderByDescending(r => r.DateHeure)
            .ToListAsync();
    }

    public async Task<IEnumerable<Releve>> GetBySondeDateRangeAsync(Guid sondeId, DateTime dateDebut, DateTime dateFin)
    {
        return await _context.Releves
            .Include(r => r.Sonde)
            .Include(r => r.Sonde.Localisation)
            .Include(r => r.Sonde.UniteMesure)
            .Where(r => r.SondeId == sondeId && r.DateHeure >= dateDebut && r.DateHeure <= dateFin)
            .OrderByDescending(r => r.DateHeure)
            .ToListAsync();
    }

    public async Task<Releve?> GetLastBySondeAsync(Guid sondeId)
    {
        return await _context.Releves
            .Include(r => r.Sonde)
            .Include(r => r.Sonde.Localisation)
            .Include(r => r.Sonde.UniteMesure)
            .Where(r => r.SondeId == sondeId)
            .OrderByDescending(r => r.DateHeure)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Releve>> GetByTypeAsync(TypeReleve typeReleve)
    {
        return await _context.Releves
            .Include(r => r.Sonde)
            .Include(r => r.Sonde.Localisation)
            .Include(r => r.Sonde.UniteMesure)
            .Where(r => r.TypeReleve == typeReleve)
            .OrderByDescending(r => r.DateHeure)
            .ToListAsync();
    }

    public async Task<int> CountBySondeAsync(Guid sondeId)
    {
        return await _context.Releves
            .CountAsync(r => r.SondeId == sondeId);
    }
    public async Task<int> CountRelevesAsync(TypeReleve? type = null, DateTime? StartDate = null, DateTime? EndDate = null)
    {
        IQueryable<Releve> query = _context.Releves;
        if (type.HasValue)
        {
            query = query.Where(r => r.TypeReleve == type.Value);
        }

        if (StartDate.HasValue)
        {
            query = query.Where(r => r.DateHeure >= StartDate.Value);
        }

        if (EndDate.HasValue)
        {
            query = query.Where(r => r.DateHeure <= EndDate.Value);
        }
        return query.Count();

    }
}
