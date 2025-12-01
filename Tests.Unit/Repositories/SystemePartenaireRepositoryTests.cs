using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour le SystemePartenaireRepository.
/// Utilise InMemoryDatabase pour tester les opérations CRUD sans dépendance à une vraie base de données.
/// </summary>
public class SystemePartenaireRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISystemePartenaireRepository _repository;
    private readonly string _databaseName;

    public SystemePartenaireRepositoryTests()
    {
        // Crée une base de données InMemory avec un nom unique par test (évite les conflits)
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new SystemePartenaireRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterSystemePartenaire()
    {
        // Arrange - Préparer un système partenaire sans Id ni DateCreation
        var systeme = new SystemePartenaire
        {
            Nom = "Partenaire Test",
            UrlBase = "https://api.partenaire-test.com",
            UsernameAppel = "notre_username",
            PasswordChiffre = "password_chiffre_base64",
            EstAppelant = false,
            EstAppele = true,
            EstActif = true
        };

        // Act - Ajouter le système partenaire
        var result = await _repository.AddAsync(systeme);

        // Assert - Vérifier que l'Id et la DateCreation ont été générés
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.DateCreation);
        Assert.Equal("Partenaire Test", result.Nom);
        Assert.Equal("https://api.partenaire-test.com", result.UrlBase);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerSysteme_QuandExiste()
    {
        // Arrange - Créer et ajouter un système partenaire
        var systeme = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système IoT Ville",
            UrlBase = "https://iot.ville.fr",
            UsernameAcces = "ville_user",
            PasswordHashAcces = "hash_bcrypt_xxx",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(systeme);

        // Act - Récupérer le système par son Id
        var result = await _repository.GetByIdAsync(systeme.Id);

        // Assert - Vérifier que le système est trouvé
        Assert.NotNull(result);
        Assert.Equal(systeme.Id, result.Id);
        Assert.Equal("Système IoT Ville", result.Nom);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Tenter de récupérer un système inexistant
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert - Vérifier que le résultat est null
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerTousLesSystemes()
    {
        // Arrange - Créer et ajouter plusieurs systèmes
        var systeme1 = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Partenaire A",
            UrlBase = "https://a.com",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        var systeme2 = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Partenaire B",
            UrlBase = "https://b.com",
            EstAppelant = false,
            EstAppele = true,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(systeme1);
        await _repository.AddAsync(systeme2);

        // Act - Récupérer tous les systèmes
        var result = await _repository.GetAllAsync();
        var systemesList = result.ToList();

        // Assert - Vérifier que tous les systèmes sont retournés et triés par Nom
        Assert.Equal(2, systemesList.Count);
        Assert.Equal("Partenaire A", systemesList[0].Nom);
        Assert.Equal("Partenaire B", systemesList[1].Nom);
    }

    [Fact]
    public async Task GetAppelantsAsync_DevraitRetournerSeulementAppelants()
    {
        // Arrange - Créer des systèmes avec différents flags
        var appelant = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Appelant",
            UrlBase = "https://appelant.com",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        var appele = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Appelé",
            UrlBase = "https://appele.com",
            EstAppelant = false,
            EstAppele = true,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        var inactif = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Inactif",
            UrlBase = "https://inactif.com",
            EstAppelant = true,
            EstAppele = false,
            EstActif = false, // Inactif
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(appelant);
        await _repository.AddAsync(appele);
        await _repository.AddAsync(inactif);

        // Act - Récupérer uniquement les appelants actifs
        var result = await _repository.GetAppelantsAsync();
        var systemesList = result.ToList();

        // Assert - Vérifier que seul le système appelant actif est retourné
        Assert.Single(systemesList);
        Assert.Equal("Système Appelant", systemesList[0].Nom);
        Assert.True(systemesList[0].EstAppelant);
        Assert.True(systemesList[0].EstActif);
    }

    [Fact]
    public async Task GetAppelesAsync_DevraitRetournerSeulementAppeles()
    {
        // Arrange - Créer des systèmes avec différents flags
        var appelant = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Appelant",
            UrlBase = "https://appelant.com",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        var appele = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Appelé",
            UrlBase = "https://appele.com",
            EstAppelant = false,
            EstAppele = true,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        var inactif = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Inactif",
            UrlBase = "https://inactif.com",
            EstAppelant = false,
            EstAppele = true,
            EstActif = false, // Inactif
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(appelant);
        await _repository.AddAsync(appele);
        await _repository.AddAsync(inactif);

        // Act - Récupérer uniquement les appelés actifs
        var result = await _repository.GetAppelesAsync();
        var systemesList = result.ToList();

        // Assert - Vérifier que seul le système appelé actif est retourné
        Assert.Single(systemesList);
        Assert.Equal("Système Appelé", systemesList[0].Nom);
        Assert.True(systemesList[0].EstAppele);
        Assert.True(systemesList[0].EstActif);
    }

    [Fact]
    public async Task GetActifsAsync_DevraitRetournerSeulementActifs()
    {
        // Arrange - Créer des systèmes actifs et inactifs
        var actif1 = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Actif 1",
            UrlBase = "https://actif1.com",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        var actif2 = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Actif 2",
            UrlBase = "https://actif2.com",
            EstAppelant = false,
            EstAppele = true,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        var inactif = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Inactif",
            UrlBase = "https://inactif.com",
            EstAppelant = true,
            EstAppele = true,
            EstActif = false,
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(actif1);
        await _repository.AddAsync(actif2);
        await _repository.AddAsync(inactif);

        // Act - Récupérer uniquement les systèmes actifs
        var result = await _repository.GetActifsAsync();
        var systemesList = result.ToList();

        // Assert - Vérifier que seuls les systèmes actifs sont retournés
        Assert.Equal(2, systemesList.Count);
        Assert.All(systemesList, s => Assert.True(s.EstActif));
    }

    [Fact]
    public async Task GetByUsernameAccesAsync_DevraitRetournerSysteme_QuandExiste()
    {
        // Arrange - Créer un système avec un username d'accès
        var systeme = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Auth",
            UrlBase = "https://auth.com",
            UsernameAcces = "partenaire_auth_user",
            PasswordHashAcces = "hash_secure",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(systeme);

        // Act - Rechercher par username d'accès
        var result = await _repository.GetByUsernameAccesAsync("partenaire_auth_user");

        // Assert - Vérifier que le système est trouvé
        Assert.NotNull(result);
        Assert.Equal("Système Auth", result.Nom);
        Assert.Equal("partenaire_auth_user", result.UsernameAcces);
    }

    [Fact]
    public async Task GetByUsernameAccesAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange - Username qui n'existe pas
        var usernameInexistant = "username_inexistant";

        // Act - Rechercher par username inexistant
        var result = await _repository.GetByUsernameAccesAsync(usernameInexistant);

        // Assert - Vérifier que le résultat est null
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourSysteme()
    {
        // Arrange - Créer et ajouter un système
        var systeme = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Ancien Nom",
            UrlBase = "https://old-url.com",
            EstAppelant = false,
            EstAppele = true,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(systeme);

        // Modifier l'URL
        systeme.UrlBase = "https://new-url.com";

        // Act - Mettre à jour le système
        await _repository.UpdateAsync(systeme);

        // Assert - Vérifier que la modification est persistée
        var updatedSysteme = await _repository.GetByIdAsync(systeme.Id);
        Assert.NotNull(updatedSysteme);
        Assert.Equal("https://new-url.com", updatedSysteme.UrlBase);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerSysteme()
    {
        // Arrange - Créer et ajouter un système
        var systeme = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système à supprimer",
            UrlBase = "https://delete.com",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(systeme);

        // Act - Supprimer le système
        await _repository.DeleteAsync(systeme.Id);

        // Assert - Vérifier que le système n'existe plus
        var deletedSysteme = await _repository.GetByIdAsync(systeme.Id);
        Assert.Null(deletedSysteme);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandSystemeExiste()
    {
        // Arrange - Créer et ajouter un système
        var systeme = new SystemePartenaire
        {
            Id = Guid.NewGuid(),
            Nom = "Système Existant",
            UrlBase = "https://exist.com",
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(systeme);

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(systeme.Id);

        // Assert - Le système doit exister
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerFalse_QuandSystemeNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(nonExistentId);

        // Assert - Le système ne doit pas exister
        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterCredentialsNull()
    {
        // Arrange - Créer un système avec tous les credentials null (optionnels)
        var systeme = new SystemePartenaire
        {
            Nom = "Système Sans Credentials",
            UrlBase = "https://no-creds.com",
            UsernameAppel = null,
            PasswordChiffre = null,
            UsernameAcces = null,
            PasswordHashAcces = null,
            EstAppelant = false,
            EstAppele = false,
            EstActif = true
        };

        // Act - Ajouter le système
        var result = await _repository.AddAsync(systeme);

        // Assert - Vérifier que l'ajout fonctionne avec credentials null
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Système Sans Credentials", result.Nom);
        Assert.Null(result.UsernameAppel);
        Assert.Null(result.PasswordChiffre);
        Assert.Null(result.UsernameAcces);
        Assert.Null(result.PasswordHashAcces);

        // Vérifier la récupération
        var retrieved = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(retrieved);
        Assert.Null(retrieved.UsernameAppel);
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
