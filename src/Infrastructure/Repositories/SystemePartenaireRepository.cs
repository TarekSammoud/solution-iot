using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité SystemePartenaire.
/// Gère la persistance et la récupération des systèmes partenaires dans la base de données.
/// </summary>
public class SystemePartenaireRepository : ISystemePartenaireRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance du SystemePartenaireRepository.
    /// </summary>
    /// <param name="context">Contexte de base de données injecté.</param>
    public SystemePartenaireRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un système partenaire par son identifiant unique.
    /// </summary>
    public async Task<SystemePartenaire?> GetByIdAsync(Guid id)
    {
        return await _context.SystemesPartenaires
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Récupère tous les systèmes partenaires triés par nom.
    /// </summary>
    public async Task<IEnumerable<SystemePartenaire>> GetAllAsync()
    {
        return await _context.SystemesPartenaires
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les systèmes partenaires appelants actifs.
    /// Filtre : EstAppelant = true ET EstActif = true.
    /// </summary>
    public async Task<IEnumerable<SystemePartenaire>> GetAppelantsAsync()
    {
        return await _context.SystemesPartenaires
            .Where(s => s.EstAppelant && s.EstActif)
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les systèmes partenaires appelés actifs.
    /// Filtre : EstAppele = true ET EstActif = true.
    /// </summary>
    public async Task<IEnumerable<SystemePartenaire>> GetAppelesAsync()
    {
        return await _context.SystemesPartenaires
            .Where(s => s.EstAppele && s.EstActif)
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère tous les systèmes partenaires actifs.
    /// Filtre : EstActif = true.
    /// </summary>
    public async Task<IEnumerable<SystemePartenaire>> GetActifsAsync()
    {
        return await _context.SystemesPartenaires
            .Where(s => s.EstActif)
            .OrderBy(s => s.Nom)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère un système partenaire par son nom d'utilisateur d'accès.
    /// Utilisé pour authentifier un système partenaire qui nous appelle.
    /// </summary>
    public async Task<SystemePartenaire?> GetByUsernameAccesAsync(string username)
    {
        return await _context.SystemesPartenaires
            .FirstOrDefaultAsync(s => s.UsernameAcces == username);
    }

    /// <summary>
    /// Ajoute un nouveau système partenaire dans la base de données.
    /// Génère automatiquement un nouvel identifiant si nécessaire et définit la date de création.
    /// </summary>
    public async Task<SystemePartenaire> AddAsync(SystemePartenaire systemePartenaire)
    {
        // Génère un nouvel identifiant si non défini
        if (systemePartenaire.Id == Guid.Empty)
        {
            systemePartenaire.Id = Guid.NewGuid();
        }

        // Définit la date de création à maintenant (UTC) si non définie
        if (systemePartenaire.DateCreation == default)
        {
            systemePartenaire.DateCreation = DateTime.UtcNow;
        }

        _context.SystemesPartenaires.Add(systemePartenaire);
        await _context.SaveChangesAsync();
        return systemePartenaire;
    }

    /// <summary>
    /// Met à jour un système partenaire existant.
    /// </summary>
    public async Task UpdateAsync(SystemePartenaire systemePartenaire)
    {
        _context.SystemesPartenaires.Update(systemePartenaire);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime un système partenaire par son identifiant.
    /// Ne fait rien si le système partenaire n'existe pas.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var systemePartenaire = await _context.SystemesPartenaires.FindAsync(id);
        if (systemePartenaire != null)
        {
            _context.SystemesPartenaires.Remove(systemePartenaire);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Vérifie si un système partenaire existe avec l'identifiant spécifié.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.SystemesPartenaires.AnyAsync(s => s.Id == id);
    }
}
