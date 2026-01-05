using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité User.
/// Gère la persistance et la récupération des utilisateurs dans la base de données.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance du UserRepository.
    /// </summary>
    /// <param name="context">Contexte de base de données injecté.</param>
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère un utilisateur par son identifiant unique.
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Récupère un utilisateur par son nom d'utilisateur.
    /// Recherche insensible à la casse.
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// Récupère un utilisateur par son adresse email.
    /// Recherche insensible à la casse.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Récupère tous les utilisateurs triés par nom d'utilisateur.
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> SearchQuery(string query)
    {
        if (!String.IsNullOrEmpty(query))
        {
            return await _context.Users
                .Where(u => u.Username.ToUpper().Contains(query.ToUpper()) || u.Email.ToUpper().Contains(query.ToUpper()))
                .OrderBy(u => u.Username)
                .ToListAsync(); 
        }

        else 
            return await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

    }

    /// <summary>
    /// Ajoute un nouvel utilisateur dans la base de données.
    /// Génère automatiquement un nouvel identifiant si nécessaire et définit la date de création.
    /// </summary>
    public async Task<User> AddAsync(User user)
    {
        // Génère un nouvel identifiant si non défini
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }

        // Définit la date de création à maintenant (UTC) si non définie
        if (user.DateCreation == default)
        {
            user.DateCreation = DateTime.UtcNow;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Met à jour un utilisateur existant.
    /// </summary>
    public async Task UpdateAsync(User user)
    {
        // The user is already tracked because it was loaded in the service.
        await _context.SaveChangesAsync();
    }


    /// <summary>
    /// Supprime un utilisateur par son identifiant.
    /// Ne fait rien si l'utilisateur n'existe pas.
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountAsync()
    {
        return await _context.Users.CountAsync();
    }

    public async Task<Dictionary<string, int>> CountByRoleAsync()
    {
        return await _context.Users
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Role, x => x.Count);
    }


    /// <summary>
    /// Vérifie si un utilisateur existe avec l'identifiant spécifié.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
}
