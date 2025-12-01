using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le ActionneurRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// Teste le Table Per Hierarchy et le eager loading.
/// </summary>
public class ActionneurRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IActionneurRepository _repository;
    private readonly string _databaseName;

    // IDs des entités seedées pour référence dans les tests
    private readonly Guid _salonId;
    private readonly Guid _cuisineId;
    private readonly Guid _ampouleSimpleSalonId;
    private readonly Guid _ampouleVarioSalonId;
    private readonly Guid _moteurCuisineId;
    private readonly Guid _ampouleSimpleCuisineId;

    public ActionneurRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new ActionneurRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie

        // IDs pour les localisations
        _salonId = Guid.NewGuid();
        _cuisineId = Guid.NewGuid();

        // IDs pour les actionneurs
        _ampouleSimpleSalonId = Guid.NewGuid();
        _ampouleVarioSalonId = Guid.NewGuid();
        _moteurCuisineId = Guid.NewGuid();
        _ampouleSimpleCuisineId = Guid.NewGuid();

        // Seed Localisations
        var salon = new Localisation
        {
            Id = _salonId,
            Nom = "Salon",
            Description = "Pièce principale",
            DateCreation = DateTime.UtcNow.AddDays(-30)
        };

        var cuisine = new Localisation
        {
            Id = _cuisineId,
            Nom = "Cuisine",
            Description = "Espace cuisine",
            DateCreation = DateTime.UtcNow.AddDays(-25)
        };

        // Seed Actionneurs
        var ampouleSimpleSalon = new Actionneur
        {
            Id = _ampouleSimpleSalonId,
            Nom = "Ampoule LED Salon",
            LocalisationId = _salonId,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            UrlDevice = "http://192.168.1.30/light",
            CredentialsDevice = "encrypted_token_1",
            DateCreation = DateTime.UtcNow.AddDays(-15),
            DateInstallation = DateTime.UtcNow.AddDays(-14)
        };

        var ampouleVarioSalon = new Actionneur
        {
            Id = _ampouleVarioSalonId,
            Nom = "Ampoule Variateur Salon",
            LocalisationId = _salonId,
            TypeActionneur = TypeActionneur.AmpouleVariometre,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            UrlDevice = null,
            CredentialsDevice = null,
            DateCreation = DateTime.UtcNow.AddDays(-10),
            DateInstallation = DateTime.UtcNow.AddDays(-9)
        };

        var moteurCuisine = new Actionneur
        {
            Id = _moteurCuisineId,
            Nom = "Ventilateur Cuisine",
            LocalisationId = _cuisineId,
            TypeActionneur = TypeActionneur.Moteur,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            UrlDevice = null,
            CredentialsDevice = null,
            DateCreation = DateTime.UtcNow.AddDays(-5),
            DateInstallation = DateTime.UtcNow.AddDays(-4)
        };

        var ampouleSimpleCuisine = new Actionneur
        {
            Id = _ampouleSimpleCuisineId,
            Nom = "Ampoule Cuisine",
            LocalisationId = _cuisineId,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = false, // INACTIVE
            CanalCommunication = CanalCommunication.HttpPush,
            UrlDevice = "http://192.168.1.40/light",
            CredentialsDevice = null,
            DateCreation = DateTime.UtcNow.AddDays(-2),
            DateInstallation = DateTime.UtcNow.AddDays(-1)
        };

        _context.Localisations.AddRange(salon, cuisine);
        _context.Actionneurs.AddRange(ampouleSimpleSalon, ampouleVarioSalon, moteurCuisine, ampouleSimpleCuisine);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer un nouvel actionneur
        var nouvelActionneur = new Actionneur
        {
            Nom = "Nouvel Actionneur Test",
            LocalisationId = _salonId,
            TypeActionneur = TypeActionneur.Moteur,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };

        // Act & Assert - CREATE
        var createdActionneur = await _repository.AddAsync(nouvelActionneur);
        Assert.NotEqual(Guid.Empty, createdActionneur.Id);
        Assert.Equal("Nouvel Actionneur Test", createdActionneur.Nom);

        // Act & Assert - READ with eager loading
        var readActionneur = await _repository.GetByIdAsync(createdActionneur.Id);
        Assert.NotNull(readActionneur);
        Assert.Equal("Nouvel Actionneur Test", readActionneur.Nom);
        Assert.NotNull(readActionneur.Localisation);
        Assert.Equal("Salon", readActionneur.Localisation.Nom);

        // Act & Assert - UPDATE
        readActionneur.Nom = "Actionneur Modifié";
        readActionneur.EstActif = false;
        await _repository.UpdateAsync(readActionneur);
        var updatedActionneur = await _repository.GetByIdAsync(createdActionneur.Id);
        Assert.NotNull(updatedActionneur);
        Assert.Equal("Actionneur Modifié", updatedActionneur.Nom);
        Assert.False(updatedActionneur.EstActif);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdActionneur.Id);
        var deletedActionneur = await _repository.GetByIdAsync(createdActionneur.Id);
        Assert.Null(deletedActionneur);

        // Vérifier que les actionneurs seedés n'ont pas été affectés
        var allActionneurs = await _repository.GetAllAsync();
        Assert.Equal(4, allActionneurs.Count()); // 4 actionneurs seedés toujours présents
    }

    [Fact]
    public async Task GetByLocalisationAsync_Salon_DevraitRetourner2Actionneurs()
    {
        // Arrange - Les actionneurs sont déjà seedés (2 dans le salon)

        // Act - Récupérer les actionneurs du salon
        var actionneurs = await _repository.GetByLocalisationAsync(_salonId);
        var actionneursList = actionneurs.ToList();

        // Assert - Vérifier que 2 actionneurs du salon sont retournés
        Assert.Equal(2, actionneursList.Count);
        Assert.All(actionneursList, a => Assert.Equal(_salonId, a.LocalisationId));

        // Vérifier le tri par nom
        Assert.Equal("Ampoule LED Salon", actionneursList[0].Nom);
        Assert.Equal("Ampoule Variateur Salon", actionneursList[1].Nom);

        // Vérifier le eager loading
        Assert.All(actionneursList, a =>
        {
            Assert.NotNull(a.Localisation);
            Assert.Equal("Salon", a.Localisation.Nom);
        });
    }

    [Fact]
    public async Task GetByTypeAsync_DevraitFiltrerParType()
    {
        // Act - Récupérer les ampoules simples
        var ampoulesSimples = await _repository.GetByTypeAsync(TypeActionneur.AmpouleSimple);
        var ampoulesSimplesList = ampoulesSimples.ToList();

        // Assert - Vérifier que 2 ampoules simples sont retournées
        Assert.Equal(2, ampoulesSimplesList.Count);
        Assert.All(ampoulesSimplesList, a => Assert.Equal(TypeActionneur.AmpouleSimple, a.TypeActionneur));

        // Act - Récupérer les ampoules variomètre
        var ampoulesVario = await _repository.GetByTypeAsync(TypeActionneur.AmpouleVariometre);
        var ampoulesVarioList = ampoulesVario.ToList();

        // Assert - Vérifier qu'1 ampoule variomètre est retournée
        Assert.Single(ampoulesVarioList);
        Assert.Equal(TypeActionneur.AmpouleVariometre, ampoulesVarioList[0].TypeActionneur);
        Assert.Equal("Ampoule Variateur Salon", ampoulesVarioList[0].Nom);

        // Act - Récupérer les moteurs
        var moteurs = await _repository.GetByTypeAsync(TypeActionneur.Moteur);
        var moteursList = moteurs.ToList();

        // Assert - Vérifier qu'1 moteur est retourné
        Assert.Single(moteursList);
        Assert.Equal(TypeActionneur.Moteur, moteursList[0].TypeActionneur);
        Assert.Equal("Ventilateur Cuisine", moteursList[0].Nom);
    }

    [Fact]
    public async Task GetActifsAsync_DevraitExclureInactifs()
    {
        // Arrange - Les actionneurs sont déjà seedés (3 actifs, 1 inactif)

        // Act - Récupérer uniquement les actionneurs actifs
        var actionneurs = await _repository.GetActifsAsync();
        var actionneursList = actionneurs.ToList();

        // Assert - Vérifier que seuls les 3 actionneurs actifs sont retournés
        Assert.Equal(3, actionneursList.Count);
        Assert.All(actionneursList, a => Assert.True(a.EstActif));

        // Vérifier que l'actionneur inactif n'est pas retourné
        Assert.DoesNotContain(actionneursList, a => a.Id == _ampouleSimpleCuisineId);

        // Vérifier le tri par nom
        Assert.Equal("Ampoule LED Salon", actionneursList[0].Nom);
        Assert.Equal("Ampoule Variateur Salon", actionneursList[1].Nom);
        Assert.Equal("Ventilateur Cuisine", actionneursList[2].Nom);
    }

    [Fact]
    public async Task EagerLoading_DevraitChargerLocalisation()
    {
        // Arrange - Récupérer un actionneur

        // Act - Récupérer l'actionneur avec eager loading
        var actionneur = await _repository.GetByIdAsync(_ampouleSimpleSalonId);

        // Assert - Vérifier que Localisation est chargée
        Assert.NotNull(actionneur);

        // Vérifier eager loading de Localisation
        Assert.NotNull(actionneur.Localisation);
        Assert.Equal("Salon", actionneur.Localisation.Nom);
        Assert.Equal("Pièce principale", actionneur.Localisation.Description);

        // Vérifier les propriétés de l'actionneur
        Assert.Equal("Ampoule LED Salon", actionneur.Nom);
        Assert.Equal(TypeActionneur.AmpouleSimple, actionneur.TypeActionneur);
        Assert.Equal(CanalCommunication.HttpPull, actionneur.CanalCommunication);
    }

    [Fact]
    public async Task TablePerHierarchy_DevraitStockerSondesEtActionneursMemeSTable()
    {
        // Arrange - Ajouter une sonde dans la même base
        var uniteCelsius = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        _context.UnitesMesures.Add(uniteCelsius);

        var sonde = new Sonde
        {
            Nom = "Sonde Test",
            LocalisationId = _salonId,
            UniteMesureId = uniteCelsius.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        // Act - Récupérer tous les actionneurs et tous les devices
        var actionneurs = await _repository.GetAllAsync();
        var allDevices = await _context.Devices.ToListAsync();

        // Assert - Vérifier le Table Per Hierarchy
        // Les actionneurs ne doivent contenir que des Actionneur (pas de Sonde)
        Assert.Equal(4, actionneurs.Count());
        Assert.All(actionneurs, a => Assert.IsType<Actionneur>(a));

        // Tous les devices doivent contenir à la fois Actionneurs et Sondes
        Assert.Equal(5, allDevices.Count); // 4 actionneurs + 1 sonde
        Assert.Contains(allDevices, d => d is Actionneur);
        Assert.Contains(allDevices, d => d is Sonde);

        // Vérifier que les actionneurs et sondes partagent la même table
        var actionneursFromDevices = allDevices.OfType<Actionneur>().ToList();
        var sondesFromDevices = allDevices.OfType<Sonde>().ToList();
        Assert.Equal(4, actionneursFromDevices.Count);
        Assert.Single(sondesFromDevices);
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
