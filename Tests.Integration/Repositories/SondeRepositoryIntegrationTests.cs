using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le SondeRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// Teste le Table Per Hierarchy et le eager loading.
/// </summary>
public class SondeRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISondeRepository _repository;
    private readonly string _databaseName;

    // IDs des entités seedées pour référence dans les tests
    private readonly Guid _salonId;
    private readonly Guid _cuisineId;
    private readonly Guid _uniteCelsiusId;
    private readonly Guid _unitePourcentageId;
    private readonly Guid _unitePpmId;
    private readonly Guid _sondeTempSalonId;
    private readonly Guid _sondeHumidSalonId;
    private readonly Guid _sondeAirCuisineId;
    private readonly Guid _sondeTempCuisineId;

    public SondeRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new SondeRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie

        // IDs pour les localisations
        _salonId = Guid.NewGuid();
        _cuisineId = Guid.NewGuid();

        // IDs pour les unités de mesure
        _uniteCelsiusId = Guid.NewGuid();
        _unitePourcentageId = Guid.NewGuid();
        _unitePpmId = Guid.NewGuid();

        // IDs pour les sondes
        _sondeTempSalonId = Guid.NewGuid();
        _sondeHumidSalonId = Guid.NewGuid();
        _sondeAirCuisineId = Guid.NewGuid();
        _sondeTempCuisineId = Guid.NewGuid();

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

        // Seed Unités de Mesure
        var celsius = new UniteMesure
        {
            Id = _uniteCelsiusId,
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow.AddDays(-20)
        };

        var pourcentage = new UniteMesure
        {
            Id = _unitePourcentageId,
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow.AddDays(-20)
        };

        var ppm = new UniteMesure
        {
            Id = _unitePpmId,
            Nom = "Parties par million",
            Symbole = "ppm",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow.AddDays(-20)
        };

        // Seed Sondes
        var sondeTempSalon = new Sonde
        {
            Id = _sondeTempSalonId,
            Nom = "Capteur Température Salon",
            LocalisationId = _salonId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            UrlDevice = "http://192.168.1.10/sensor",
            CredentialsDevice = "encrypted_credentials_1",
            DateCreation = DateTime.UtcNow.AddDays(-15),
            DateInstallation = DateTime.UtcNow.AddDays(-14)
        };

        var sondeHumidSalon = new Sonde
        {
            Id = _sondeHumidSalonId,
            Nom = "Capteur Humidité Salon",
            LocalisationId = _salonId,
            UniteMesureId = _unitePourcentageId,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            UrlDevice = null,
            CredentialsDevice = null,
            DateCreation = DateTime.UtcNow.AddDays(-10),
            DateInstallation = DateTime.UtcNow.AddDays(-9)
        };

        var sondeAirCuisine = new Sonde
        {
            Id = _sondeAirCuisineId,
            Nom = "Capteur Qualité Air Cuisine",
            LocalisationId = _cuisineId,
            UniteMesureId = _unitePpmId,
            TypeSonde = TypeSonde.QualiteAir,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            UrlDevice = null,
            CredentialsDevice = null,
            DateCreation = DateTime.UtcNow.AddDays(-5),
            DateInstallation = DateTime.UtcNow.AddDays(-4)
        };

        var sondeTempCuisine = new Sonde
        {
            Id = _sondeTempCuisineId,
            Nom = "Capteur Température Cuisine",
            LocalisationId = _cuisineId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = false, // INACTIVE
            CanalCommunication = CanalCommunication.HttpPush,
            UrlDevice = "http://192.168.1.20/sensor",
            CredentialsDevice = null,
            DateCreation = DateTime.UtcNow.AddDays(-2),
            DateInstallation = DateTime.UtcNow.AddDays(-1)
        };

        _context.Localisations.AddRange(salon, cuisine);
        _context.UnitesMesures.AddRange(celsius, pourcentage, ppm);
        _context.Sondes.AddRange(sondeTempSalon, sondeHumidSalon, sondeAirCuisine, sondeTempCuisine);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer une nouvelle sonde
        var nouvelleSonde = new Sonde
        {
            Nom = "Nouvelle Sonde Test",
            LocalisationId = _salonId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };

        // Act & Assert - CREATE
        var createdSonde = await _repository.AddAsync(nouvelleSonde);
        Assert.NotEqual(Guid.Empty, createdSonde.Id);
        Assert.Equal("Nouvelle Sonde Test", createdSonde.Nom);

        // Act & Assert - READ with eager loading
        var readSonde = await _repository.GetByIdAsync(createdSonde.Id);
        Assert.NotNull(readSonde);
        Assert.Equal("Nouvelle Sonde Test", readSonde.Nom);
        Assert.NotNull(readSonde.Localisation);
        Assert.Equal("Salon", readSonde.Localisation.Nom);
        Assert.NotNull(readSonde.UniteMesure);
        Assert.Equal("°C", readSonde.UniteMesure.Symbole);

        // Act & Assert - UPDATE
        readSonde.Nom = "Sonde Modifiée";
        readSonde.EstActif = false;
        await _repository.UpdateAsync(readSonde);
        var updatedSonde = await _repository.GetByIdAsync(createdSonde.Id);
        Assert.NotNull(updatedSonde);
        Assert.Equal("Sonde Modifiée", updatedSonde.Nom);
        Assert.False(updatedSonde.EstActif);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdSonde.Id);
        var deletedSonde = await _repository.GetByIdAsync(createdSonde.Id);
        Assert.Null(deletedSonde);

        // Vérifier que les sondes seedées n'ont pas été affectées
        var allSondes = await _repository.GetAllAsync();
        Assert.Equal(4, allSondes.Count()); // 4 sondes seedées toujours présentes
    }

    [Fact]
    public async Task GetByLocalisationAsync_Salon_DevraitRetourner2Sondes()
    {
        // Arrange - Les sondes sont déjà seedées (2 dans le salon)

        // Act - Récupérer les sondes du salon
        var sondes = await _repository.GetByLocalisationAsync(_salonId);
        var sondesList = sondes.ToList();

        // Assert - Vérifier que 2 sondes du salon sont retournées
        Assert.Equal(2, sondesList.Count);
        Assert.All(sondesList, s => Assert.Equal(_salonId, s.LocalisationId));

        // Vérifier le tri par nom
        Assert.Equal("Capteur Humidité Salon", sondesList[0].Nom);
        Assert.Equal("Capteur Température Salon", sondesList[1].Nom);

        // Vérifier le eager loading
        Assert.All(sondesList, s =>
        {
            Assert.NotNull(s.Localisation);
            Assert.Equal("Salon", s.Localisation.Nom);
            Assert.NotNull(s.UniteMesure);
        });
    }

    [Fact]
    public async Task GetByTypeAsync_Temperature_DevraitRetourner2Sondes()
    {
        // Arrange - Les sondes sont déjà seedées (2 sondes de température)

        // Act - Récupérer les sondes de température
        var sondes = await _repository.GetByTypeAsync(TypeSonde.Temperature);
        var sondesList = sondes.ToList();

        // Assert - Vérifier que 2 sondes de température sont retournées
        Assert.Equal(2, sondesList.Count);
        Assert.All(sondesList, s => Assert.Equal(TypeSonde.Temperature, s.TypeSonde));

        // Vérifier le tri par nom
        Assert.Equal("Capteur Température Cuisine", sondesList[0].Nom);
        Assert.Equal("Capteur Température Salon", sondesList[1].Nom);

        // Vérifier que les unités de mesure sont bien chargées
        Assert.All(sondesList, s =>
        {
            Assert.NotNull(s.UniteMesure);
            Assert.Equal("°C", s.UniteMesure.Symbole);
        });
    }

    [Fact]
    public async Task GetActivesAsync_DevraitExclureInactives()
    {
        // Arrange - Les sondes sont déjà seedées (3 actives, 1 inactive)

        // Act - Récupérer uniquement les sondes actives
        var sondes = await _repository.GetActivesAsync();
        var sondesList = sondes.ToList();

        // Assert - Vérifier que seules les 3 sondes actives sont retournées
        Assert.Equal(3, sondesList.Count);
        Assert.All(sondesList, s => Assert.True(s.EstActif));

        // Vérifier que la sonde inactive n'est pas retournée
        Assert.DoesNotContain(sondesList, s => s.Id == _sondeTempCuisineId);

        // Vérifier le tri par nom
        Assert.Equal("Capteur Humidité Salon", sondesList[0].Nom);
        Assert.Equal("Capteur Qualité Air Cuisine", sondesList[1].Nom);
        Assert.Equal("Capteur Température Salon", sondesList[2].Nom);
    }

    [Fact]
    public async Task EagerLoading_DevraitChargerLocalisationEtUniteMesure()
    {
        // Arrange - Récupérer une sonde

        // Act - Récupérer la sonde avec eager loading
        var sonde = await _repository.GetByIdAsync(_sondeTempSalonId);

        // Assert - Vérifier que Localisation et UniteMesure sont chargés
        Assert.NotNull(sonde);

        // Vérifier eager loading de Localisation
        Assert.NotNull(sonde.Localisation);
        Assert.Equal("Salon", sonde.Localisation.Nom);
        Assert.Equal("Pièce principale", sonde.Localisation.Description);

        // Vérifier eager loading de UniteMesure
        Assert.NotNull(sonde.UniteMesure);
        Assert.Equal("Celsius", sonde.UniteMesure.Nom);
        Assert.Equal("°C", sonde.UniteMesure.Symbole);
        Assert.Equal(TypeSonde.Temperature, sonde.UniteMesure.TypeSonde);
    }

    [Fact]
    public async Task TablePerHierarchy_DevraitStockerSondesEtActionneursMemeSTable()
    {
        // Arrange - Ajouter un actionneur dans la même base
        var actionneur = new Actionneur
        {
            Nom = "Ampoule Test",
            LocalisationId = _salonId,
            TypeActionneur = TypeActionneur.AmpouleSimple,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };
        _context.Actionneurs.Add(actionneur);
        await _context.SaveChangesAsync();

        // Act - Récupérer toutes les sondes et tous les devices
        var sondes = await _repository.GetAllAsync();
        var allDevices = await _context.Devices.ToListAsync();

        // Assert - Vérifier le Table Per Hierarchy
        // Les sondes ne doivent contenir que des Sonde (pas d'Actionneur)
        Assert.Equal(4, sondes.Count());
        Assert.All(sondes, s => Assert.IsType<Sonde>(s));

        // Tous les devices doivent contenir à la fois Sondes et Actionneurs
        Assert.Equal(5, allDevices.Count); // 4 sondes + 1 actionneur
        Assert.Contains(allDevices, d => d is Sonde);
        Assert.Contains(allDevices, d => d is Actionneur);

        // Vérifier que les sondes et actionneurs partagent la même table
        var sondesFromDevices = allDevices.OfType<Sonde>().ToList();
        var actionneursFromDevices = allDevices.OfType<Actionneur>().ToList();
        Assert.Equal(4, sondesFromDevices.Count);
        Assert.Single(actionneursFromDevices);
    }

    [Fact]
    public async Task AddAsync_AvecDifferentsCanaux_DevraitReussir()
    {
        // Arrange - Créer des sondes avec différents canaux de communication

        var sondeHttpPush = new Sonde
        {
            Nom = "Sonde HttpPush",
            LocalisationId = _salonId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush,
            UrlDevice = "http://device1.local"
        };

        var sondeHttpPull = new Sonde
        {
            Nom = "Sonde HttpPull",
            LocalisationId = _salonId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            UrlDevice = "http://device2.local",
            CredentialsDevice = "encrypted_token"
        };

        var sondeMqtt = new Sonde
        {
            Nom = "Sonde MQTT",
            LocalisationId = _salonId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };

        var sondeSignalR = new Sonde
        {
            Nom = "Sonde SignalR",
            LocalisationId = _salonId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR
        };

        // Act - Ajouter toutes les sondes
        await _repository.AddAsync(sondeHttpPush);
        await _repository.AddAsync(sondeHttpPull);
        await _repository.AddAsync(sondeMqtt);
        await _repository.AddAsync(sondeSignalR);

        // Assert - Vérifier que toutes les sondes ont été ajoutées avec leur canal
        var toutes = await _repository.GetAllAsync();
        var nouvellesSondes = toutes.Where(s => s.Nom.StartsWith("Sonde ")).ToList();

        Assert.Equal(4, nouvellesSondes.Count);
        Assert.Contains(nouvellesSondes, s => s.CanalCommunication == CanalCommunication.HttpPush);
        Assert.Contains(nouvellesSondes, s => s.CanalCommunication == CanalCommunication.HttpPull);
        Assert.Contains(nouvellesSondes, s => s.CanalCommunication == CanalCommunication.MQTT);
        Assert.Contains(nouvellesSondes, s => s.CanalCommunication == CanalCommunication.SignalR);
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
