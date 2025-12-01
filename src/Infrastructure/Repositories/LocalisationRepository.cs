using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité Localisation.
/// Gère la persistance et la récupération des localisations dans la base de données.
/// </summary>
public class LocalisationRepository : ILocalisationRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance du LocalisationRepository.
    /// </summary>
    /// <param name="context">Contexte de base de données injecté.</param>
    public LocalisationRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère une localisation par son identifiant unique.
    /// </summary>
    public async Task<Localisation?> GetByIdAsync(Guid id)
    {
        return await _context.Localisations
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    /// <summary>
    /// Récupère toutes les localisations triées par nom.
    /// </summary>
    public async Task<IEnumerable<Localisation>> GetAllAsync()
    {
        return await _context.Localisations
            .OrderBy(l => l.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Ajoute une nouvelle localisation dans la base de données.
    /// Génère automatiquement un nouvel identifiant si nécessaire et définit la date de création.
    /// </summary>
    public async Task<Localisation> AddAsync(Localisation localisation)
    {
        // Génère un nouvel identifiant si non défini
        if (localisation.Id == Guid.Empty)
        {
            localisation.Id = Guid.NewGuid();
        }

        // Définit la date de création à maintenant (UTC) si non définie
        if (localisation.DateCreation == default)
        {
            localisation.DateCreation = DateTime.UtcNow;
        }

        _context.Localisations.Add(localisation);
        await _context.SaveChangesAsync();
        return localisation;
    }

    /// <summary>
    /// Met à jour une localisation existante.
    /// </summary>
    public async Task UpdateAsync(Localisation localisation)
    {
        _context.Localisations.Update(localisation);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une localisation par son identifiant.
    /// Ne fait rien si la localisation n'existe pas.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var localisation = await _context.Localisations.FindAsync(id);
        if (localisation != null)
        {
            _context.Localisations.Remove(localisation);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si une localisation existe avec l'identifiant spécifié.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Localisations.AnyAsync(l => l.Id == id);
    }
}
