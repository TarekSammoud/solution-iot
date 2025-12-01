using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour EtatActionneurRepository.
/// Teste toutes les opérations CRUD et les méthodes de requête spécifiques.
/// Utilise InMemoryDatabase avec nom unique par test pour isolation complète.
/// </summary>
public class EtatActionneurRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IEtatActionneurRepository _repository;
    private readonly string _databaseName;

    public EtatActionneurRepositoryTests()
    {
        // Nom de base de données unique pour chaque test (isolation)
        _databaseName = $"TestDb_{Guid.NewGuid()}";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new EtatActionneurRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Crée une localisation de test prérequise pour créer un actionneur.
    /// </summary>
    private Localisation CreerLocalisationTest()
    {
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salon",
            Description = "Pièce de test",
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.SaveChanges();
        return localisation;
    }

    /// <summary>
    /// Crée un actionneur de test (AmpouleSimple par défaut).
    /// </summary>
    private Actionneur CreerActionneurTest(Localisation localisation, TypeActionneur type = TypeActionneur.AmpouleSimple)
    {
        var actionneur = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Actionneur Test",
            LocalisationId = localisation.Id,
            EstActif = true,
            DateInstallation = DateTime.UtcNow,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPush,
            TypeActionneur = type
        };
        _context.Actionneurs.Add(actionneur);
        _context.SaveChanges();
        return actionneur;
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterEtatActionneur()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(etat);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id); // Id généré automatiquement
        Assert.Equal(actionneur.Id, result.ActionneurId);
        Assert.True(result.EstActif);
        Assert.Equal(100, result.Pourcentage);
    }

    [Fact]
    public async Task AddAsync_DevraitDefinirDerniereModificationAutomatiquement()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 75
            // DerniereModification non définie volontairement
        };

        var avant = DateTime.UtcNow;

        // Act
        var result = await _repository.AddAsync(etat);

        var apres = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(default, result.DerniereModification);
        Assert.True(result.DerniereModification >= avant && result.DerniereModification <= apres);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerEtat_AvecEagerLoading()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            Id = Guid.NewGuid(),
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 50,
            DerniereModification = DateTime.UtcNow
        };
        await _repository.AddAsync(etat);

        // Act
        var result = await _repository.GetByIdAsync(etat.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(etat.Id, result.Id);
        Assert.NotNull(result.Actionneur); // Eager loading vérifié
        Assert.Equal(actionneur.Id, result.Actionneur.Id);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange
        var idInexistant = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(idInexistant);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByActionneurAsync_DevraitRetournerEtat_QuandExiste()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow
        };
        await _repository.AddAsync(etat);

        // Act
        var result = await _repository.GetByActionneurAsync(actionneur.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(actionneur.Id, result.ActionneurId);
        Assert.NotNull(result.Actionneur); // Eager loading
    }

    [Fact]
    public async Task GetByActionneurAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);
        // Pas d'état créé pour cet actionneur

        // Act
        var result = await _repository.GetByActionneurAsync(actionneur.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerTousLesEtats()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur1 = CreerActionneurTest(localisation);
        var actionneur2 = CreerActionneurTest(localisation);
        var actionneur3 = CreerActionneurTest(localisation);

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur1.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow.AddHours(-1)
        });

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur2.Id,
            EstActif = false,
            Pourcentage = 0,
            DerniereModification = DateTime.UtcNow
        });

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur3.Id,
            EstActif = true,
            Pourcentage = 75,
            DerniereModification = DateTime.UtcNow.AddMinutes(-30)
        });

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_DevraitTrierParDerniereModificationDescendant()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur1 = CreerActionneurTest(localisation);
        var actionneur2 = CreerActionneurTest(localisation);
        var actionneur3 = CreerActionneurTest(localisation);

        var dateAncienne = DateTime.UtcNow.AddHours(-2);
        var dateMoyenne = DateTime.UtcNow.AddHours(-1);
        var dateRecente = DateTime.UtcNow;

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur1.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = dateMoyenne
        });

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur2.Id,
            EstActif = false,
            Pourcentage = 0,
            DerniereModification = dateRecente
        });

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur3.Id,
            EstActif = true,
            Pourcentage = 50,
            DerniereModification = dateAncienne
        });

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(actionneur2.Id, result[0].ActionneurId); // Plus récent en premier
        Assert.Equal(actionneur1.Id, result[1].ActionneurId);
        Assert.Equal(actionneur3.Id, result[2].ActionneurId); // Plus ancien en dernier
    }

    [Fact]
    public async Task GetActifsAsync_DevraitRetournerSeulementEtatsActifs()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur1 = CreerActionneurTest(localisation);
        var actionneur2 = CreerActionneurTest(localisation);
        var actionneur3 = CreerActionneurTest(localisation);

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur1.Id,
            EstActif = true, // Actif
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow
        });

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur2.Id,
            EstActif = false, // Inactif
            Pourcentage = 0,
            DerniereModification = DateTime.UtcNow
        });

        await _repository.AddAsync(new EtatActionneur
        {
            ActionneurId = actionneur3.Id,
            EstActif = true, // Actif
            Pourcentage = 75,
            DerniereModification = DateTime.UtcNow
        });

        // Act
        var result = await _repository.GetActifsAsync();

        // Assert
        Assert.Equal(2, result.Count()); // Seulement les actifs
        Assert.All(result, e => Assert.True(e.EstActif));
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourDerniereModification()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 50,
            DerniereModification = DateTime.UtcNow.AddHours(-1) // Date ancienne
        };
        await _repository.AddAsync(etat);

        var ancienneDateModification = etat.DerniereModification;

        // Attendre un peu pour s'assurer que la date change
        await Task.Delay(10);

        // Act
        etat.Pourcentage = 75;
        await _repository.UpdateAsync(etat);

        // Assert
        var etatMisAJour = await _repository.GetByIdAsync(etat.Id);
        Assert.NotNull(etatMisAJour);
        Assert.True(etatMisAJour.DerniereModification > ancienneDateModification);
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourEtatEtPourcentage()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow
        };
        await _repository.AddAsync(etat);

        // Act
        etat.EstActif = false;
        etat.Pourcentage = 0;
        await _repository.UpdateAsync(etat);

        // Assert
        var etatMisAJour = await _repository.GetByIdAsync(etat.Id);
        Assert.NotNull(etatMisAJour);
        Assert.False(etatMisAJour.EstActif);
        Assert.Equal(0, etatMisAJour.Pourcentage);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerEtat()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow
        };
        await _repository.AddAsync(etat);

        // Act
        await _repository.DeleteAsync(etat.Id);

        // Assert
        var etatSupprime = await _repository.GetByIdAsync(etat.Id);
        Assert.Null(etatSupprime);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandEtatExiste()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow
        };
        await _repository.AddAsync(etat);

        // Act
        var existe = await _repository.ExistsAsync(etat.Id);

        // Assert
        Assert.True(existe);
    }

    [Fact]
    public async Task ExistsByActionneurAsync_DevraitRetournerTrue_QuandEtatExiste()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);

        var etat = new EtatActionneur
        {
            ActionneurId = actionneur.Id,
            EstActif = true,
            Pourcentage = 100,
            DerniereModification = DateTime.UtcNow
        };
        await _repository.AddAsync(etat);

        // Act
        var existe = await _repository.ExistsByActionneurAsync(actionneur.Id);

        // Assert
        Assert.True(existe);
    }

    [Fact]
    public async Task ExistsByActionneurAsync_DevraitRetournerFalse_QuandEtatNExistePas()
    {
        // Arrange
        var localisation = CreerLocalisationTest();
        var actionneur = CreerActionneurTest(localisation);
        // Pas d'état créé

        // Act
        var existe = await _repository.ExistsByActionneurAsync(actionneur.Id);

        // Assert
        Assert.False(existe);
    }
}
