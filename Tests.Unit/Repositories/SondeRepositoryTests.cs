using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour le SondeRepository.
/// Utilise InMemoryDatabase pour tester les opérations CRUD sans dépendance à une vraie base de données.
/// Teste également le eager loading et le pattern Table Per Hierarchy.
/// </summary>
public class SondeRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISondeRepository _repository;
    private readonly string _databaseName;

    public SondeRepositoryTests()
    {
        // Crée une base de données InMemory avec un nom unique par test (évite les conflits)
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new SondeRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterSonde_AvecGenerationAutomatique()
    {
        // Arrange - Créer une localisation et une unité de mesure
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salon",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        await _context.SaveChangesAsync();

        // Préparer une sonde sans Id, DateCreation ni DateInstallation
        var sonde = new Sonde
        {
            Nom = "Capteur Température Salon",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            UrlDevice = "http://192.168.1.10/sensor"
        };

        // Act - Ajouter la sonde
        var result = await _repository.AddAsync(sonde);

        // Assert - Vérifier que l'Id, DateCreation et DateInstallation ont été générés
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.DateCreation);
        Assert.NotEqual(default(DateTime), result.DateInstallation);
        Assert.Equal("Capteur Température Salon", result.Nom);
        Assert.Equal(TypeSonde.Temperature, result.TypeSonde);
        Assert.Equal(CanalCommunication.HttpPull, result.CanalCommunication);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerSonde_AvecEagerLoading()
    {
        // Arrange - Créer une localisation, une unité et une sonde
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Cuisine",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        // Act - Récupérer la sonde par son Id
        var result = await _repository.GetByIdAsync(sonde.Id);

        // Assert - Vérifier que la sonde est trouvée avec eager loading
        Assert.NotNull(result);
        Assert.Equal(sonde.Id, result.Id);
        Assert.Equal("Capteur Humidité", result.Nom);

        // Vérifier le eager loading de Localisation
        Assert.NotNull(result.Localisation);
        Assert.Equal("Cuisine", result.Localisation.Nom);

        // Vérifier le eager loading de UniteMesure
        Assert.NotNull(result.UniteMesure);
        Assert.Equal("%", result.UniteMesure.Symbole);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Tenter de récupérer une sonde inexistante
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert - Vérifier que le résultat est null
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerToutesLesSondes_TrieesParNom()
    {
        // Arrange - Créer une localisation, une unité et plusieurs sondes
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Bureau",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "PPM",
            Symbole = "ppm",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow
        };

        var sonde1 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Zéro",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.QualiteAir,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sonde2 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Alpha",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.QualiteAir,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.AddRange(sonde1, sonde2);
        await _context.SaveChangesAsync();

        // Act - Récupérer toutes les sondes
        var result = await _repository.GetAllAsync();
        var sondesList = result.ToList();

        // Assert - Vérifier le tri par nom
        Assert.Equal(2, sondesList.Count);
        Assert.Equal("Capteur Alpha", sondesList[0].Nom); // Alphabétiquement premier
        Assert.Equal("Capteur Zéro", sondesList[1].Nom);

        // Vérifier le eager loading
        Assert.NotNull(sondesList[0].Localisation);
        Assert.NotNull(sondesList[0].UniteMesure);
    }

    [Fact]
    public async Task GetByLocalisationAsync_DevraitRetournerSeulementSondesDeLaLocalisation()
    {
        // Arrange - Créer deux localisations et des sondes dans chacune
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
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        var sondeSalon1 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Salon 1",
            LocalisationId = salon.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeSalon2 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Salon 2",
            LocalisationId = salon.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeChambre = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Chambre",
            LocalisationId = chambre.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.AddRange(salon, chambre);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.AddRange(sondeSalon1, sondeSalon2, sondeChambre);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les sondes du salon
        var result = await _repository.GetByLocalisationAsync(salon.Id);
        var sondesList = result.ToList();

        // Assert - Vérifier que seules les sondes du salon sont retournées
        Assert.Equal(2, sondesList.Count);
        Assert.All(sondesList, s => Assert.Equal(salon.Id, s.LocalisationId));
        Assert.Contains(sondesList, s => s.Nom == "Capteur Salon 1");
        Assert.Contains(sondesList, s => s.Nom == "Capteur Salon 2");
    }

    [Fact]
    public async Task GetByTypeAsync_Temperature_DevraitRetournerSeulementSondesTemperature()
    {
        // Arrange - Créer des sondes de différents types
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Labo",
            DateCreation = DateTime.UtcNow
        };
        var uniteTemp = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        var uniteHumid = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };

        var sondeTemp = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Température",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteTemp.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeHumid = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteHumid.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.AddRange(uniteTemp, uniteHumid);
        _context.Sondes.AddRange(sondeTemp, sondeHumid);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les sondes de température
        var result = await _repository.GetByTypeAsync(TypeSonde.Temperature);
        var sondesList = result.ToList();

        // Assert - Vérifier que seules les sondes de température sont retournées
        Assert.Single(sondesList);
        Assert.Equal(TypeSonde.Temperature, sondesList[0].TypeSonde);
        Assert.Equal("Capteur Température", sondesList[0].Nom);
    }

    [Fact]
    public async Task GetByTypeAsync_Hydrometrie_DevraitRetournerSeulementSondesHydrometrie()
    {
        // Arrange - Créer des sondes de différents types
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Serre",
            DateCreation = DateTime.UtcNow
        };
        var uniteTemp = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        var uniteHumid = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };

        var sondeTemp = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Temp",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteTemp.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeHumid1 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité 1",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteHumid.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeHumid2 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité 2",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteHumid.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.AddRange(uniteTemp, uniteHumid);
        _context.Sondes.AddRange(sondeTemp, sondeHumid1, sondeHumid2);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les sondes d'hydrométrie
        var result = await _repository.GetByTypeAsync(TypeSonde.Hydrometrie);
        var sondesList = result.ToList();

        // Assert - Vérifier que seules les sondes d'hydrométrie sont retournées
        Assert.Equal(2, sondesList.Count);
        Assert.All(sondesList, s => Assert.Equal(TypeSonde.Hydrometrie, s.TypeSonde));
    }

    [Fact]
    public async Task GetByTypeAsync_QualiteAir_DevraitRetournerSeulementSondesQualiteAir()
    {
        // Arrange - Créer des sondes de différents types
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Usine",
            DateCreation = DateTime.UtcNow
        };
        var uniteQualiteAir = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "PPM",
            Symbole = "ppm",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow
        };
        var uniteTemp = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        var sondeAir = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur CO2",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteQualiteAir.Id,
            TypeSonde = TypeSonde.QualiteAir,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeTemp = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Temp",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteTemp.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.AddRange(uniteQualiteAir, uniteTemp);
        _context.Sondes.AddRange(sondeAir, sondeTemp);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les sondes de qualité d'air
        var result = await _repository.GetByTypeAsync(TypeSonde.QualiteAir);
        var sondesList = result.ToList();

        // Assert - Vérifier que seules les sondes de qualité d'air sont retournées
        Assert.Single(sondesList);
        Assert.Equal(TypeSonde.QualiteAir, sondesList[0].TypeSonde);
        Assert.Equal("Capteur CO2", sondesList[0].Nom);
    }

    [Fact]
    public async Task GetActivesAsync_DevraitRetournerSeulementSondesActives()
    {
        // Arrange - Créer des sondes actives et inactives
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Entrepôt",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        var sondeActive1 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Sonde Active 1",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeActive2 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Sonde Active 2",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        var sondeInactive = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Sonde Inactive",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = false, // INACTIVE
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.AddRange(sondeActive1, sondeActive2, sondeInactive);
        await _context.SaveChangesAsync();

        // Act - Récupérer uniquement les sondes actives
        var result = await _repository.GetActivesAsync();
        var sondesList = result.ToList();

        // Assert - Vérifier que seules les sondes actives sont retournées
        Assert.Equal(2, sondesList.Count);
        Assert.All(sondesList, s => Assert.True(s.EstActif));
        Assert.DoesNotContain(sondesList, s => s.Nom == "Sonde Inactive");
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourSonde()
    {
        // Arrange - Créer et ajouter une sonde
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Garage",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Ancien Nom",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull,
            UrlDevice = "http://old-url.com",
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        // Modifier le nom et l'URL
        sonde.Nom = "Nouveau Nom";
        sonde.UrlDevice = "http://new-url.com";

        // Act - Mettre à jour la sonde
        await _repository.UpdateAsync(sonde);

        // Assert - Vérifier que la modification est persistée
        var updatedSonde = await _repository.GetByIdAsync(sonde.Id);
        Assert.NotNull(updatedSonde);
        Assert.Equal("Nouveau Nom", updatedSonde.Nom);
        Assert.Equal("http://new-url.com", updatedSonde.UrlDevice);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerSonde()
    {
        // Arrange - Créer et ajouter une sonde
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Cave",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Sonde à supprimer",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        // Act - Supprimer la sonde
        await _repository.DeleteAsync(sonde.Id);

        // Assert - Vérifier que la sonde n'existe plus
        var deletedSonde = await _repository.GetByIdAsync(sonde.Id);
        Assert.Null(deletedSonde);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandSondeExiste()
    {
        // Arrange - Créer et ajouter une sonde
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Grenier",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Sonde Existante",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(sonde.Id);

        // Assert - La sonde doit exister
        Assert.True(exists);
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterTousLesCanaux()
    {
        // Arrange - Créer des sondes avec différents canaux
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Test Canal",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        await _context.SaveChangesAsync();

        var sondeHttpPush = new Sonde
        {
            Nom = "Sonde HttpPush",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush
        };

        var sondeHttpPull = new Sonde
        {
            Nom = "Sonde HttpPull",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull
        };

        var sondeMqtt = new Sonde
        {
            Nom = "Sonde MQTT",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };

        var sondeSignalR = new Sonde
        {
            Nom = "Sonde SignalR",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
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
        Assert.Equal(CanalCommunication.HttpPush, sondeHttpPush.CanalCommunication);
        Assert.Equal(CanalCommunication.HttpPull, sondeHttpPull.CanalCommunication);
        Assert.Equal(CanalCommunication.MQTT, sondeMqtt.CanalCommunication);
        Assert.Equal(CanalCommunication.SignalR, sondeSignalR.CanalCommunication);

        var allSondes = await _repository.GetAllAsync();
        Assert.Equal(4, allSondes.Count());
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterCredentialsNull()
    {
        // Arrange - Créer une sonde avec UrlDevice et CredentialsDevice null
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Test Null",
            DateCreation = DateTime.UtcNow
        };
        var uniteMesure = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        await _context.SaveChangesAsync();

        var sonde = new Sonde
        {
            Nom = "Sonde Sans Credentials",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            UrlDevice = null,
            CredentialsDevice = null
        };

        // Act - Ajouter la sonde
        var result = await _repository.AddAsync(sonde);

        // Assert - Vérifier que l'ajout fonctionne avec credentials null
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Null(result.UrlDevice);
        Assert.Null(result.CredentialsDevice);

        // Vérifier la récupération
        var retrieved = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(retrieved);
        Assert.Null(retrieved.UrlDevice);
        Assert.Null(retrieved.CredentialsDevice);
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
