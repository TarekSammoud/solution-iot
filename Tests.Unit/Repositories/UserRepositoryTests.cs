using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour le UserRepository.
/// Utilise InMemoryDatabase pour tester les opérations CRUD sans dépendance à une vraie base de données.
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _repository;
    private readonly string _databaseName;

    public UserRepositoryTests()
    {
        // Crée une base de données InMemory avec un nom unique par test (évite les conflits)
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterUtilisateur()
    {
        // Arrange - Préparer un utilisateur de test sans Id ni DateCreation
        var user = new User
        {
            Username = "admin",
            Email = "admin@iot.com",
            PasswordHash = "hash_secure_123",
            Role = RoleUtilisateur.Administrateur,
            EstActif = true
        };

        // Act - Ajouter l'utilisateur
        var result = await _repository.AddAsync(user);

        // Assert - Vérifier que l'Id et la DateCreation ont été générés
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.DateCreation);
        Assert.Equal("admin", result.Username);
        Assert.Equal("admin@iot.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerUtilisateur_QuandExiste()
    {
        // Arrange - Créer et ajouter un utilisateur de test
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "user1",
            Email = "user1@iot.com",
            PasswordHash = "hash_user1",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };
        await _repository.AddAsync(user);

        // Act - Récupérer l'utilisateur par son Id
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert - Vérifier que l'utilisateur est trouvé
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("user1", result.Username);
        Assert.Equal("user1@iot.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas dans la base
        var nonExistentId = Guid.NewGuid();

        // Act - Tenter de récupérer un utilisateur inexistant
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert - Vérifier que le résultat est null
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_DevraitRetournerUtilisateur_QuandExiste()
    {
        // Arrange - Créer et ajouter un utilisateur de test
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testuser@iot.com",
            PasswordHash = "hash_test",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };
        await _repository.AddAsync(user);

        // Act - Rechercher par nom d'utilisateur
        var result = await _repository.GetByUsernameAsync("testuser");

        // Assert - Vérifier que l'utilisateur est trouvé
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_DevraitRetournerUtilisateur_QuandExiste()
    {
        // Arrange - Créer et ajouter un utilisateur de test
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "emailuser",
            Email = "emailuser@iot.com",
            PasswordHash = "hash_email",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };
        await _repository.AddAsync(user);

        // Act - Rechercher par email
        var result = await _repository.GetByEmailAsync("emailuser@iot.com");

        // Assert - Vérifier que l'utilisateur est trouvé
        Assert.NotNull(result);
        Assert.Equal("emailuser@iot.com", result.Email);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerTousLesUtilisateurs()
    {
        // Arrange - Créer et ajouter plusieurs utilisateurs
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Username = "alice",
            Email = "alice@iot.com",
            PasswordHash = "hash_alice",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Username = "bob",
            Email = "bob@iot.com",
            PasswordHash = "hash_bob",
            Role = RoleUtilisateur.Administrateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };

        var user3 = new User
        {
            Id = Guid.NewGuid(),
            Username = "charlie",
            Email = "charlie@iot.com",
            PasswordHash = "hash_charlie",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = false
        };

        await _repository.AddAsync(user1);
        await _repository.AddAsync(user2);
        await _repository.AddAsync(user3);

        // Act - Récupérer tous les utilisateurs
        var result = await _repository.GetAllAsync();
        var userList = result.ToList();

        // Assert - Vérifier que tous les utilisateurs sont retournés et triés par Username
        Assert.Equal(3, userList.Count);
        Assert.Equal("alice", userList[0].Username);
        Assert.Equal("bob", userList[1].Username);
        Assert.Equal("charlie", userList[2].Username);
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourUtilisateur()
    {
        // Arrange - Créer et ajouter un utilisateur
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "updateuser",
            Email = "old@iot.com",
            PasswordHash = "hash_old",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };
        await _repository.AddAsync(user);

        // Modifier l'email de l'utilisateur
        user.Email = "new@iot.com";

        // Act - Mettre à jour l'utilisateur
        await _repository.UpdateAsync(user);

        // Assert - Vérifier que la modification est bien persistée
        var updatedUser = await _repository.GetByIdAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("new@iot.com", updatedUser.Email);
        Assert.Equal("updateuser", updatedUser.Username); // Username inchangé
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerUtilisateur()
    {
        // Arrange - Créer et ajouter un utilisateur
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "deleteuser",
            Email = "delete@iot.com",
            PasswordHash = "hash_delete",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };
        await _repository.AddAsync(user);

        // Act - Supprimer l'utilisateur
        await _repository.DeleteAsync(user.Id);

        // Assert - Vérifier que l'utilisateur n'existe plus
        var deletedUser = await _repository.GetByIdAsync(user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandUtilisateurExiste()
    {
        // Arrange - Créer et ajouter un utilisateur
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "existuser",
            Email = "exist@iot.com",
            PasswordHash = "hash_exist",
            Role = RoleUtilisateur.Utilisateur,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };
        await _repository.AddAsync(user);

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(user.Id);

        // Assert - L'utilisateur doit exister
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerFalse_QuandUtilisateurNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(nonExistentId);

        // Assert - L'utilisateur ne doit pas exister
        Assert.False(exists);
    }

    /// <summary>
    /// Nettoyage après chaque test - supprime la base de données InMemory.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
