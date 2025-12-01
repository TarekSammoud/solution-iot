using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le UserRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// </summary>
public class UserRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _repository;
    private readonly string _databaseName;

    // IDs des utilisateurs seedés pour référence dans les tests
    private readonly Guid _adminId;
    private readonly Guid _user1Id;
    private readonly Guid _user2Id;

    public UserRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie
        _adminId = Guid.NewGuid();
        _user1Id = Guid.NewGuid();
        _user2Id = Guid.NewGuid();

        var admin = new User
        {
            Id = _adminId,
            Username = "admin",
            Email = "admin@iot.com",
            PasswordHash = "hash_admin_secure",
            Role = RoleUtilisateur.Administrateur,
            DateCreation = DateTime.UtcNow.AddDays(-30), // Créé il y a 30 jours
            EstActif = true
        };

        var user1 = new User
        {
            Id = _user1Id,
            Username = "user1",
            Email = "user1@iot.com",
            PasswordHash = "hash_user1_secure",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow.AddDays(-15), // Créé il y a 15 jours
            EstActif = true
        };

        var user2 = new User
        {
            Id = _user2Id,
            Username = "user2",
            Email = "user2@iot.com",
            PasswordHash = "hash_user2_secure",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow.AddDays(-5), // Créé il y a 5 jours
            EstActif = false // Compte inactif
        };

        _context.Users.AddRange(admin, user1, user2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer un nouvel utilisateur
        var newUser = new User
        {
            Username = "newuser",
            Email = "newuser@iot.com",
            PasswordHash = "hash_newuser",
            Role = RoleUtilisateur.Utilisateur,
            EstActif = true
        };

        // Act & Assert - CREATE
        var createdUser = await _repository.AddAsync(newUser);
        Assert.NotEqual(Guid.Empty, createdUser.Id);
        Assert.Equal("newuser", createdUser.Username);

        // Act & Assert - READ
        var readUser = await _repository.GetByIdAsync(createdUser.Id);
        Assert.NotNull(readUser);
        Assert.Equal("newuser", readUser.Username);
        Assert.Equal("newuser@iot.com", readUser.Email);

        // Act & Assert - UPDATE
        readUser.Email = "updated@iot.com";
        await _repository.UpdateAsync(readUser);
        var updatedUser = await _repository.GetByIdAsync(createdUser.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("updated@iot.com", updatedUser.Email);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdUser.Id);
        var deletedUser = await _repository.GetByIdAsync(createdUser.Id);
        Assert.Null(deletedUser);

        // Vérifier que les utilisateurs seedés n'ont pas été affectés
        var allUsers = await _repository.GetAllAsync();
        Assert.Equal(3, allUsers.Count()); // admin, user1, user2 toujours présents
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerUtilisateursTriesParUsername()
    {
        // Arrange - Les données sont déjà seedées (admin, user1, user2)

        // Act - Récupérer tous les utilisateurs
        var users = await _repository.GetAllAsync();
        var userList = users.ToList();

        // Assert - Vérifier le tri par username (ordre alphabétique)
        Assert.Equal(3, userList.Count);
        Assert.Equal("admin", userList[0].Username); // 1er alphabétiquement
        Assert.Equal("user1", userList[1].Username); // 2ème alphabétiquement
        Assert.Equal("user2", userList[2].Username); // 3ème alphabétiquement

        // Vérifier également que tous les utilisateurs sont bien présents
        Assert.Contains(userList, u => u.Id == _adminId);
        Assert.Contains(userList, u => u.Id == _user1Id);
        Assert.Contains(userList, u => u.Id == _user2Id);
    }

    [Fact]
    public async Task GetByUsernameAsync_DevraitEtreCaseSensitive()
    {
        // Arrange - L'utilisateur "admin" existe avec cette casse exacte

        // Act - Rechercher avec la casse exacte
        var userExactCase = await _repository.GetByUsernameAsync("admin");

        // Act - Rechercher avec une casse différente
        var userUpperCase = await _repository.GetByUsernameAsync("ADMIN");
        var userMixedCase = await _repository.GetByUsernameAsync("Admin");

        // Assert - Seule la casse exacte devrait retourner un résultat
        // Note: Le comportement dépend de la configuration de la base de données
        // SQLite InMemory est généralement case-sensitive pour les comparaisons
        Assert.NotNull(userExactCase);
        Assert.Equal("admin", userExactCase.Username);

        // Ces assertions peuvent varier selon la configuration de la BD
        // Pour SQLite, la comparaison est généralement case-sensitive
        // Si votre implémentation doit être case-insensitive, ajustez le repository
        Assert.Null(userUpperCase); // Devrait être null si case-sensitive
        Assert.Null(userMixedCase); // Devrait être null si case-sensitive
    }

    //[Fact]
    //public async Task AddAsync_DevraitLeverException_SiUsernameExiste()
    //{
    //    // Arrange - Créer un utilisateur avec un username déjà existant
    //    var duplicateUser = new User
    //    {
    //        Username = "admin", // Username déjà utilisé par l'admin seedé
    //        Email = "different@iot.com", // Email différent
    //        PasswordHash = "hash_duplicate",
    //        Role = RoleUtilisateur.Utilisateur,
    //        DateCreation = DateTime.UtcNow,
    //        EstActif = true
    //    };

    //    // Act & Assert - L'ajout devrait lever une DbUpdateException
    //    // en raison de la contrainte unique sur Username
    //    await Assert.ThrowsAsync<DbUpdateException>(async () =>
    //    {
    //        await _repository.AddAsync(duplicateUser);
    //    });
    //}

    //[Fact]
    //public async Task AddAsync_DevraitLeverException_SiEmailExiste()
    //{
    //    // Arrange - Créer un utilisateur avec un email déjà existant
    //    var duplicateUser = new User
    //    {
    //        Username = "newusername", // Username différent
    //        Email = "admin@iot.com", // Email déjà utilisé par l'admin seedé
    //        PasswordHash = "hash_duplicate",
    //        Role = RoleUtilisateur.Utilisateur,
    //        DateCreation = DateTime.UtcNow,
    //        EstActif = true
    //    };

    //    // Act & Assert - L'ajout devrait lever une DbUpdateException
    //    // en raison de la contrainte unique sur Email
    //    await Assert.ThrowsAsync<DbUpdateException>(async () =>
    //    {
    //        await _repository.AddAsync(duplicateUser);
    //    });
    //}

    /// <summary>
    /// Nettoyage après chaque test - supprime la base de données InMemory.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
