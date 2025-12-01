using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité Alerte.
/// Gère les opérations CRUD et les requêtes spécifiques pour les alertes.
/// Utilise eager loading pour charger la Sonde et le SeuilAlerte et trie par DateCreation descendant.
/// </summary>
public class AlerteRepository : IAlerteRepository
{
    private readonly AppDbContext _context;

    public AlerteRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère une alerte par son Id avec eager loading de la Sonde et du SeuilAlerte.
    /// </summary>
    public async Task<Alerte?> GetByIdAsync(Guid id)
    {
        return await _context.Alertes
            .Include(a => a.Sonde)
            .Include(a => a.SeuilAlerte)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// Récupère toutes les alertes triées par DateCreation descendant (plus récentes en premier).
    /// Eager loading de la Sonde et du SeuilAlerte pour éviter les requêtes N+1.
    /// </summary>
    public async Task<IEnumerable<Alerte>> GetAllAsync()
    {
        return await _context.Alertes
            .Include(a => a.Sonde)
            .Include(a => a.SeuilAlerte)
            .OrderByDescending(a => a.DateCreation)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les alertes d'une sonde spécifique.
    /// Triées par DateCreation descendant (plus récentes en premier).
    /// </summary>
    public async Task<IEnumerable<Alerte>> GetBySondeAsync(Guid sondeId)
    {
        return await _context.Alertes
            .Include(a => a.Sonde)
            .Include(a => a.SeuilAlerte)
            .Where(a => a.SondeId == sondeId)
            .OrderByDescending(a => a.DateCreation)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les alertes ayant un statut spécifique.
    /// Triées par DateCreation descendant (plus récentes en premier).
    /// </summary>
    public async Task<IEnumerable<Alerte>> GetByStatutAsync(StatutAlerte statut)
    {
        return await _context.Alertes
            .Include(a => a.Sonde)
            .Include(a => a.SeuilAlerte)
            .Where(a => a.Statut == statut)
            .OrderByDescending(a => a.DateCreation)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère toutes les alertes actives (Statut = Active).
    /// Triées par DateCreation descendant (plus récentes en premier).
    /// </summary>
    public async Task<IEnumerable<Alerte>> GetActivesAsync()
    {
        return await _context.Alertes
            .Include(a => a.Sonde)
            .Include(a => a.SeuilAlerte)
            .Where(a => a.Statut == StatutAlerte.Active)
            .OrderByDescending(a => a.DateCreation)
            .ToListAsync();
    }

    /// <summary>
    /// Vérifie si une alerte du même TypeAlerte est déjà active pour un seuil donné.
    /// CRITIQUE pour la logique métier : empêche la création de doublons d'alertes.
    ///
    /// Filtre complexe par :
    /// - SondeId : la sonde concernée
    /// - SeuilAlerteId : le seuil d'alerte concerné
    /// - TypeAlerte : le type d'alerte (Alerte ou Avertissement)
    /// - Statut = Active : uniquement les alertes actives
    ///
    /// Le filtre par TypeAlerte est ESSENTIEL pour permettre la coexistence
    /// d'un Avertissement actif ET d'une Alerte active pour le même seuil.
    /// </summary>
    public async Task<Alerte?> GetAlerteActiveForSeuilAsync(Guid sondeId, Guid seuilAlerteId, TypeAlerte typeAlerte)
    {
        return await _context.Alertes
            .Include(a => a.Sonde)
            .Include(a => a.SeuilAlerte)
            .Where(a => a.SondeId == sondeId
                     && a.SeuilAlerteId == seuilAlerteId
                     && a.TypeAlerte == typeAlerte
                     && a.Statut == StatutAlerte.Active)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Ajoute une nouvelle alerte.
    /// Génère automatiquement un Id si non fourni.
    /// Définit DateCreation à DateTime.UtcNow si non définie.
    /// Définit Statut à Active si non défini.
    /// </summary>
    public async Task<Alerte> AddAsync(Alerte alerte)
    {
        // Génère un nouvel Id si non fourni
        if (alerte.Id == Guid.Empty)
        {
            alerte.Id = Guid.NewGuid();
        }

        // Définit DateCreation si non définie
        if (alerte.DateCreation == default)
        {
            alerte.DateCreation = DateTime.UtcNow;
        }

        // Définit Statut à Active si non défini (valeur par défaut de l'enum est 0 = Active)
        if (alerte.Statut == default)
        {
            alerte.Statut = StatutAlerte.Active;
        }

        await _context.Alertes.AddAsync(alerte);
        await _context.SaveChangesAsync();
        return alerte;
    }

    /// <summary>
    /// Met à jour une alerte existante.
    /// Utilisé notamment pour changer le statut (Active → Acquittee → Resolue).
    /// </summary>
    public async Task UpdateAsync(Alerte alerte)
    {
        _context.Alertes.Update(alerte);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une alerte par son Id.
    /// Ne fait rien si l'alerte n'existe pas.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var alerte = await _context.Alertes.FindAsync(id);
        if (alerte != null)
        {
            _context.Alertes.Remove(alerte);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si une alerte existe par son Id.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Alertes.AnyAsync(a => a.Id == id);
    }
}
