using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le SystemePartenaireRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// </summary>
public class SystemePartenaireRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISystemePartenaireRepository _repository;
    private readonly string _databaseName;

    // IDs des systèmes partenaires seedés pour référence dans les tests
    private readonly Guid _partenaireAId;
    private readonly Guid _partenaireBId;
    private readonly Guid _partenaireCId;
    private readonly Guid _partenaireDId;

    public SystemePartenaireRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new SystemePartenaireRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie
        _partenaireAId = Guid.NewGuid();
        _partenaireBId = Guid.NewGuid();
        _partenaireCId = Guid.NewGuid();
        _partenaireDId = Guid.NewGuid();

        // Système 1 : Partenaire A - APPELANT uniquement (lui → nous)
        var partenaireA = new SystemePartenaire
        {
            Id = _partenaireAId,
            Nom = "Partenaire A",
            UrlBase = "https://partenaire-a.com",
            UsernameAcces = "partenaire_a_user",
            PasswordHashAcces = "hash_bcrypt_partenaire_a",
            UsernameAppel = null,
            PasswordChiffre = null,
            EstAppelant = true,
            EstAppele = false,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-30)
        };

        // Système 2 : Partenaire B - APPELÉ uniquement (nous → lui)
        var partenaireB = new SystemePartenaire
        {
            Id = _partenaireBId,
            Nom = "Partenaire B",
            UrlBase = "https://partenaire-b.com",
            UsernameAcces = null,
            PasswordHashAcces = null,
            UsernameAppel = "notre_username_pour_b",
            PasswordChiffre = "password_chiffre_base64_pour_b",
            EstAppelant = false,
            EstAppele = true,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-20)
        };

        // Système 3 : Partenaire C - BIDIRECTIONNEL (les deux modes)
        var partenaireC = new SystemePartenaire
        {
            Id = _partenaireCId,
            Nom = "Partenaire C",
            UrlBase = "https://partenaire-c.com",
            UsernameAcces = "partenaire_c_user",
            PasswordHashAcces = "hash_bcrypt_partenaire_c",
            UsernameAppel = "notre_username_pour_c",
            PasswordChiffre = "password_chiffre_base64_pour_c",
            EstAppelant = true,
            EstAppele = true,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        // Système 4 : Partenaire D - INACTIF
        var partenaireD = new SystemePartenaire
        {
            Id = _partenaireDId,
            Nom = "Partenaire D",
            UrlBase = "https://partenaire-d.com",
            UsernameAcces = "partenaire_d_user",
            PasswordHashAcces = "hash_bcrypt_partenaire_d",
            UsernameAppel = null,
            PasswordChiffre = null,
            EstAppelant = true,
            EstAppele = false,
            EstActif = false, // INACTIF
            DateCreation = DateTime.UtcNow.AddDays(-5)
        };

        _context.SystemesPartenaires.AddRange(partenaireA, partenaireB, partenaireC, partenaireD);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer un nouveau système partenaire
        var nouveauSysteme = new SystemePartenaire
        {
            Nom = "Nouveau Partenaire",
            UrlBase = "https://nouveau-partenaire.com",
            UsernameAcces = "nouveau_user",
            PasswordHashAcces = "hash_nouveau",
            UsernameAppel = "notre_user_nouveau",
            PasswordChiffre = "pwd_chiffre_nouveau",
            EstAppelant = true,
            EstAppele = true,
            EstActif = true
        };

        // Act & Assert - CREATE
        var createdSysteme = await _repository.AddAsync(nouveauSysteme);
        Assert.NotEqual(Guid.Empty, createdSysteme.Id);
        Assert.Equal("Nouveau Partenaire", createdSysteme.Nom);

        // Act & Assert - READ
        var readSysteme = await _repository.GetByIdAsync(createdSysteme.Id);
        Assert.NotNull(readSysteme);
        Assert.Equal("Nouveau Partenaire", readSysteme.Nom);
        Assert.Equal("https://nouveau-partenaire.com", readSysteme.UrlBase);

        // Act & Assert - UPDATE
        readSysteme.UrlBase = "https://nouveau-partenaire-updated.com";
        readSysteme.EstActif = false;
        await _repository.UpdateAsync(readSysteme);
        var updatedSysteme = await _repository.GetByIdAsync(createdSysteme.Id);
        Assert.NotNull(updatedSysteme);
        Assert.Equal("https://nouveau-partenaire-updated.com", updatedSysteme.UrlBase);
        Assert.False(updatedSysteme.EstActif);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdSysteme.Id);
        var deletedSysteme = await _repository.GetByIdAsync(createdSysteme.Id);
        Assert.Null(deletedSysteme);

        // Vérifier que les systèmes seedés n'ont pas été affectés
        var allSystemes = await _repository.GetAllAsync();
        Assert.Equal(4, allSystemes.Count()); // A, B, C, D toujours présents
    }

    [Fact]
    public async Task GetAppelantsAsync_DevraitExclureInactifs()
    {
        // Arrange - Les systèmes A et C sont appelants actifs, D est appelant mais inactif

        // Act - Récupérer les appelants actifs
        var appelants = await _repository.GetAppelantsAsync();
        var appelantsList = appelants.ToList();

        // Assert - Vérifier que seuls A et C sont retournés (D exclu car inactif)
        Assert.Equal(2, appelantsList.Count);
        Assert.Contains(appelantsList, s => s.Id == _partenaireAId);
        Assert.Contains(appelantsList, s => s.Id == _partenaireCId);
        Assert.DoesNotContain(appelantsList, s => s.Id == _partenaireDId); // D exclu car inactif

        // Vérifier que tous sont appelants et actifs
        Assert.All(appelantsList, s =>
        {
            Assert.True(s.EstAppelant);
            Assert.True(s.EstActif);
        });
    }

    [Fact]
    public async Task GetAppelesAsync_DevraitRetournerSeulementAppelesActifs()
    {
        // Arrange - Les systèmes B et C sont appelés actifs

        // Act - Récupérer les appelés actifs
        var appeles = await _repository.GetAppelesAsync();
        var appelesList = appeles.ToList();

        // Assert - Vérifier que seuls B et C sont retournés
        Assert.Equal(2, appelesList.Count);
        Assert.Contains(appelesList, s => s.Id == _partenaireBId);
        Assert.Contains(appelesList, s => s.Id == _partenaireCId);

        // Vérifier que tous sont appelés et actifs
        Assert.All(appelesList, s =>
        {
            Assert.True(s.EstAppele);
            Assert.True(s.EstActif);
        });

        // Vérifier le tri par nom
        Assert.Equal("Partenaire B", appelesList[0].Nom);
        Assert.Equal("Partenaire C", appelesList[1].Nom);
    }

    [Fact]
    public async Task GetByUsernameAccesAsync_DevraitTrouverPartenaire()
    {
        // Arrange - Le partenaire A a le username "partenaire_a_user"

        // Act - Rechercher par username d'accès
        var systeme = await _repository.GetByUsernameAccesAsync("partenaire_a_user");

        // Assert - Vérifier que le partenaire A est trouvé
        Assert.NotNull(systeme);
        Assert.Equal(_partenaireAId, systeme.Id);
        Assert.Equal("Partenaire A", systeme.Nom);
        Assert.Equal("partenaire_a_user", systeme.UsernameAcces);
        Assert.Equal("hash_bcrypt_partenaire_a", systeme.PasswordHashAcces);
    }

    [Fact]
    public async Task AddAsync_AvecLesDeuxModesAuthentification_DevraitReussir()
    {
        // Arrange - Créer un système bidirectionnel avec les 4 credentials
        var systemeBidirectionnel = new SystemePartenaire
        {
            Nom = "Système Bidirectionnel",
            UrlBase = "https://bidirectionnel.com",
            // Credentials pour être appelé (lui → nous)
            UsernameAcces = "bidirectionnel_acces_user",
            PasswordHashAcces = "hash_bidirectionnel_acces",
            // Credentials pour appeler (nous → lui)
            UsernameAppel = "bidirectionnel_appel_user",
            PasswordChiffre = "pwd_chiffre_bidirectionnel",
            EstAppelant = true,
            EstAppele = true,
            EstActif = true
        };

        // Act - Ajouter le système
        var result = await _repository.AddAsync(systemeBidirectionnel);

        // Assert - Vérifier que tous les credentials sont présents
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Système Bidirectionnel", result.Nom);

        // Vérifier les credentials d'accès (lui → nous)
        Assert.Equal("bidirectionnel_acces_user", result.UsernameAcces);
        Assert.Equal("hash_bidirectionnel_acces", result.PasswordHashAcces);

        // Vérifier les credentials d'appel (nous → lui)
        Assert.Equal("bidirectionnel_appel_user", result.UsernameAppel);
        Assert.Equal("pwd_chiffre_bidirectionnel", result.PasswordChiffre);

        // Vérifier les flags
        Assert.True(result.EstAppelant);
        Assert.True(result.EstAppele);
        Assert.True(result.EstActif);

        // Vérifier la récupération
        var retrieved = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("bidirectionnel_acces_user", retrieved.UsernameAcces);
        Assert.Equal("bidirectionnel_appel_user", retrieved.UsernameAppel);
    }

    [Fact]
    public async Task UpdateAsync_DevraitModifierFlags_EstAppelantEtEstAppele()
    {
        // Arrange - Récupérer le partenaire A (actuellement EstAppelant=true, EstAppele=false)
        var partenaireA = await _repository.GetByIdAsync(_partenaireAId);
        Assert.NotNull(partenaireA);
        Assert.True(partenaireA.EstAppelant);
        Assert.False(partenaireA.EstAppele);

        // Act - Modifier les flags (devenir bidirectionnel)
        partenaireA.EstAppele = true;
        partenaireA.UsernameAppel = "nouveau_username_appel";
        partenaireA.PasswordChiffre = "nouveau_pwd_chiffre";
        await _repository.UpdateAsync(partenaireA);

        // Assert - Vérifier que les modifications sont persistées
        var partenaireModifie = await _repository.GetByIdAsync(_partenaireAId);
        Assert.NotNull(partenaireModifie);
        Assert.True(partenaireModifie.EstAppelant); // Toujours appelant
        Assert.True(partenaireModifie.EstAppele); // Maintenant aussi appelé
        Assert.Equal("nouveau_username_appel", partenaireModifie.UsernameAppel);
        Assert.Equal("nouveau_pwd_chiffre", partenaireModifie.PasswordChiffre);

        // Vérifier qu'il apparaît maintenant dans les deux listes
        var appelants = await _repository.GetAppelantsAsync();
        var appeles = await _repository.GetAppelesAsync();

        Assert.Contains(appelants, s => s.Id == _partenaireAId);
        Assert.Contains(appeles, s => s.Id == _partenaireAId);
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
