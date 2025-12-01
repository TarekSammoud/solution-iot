using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité SeuilAlerte.
/// Gère les opérations CRUD et les requêtes spécifiques pour les seuils d'alerte.
/// Utilise eager loading pour charger la Sonde et trie par TypeSeuil puis TypeAlerte.
/// </summary>
public class SeuilAlerteRepository : ISeuilAlerteRepository
{
    private readonly AppDbContext _context;

    public SeuilAlerteRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un seuil d'alerte par son Id avec eager loading de la Sonde.
    /// </summary>
    public async Task<SeuilAlerte?> GetByIdAsync(Guid id)
    {
        return await _context.SeuilsAlerte
            .Include(s => s.Sonde)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Récupère tous les seuils d'alerte triés par SondeId, TypeSeuil, puis TypeAlerte.
    /// Eager loading de la Sonde pour éviter les requêtes N+1.
    /// </summary>
    public async Task<IEnumerable<SeuilAlerte>> GetAllAsync()
    {
        return await _context.SeuilsAlerte
            .Include(s => s.Sonde)
            .OrderBy(s => s.SondeId)
            .ThenBy(s => s.TypeSeuil)
            .ThenBy(s => s.TypeAlerte)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les seuils d'alerte d'une sonde spécifique.
    /// Triés par TypeSeuil puis TypeAlerte (ordre logique: Min Alerte, Min Avertissement, Max Alerte, Max Avertissement).
    /// </summary>
    public async Task<IEnumerable<SeuilAlerte>> GetBySondeAsync(Guid sondeId)
    {
        return await _context.SeuilsAlerte
            .Include(s => s.Sonde)
            .Where(s => s.SondeId == sondeId)
            .OrderBy(s => s.TypeSeuil)
            .ThenBy(s => s.TypeAlerte)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les seuils d'alerte actifs.
    /// Triés par SondeId, TypeSeuil, puis TypeAlerte.
    /// </summary>
    public async Task<IEnumerable<SeuilAlerte>> GetActifsAsync()
    {
        return await _context.SeuilsAlerte
            .Include(s => s.Sonde)
            .Where(s => s.EstActif)
            .OrderBy(s => s.SondeId)
            .ThenBy(s => s.TypeSeuil)
            .ThenBy(s => s.TypeAlerte)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les seuils d'alerte actifs d'une sonde spécifique.
    /// Double filtre: sonde ET actif.
    /// Triés par TypeSeuil puis TypeAlerte.
    /// </summary>
    public async Task<IEnumerable<SeuilAlerte>> GetBySondeActifsAsync(Guid sondeId)
    {
        return await _context.SeuilsAlerte
            .Include(s => s.Sonde)
            .Where(s => s.SondeId == sondeId && s.EstActif)
            .OrderBy(s => s.TypeSeuil)
            .ThenBy(s => s.TypeAlerte)
            .ToListAsync();
    }

    /// <summary>
    /// Ajoute un nouveau seuil d'alerte.
    /// Génère automatiquement un Id si non fourni.
    /// Définit DateCreation à DateTime.UtcNow si non définie.
    /// </summary>
    public async Task<SeuilAlerte> AddAsync(SeuilAlerte seuilAlerte)
    {
        // Génère un nouvel Id si non fourni
        if (seuilAlerte.Id == Guid.Empty)
        {
            seuilAlerte.Id = Guid.NewGuid();
        }

        // Définit DateCreation si non définie
        if (seuilAlerte.DateCreation == default)
        {
            seuilAlerte.DateCreation = DateTime.UtcNow;
        }

        await _context.SeuilsAlerte.AddAsync(seuilAlerte);
        await _context.SaveChangesAsync();
        return seuilAlerte;
    }

    /// <summary>
    /// Met à jour un seuil d'alerte existant.
    /// </summary>
    public async Task UpdateAsync(SeuilAlerte seuilAlerte)
    {
        _context.SeuilsAlerte.Update(seuilAlerte);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime un seuil d'alerte par son Id.
    /// Ne fait rien si le seuil n'existe pas.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var seuilAlerte = await _context.SeuilsAlerte.FindAsync(id);
        if (seuilAlerte != null)
        {
            _context.SeuilsAlerte.Remove(seuilAlerte);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si un seuil d'alerte existe par son Id.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.SeuilsAlerte.AnyAsync(s => s.Id == id);
    }
}
