using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour le ActionneurRepository.
/// Utilise InMemoryDatabase pour tester les opérations CRUD sans dépendance à une vraie base de données.
/// Teste également le eager loading et le pattern Table Per Hierarchy.
/// </summary>
public class ActionneurRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IActionneurRepository _repository;
    private readonly string _databaseName;

    public ActionneurRepositoryTests()
    {
        // Crée une base de données InMemory avec un nom unique par test (évite les conflits)
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new ActionneurRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterActionneur_AvecGenerationAutomatique()
    {
        // Arrange - Créer une localisation
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salon",
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        await _context.SaveChangesAsync();

        // Préparer un actionneur sans Id, DateCreation ni DateInstallation
        var actionneur = new Actionneur
        {
            Nom = "Ampoule Salon Principale",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            UrlDevice = "http://192.168.1.20/light"
        };

        // Act - Ajouter l'actionneur
        var result = await _repository.AddAsync(actionneur);

        // Assert - Vérifier que l'Id, DateCreation et DateInstallation ont été générés
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.DateCreation);
        Assert.NotEqual(default(DateTime), result.DateInstallation);
        Assert.Equal("Ampoule Salon Principale", result.Nom);
        Assert.Equal(TypeActionneur.AmpouleVariometre, result.TypeActionneur);
        Assert.Equal(CanalCommunication.SignalR, result.CanalCommunication);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerActionneur_AvecEagerLoading()
    {
        // Arrange - Créer une localisation et un actionneur
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Cuisine",
            DateCreation = DateTime.UtcNow
        };
        var actionneur = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ventilateur Cuisine",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.Moteur,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.Add(actionneur);
        await _context.SaveChangesAsync();

        // Act - Récupérer l'actionneur par son Id
        var result = await _repository.GetByIdAsync(actionneur.Id);

        // Assert - Vérifier que l'actionneur est trouvé avec eager loading
        Assert.NotNull(result);
        Assert.Equal(actionneur.Id, result.Id);
        Assert.Equal("Ventilateur Cuisine", result.Nom);

        // Vérifier le eager loading de Localisation
        Assert.NotNull(result.Localisation);
        Assert.Equal("Cuisine", result.Localisation.Nom);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Tenter de récupérer un actionneur inexistant
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert - Vérifier que le résultat est null
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerTousLesActionneurs_TriesParNom()
    {
        // Arrange - Créer une localisation et plusieurs actionneurs
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Bureau",
            DateCreation = DateTime.UtcNow
        };

        var actionneur1 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Lampe Zéro",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var actionneur2 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Lampe Alpha",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.AddRange(actionneur1, actionneur2);
        await _context.SaveChangesAsync();

        // Act - Récupérer tous les actionneurs
        var result = await _repository.GetAllAsync();
        var actionneursList = result.ToList();

        // Assert - Vérifier le tri par nom
        Assert.Equal(2, actionneursList.Count);
        Assert.Equal("Lampe Alpha", actionneursList[0].Nom); // Alphabétiquement premier
        Assert.Equal("Lampe Zéro", actionneursList[1].Nom);

        // Vérifier le eager loading
        Assert.NotNull(actionneursList[0].Localisation);
    }

    [Fact]
    public async Task GetByLocalisationAsync_DevraitRetournerSeulementActionneursDeLocalisation()
    {
        // Arrange - Créer deux localisations et des actionneurs dans chacune
        var salon = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salon",
            DateCreation = DateTime.UtcNow
        };
        var chambre = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Chambre",
            DateCreation = DateTime.UtcNow
        };

        var actionneurSalon1 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Salon 1",
            LocalisationId = salon.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var actionneurSalon2 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Salon 2",
            LocalisationId = salon.Id,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var actionneurChambre = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Chambre",
            LocalisationId = chambre.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.AddRange(salon, chambre);
        _context.Actionneurs.AddRange(actionneurSalon1, actionneurSalon2, actionneurChambre);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les actionneurs du salon
        var result = await _repository.GetByLocalisationAsync(salon.Id);
        var actionneursList = result.ToList();

        // Assert - Vérifier que seuls les actionneurs du salon sont retournés
        Assert.Equal(2, actionneursList.Count);
        Assert.All(actionneursList, a => Assert.Equal(salon.Id, a.LocalisationId));
        Assert.Contains(actionneursList, a => a.Nom == "Ampoule Salon 1");
        Assert.Contains(actionneursList, a => a.Nom == "Ampoule Salon 2");
    }

    [Fact]
    public async Task GetByTypeAsync_AmpouleSimple_DevraitRetournerSeulementAmpoulesSimples()
    {
        // Arrange - Créer des actionneurs de différents types
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Maison",
            DateCreation = DateTime.UtcNow
        };

        var ampouleSimple = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Simple",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var ampouleVariometre = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Variomètre",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.AddRange(ampouleSimple, ampouleVariometre);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les ampoules simples
        var result = await _repository.GetByTypeAsync(TypeActionneur.AmpouleSimple);
        var actionneursList = result.ToList();

        // Assert - Vérifier que seules les ampoules simples sont retournées
        Assert.Single(actionneursList);
        Assert.Equal(TypeActionneur.AmpouleSimple, actionneursList[0].TypeActionneur);
        Assert.Equal("Ampoule Simple", actionneursList[0].Nom);
    }

    [Fact]
    public async Task GetByTypeAsync_AmpouleVariometre_DevraitRetournerSeulementAmpoulesVariometre()
    {
        // Arrange - Créer des actionneurs de différents types
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Appartement",
            DateCreation = DateTime.UtcNow
        };

        var ampouleSimple = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Simple",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var ampouleVario1 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Variomètre 1",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var ampouleVario2 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Variomètre 2",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.AddRange(ampouleSimple, ampouleVario1, ampouleVario2);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les ampoules variomètre
        var result = await _repository.GetByTypeAsync(TypeActionneur.AmpouleVariometre);
        var actionneursList = result.ToList();

        // Assert - Vérifier que seules les ampoules variomètre sont retournées
        Assert.Equal(2, actionneursList.Count);
        Assert.All(actionneursList, a => Assert.Equal(TypeActionneur.AmpouleVariometre, a.TypeActionneur));
    }

    [Fact]
    public async Task GetByTypeAsync_Moteur_DevraitRetournerSeulementMoteurs()
    {
        // Arrange - Créer des actionneurs de différents types
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Atelier",
            DateCreation = DateTime.UtcNow
        };

        var moteur = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Moteur Ventilateur",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.Moteur,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var ampoule = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Atelier",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.AddRange(moteur, ampoule);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les moteurs
        var result = await _repository.GetByTypeAsync(TypeActionneur.Moteur);
        var actionneursList = result.ToList();

        // Assert - Vérifier que seuls les moteurs sont retournés
        Assert.Single(actionneursList);
        Assert.Equal(TypeActionneur.Moteur, actionneursList[0].TypeActionneur);
        Assert.Equal("Moteur Ventilateur", actionneursList[0].Nom);
    }

    [Fact]
    public async Task GetActifsAsync_DevraitRetournerSeulementActionneursActifs()
    {
        // Arrange - Créer des actionneurs actifs et inactifs
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Entrepôt",
            DateCreation = DateTime.UtcNow
        };

        var actionneurActif1 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Actionneur Actif 1",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var actionneurActif2 = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Actionneur Actif 2",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.Moteur,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var actionneurInactif = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Actionneur Inactif",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = false, // INACTIF
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.AddRange(actionneurActif1, actionneurActif2, actionneurInactif);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les actionneurs actifs
        var result = await _repository.GetActifsAsync();
        var actionneursList = result.ToList();

        // Assert - Vérifier que seuls les actionneurs actifs sont retournés
        Assert.Equal(2, actionneursList.Count);
        Assert.All(actionneursList, a => Assert.True(a.EstActif));
        Assert.DoesNotContain(actionneursList, a => a.Nom == "Actionneur Inactif");
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourActionneur()
    {
        // Arrange - Créer et ajouter un actionneur
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Garage",
            DateCreation = DateTime.UtcNow
        };
        var actionneur = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ancien Nom",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            UrlDevice = "http://old-url.com",
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.Add(actionneur);
        await _context.SaveChangesAsync();

        // Modifier le nom et l'URL
        actionneur.Nom = "Nouveau Nom";
        actionneur.UrlDevice = "http://new-url.com";

        // Act - Mettre à jour l'actionneur
        await _repository.UpdateAsync(actionneur);

        // Assert - Vérifier que la modification est persistée
        var updatedActionneur = await _repository.GetByIdAsync(actionneur.Id);
        Assert.NotNull(updatedActionneur);
        Assert.Equal("Nouveau Nom", updatedActionneur.Nom);
        Assert.Equal("http://new-url.com", updatedActionneur.UrlDevice);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerActionneur()
    {
        // Arrange - Créer et ajouter un actionneur
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Cave",
            DateCreation = DateTime.UtcNow
        };
        var actionneur = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Actionneur à supprimer",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.Moteur,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.Add(actionneur);
        await _context.SaveChangesAsync();

        // Act - Supprimer l'actionneur
        await _repository.DeleteAsync(actionneur.Id);

        // Assert - Vérifier que l'actionneur n'existe plus
        var deletedActionneur = await _repository.GetByIdAsync(actionneur.Id);
        Assert.Null(deletedActionneur);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandActionneurExiste()
    {
        // Arrange - Créer et ajouter un actionneur
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Grenier",
            DateCreation = DateTime.UtcNow
        };
        var actionneur = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Actionneur Existant",
            LocalisationId = localisation.Id,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.Actionneurs.Add(actionneur);
        await _context.SaveChangesAsync();

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(actionneur.Id);

        // Assert - L'actionneur doit exister
        Assert.True(exists);
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
