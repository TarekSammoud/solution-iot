using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le AlerteRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// Teste le eager loading, les transitions de statut, la cascade delete, et la coexistence Avertissement/Alerte.
/// </summary>
public class AlerteRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAlerteRepository _repository;
    private readonly string _databaseName;

    // IDs des entités seedées pour référence dans les tests
    private readonly Guid _localisationId;
    private readonly Guid _uniteCelsiusId;
    private readonly Guid _sondeSalonId;
    private readonly Guid _sondeCuisineId;
    private readonly Guid _seuilSalonMinAvertId;
    private readonly Guid _seuilSalonMinAlerteId;
    private readonly Guid _seuilSalonMaxAvertId;
    private readonly Guid _seuilSalonMaxAlerteId;
    private readonly Guid _seuilCuisineMinAlerteId;
    private readonly Guid _alerte1Id; // Salon Min Alerte Active
    private readonly Guid _alerte2Id; // Salon Min Avert Active
    private readonly Guid _alerte3Id; // Salon Max Alerte Acquittee
    private readonly Guid _alerte4Id; // Salon Max Avert Resolue
    private readonly Guid _alerte5Id; // Cuisine Min Alerte Active
    private readonly Guid _alerte6Id; // Cuisine Min Alerte Resolue

    public AlerteRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new AlerteRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie

        // IDs pour les entités
        _localisationId = Guid.NewGuid();
        _uniteCelsiusId = Guid.NewGuid();
        _sondeSalonId = Guid.NewGuid();
        _sondeCuisineId = Guid.NewGuid();
        _seuilSalonMinAvertId = Guid.NewGuid();
        _seuilSalonMinAlerteId = Guid.NewGuid();
        _seuilSalonMaxAvertId = Guid.NewGuid();
        _seuilSalonMaxAlerteId = Guid.NewGuid();
        _seuilCuisineMinAlerteId = Guid.NewGuid();
        _alerte1Id = Guid.NewGuid();
        _alerte2Id = Guid.NewGuid();
        _alerte3Id = Guid.NewGuid();
        _alerte4Id = Guid.NewGuid();
        _alerte5Id = Guid.NewGuid();
        _alerte6Id = Guid.NewGuid();

        // Seed Localisation
        var localisation = new Localisation
        {
            Id = _localisationId,
            Nom = "Maison",
            Description = "Maison principale",
            DateCreation = DateTime.UtcNow.AddDays(-30)
        };

        // Seed Unité de Mesure
        var celsius = new UniteMesure
        {
            Id = _uniteCelsiusId,
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow.AddDays(-25)
        };

        // Seed Sondes
        var sondeSalon = new Sonde
        {
            Id = _sondeSalonId,
            Nom = "Capteur Température Salon",
            LocalisationId = _localisationId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow.AddDays(-20),
            DateInstallation = DateTime.UtcNow.AddDays(-19)
        };

        var sondeCuisine = new Sonde
        {
            Id = _sondeCuisineId,
            Nom = "Capteur Température Cuisine",
            LocalisationId = _localisationId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow.AddDays(-20),
            DateInstallation = DateTime.UtcNow.AddDays(-19)
        };

        // Seed Seuils d'Alerte pour Salon
        var seuilSalonMinAvert = new SeuilAlerte
        {
            Id = _seuilSalonMinAvertId,
            SondeId = _sondeSalonId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 16.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var seuilSalonMinAlerte = new SeuilAlerte
        {
            Id = _seuilSalonMinAlerteId,
            SondeId = _sondeSalonId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var seuilSalonMaxAvert = new SeuilAlerte
        {
            Id = _seuilSalonMaxAvertId,
            SondeId = _sondeSalonId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 28.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var seuilSalonMaxAlerte = new SeuilAlerte
        {
            Id = _seuilSalonMaxAlerteId,
            SondeId = _sondeSalonId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 30.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        // Seed Seuils d'Alerte pour Cuisine
        var seuilCuisineMinAlerte = new SeuilAlerte
        {
            Id = _seuilCuisineMinAlerteId,
            SondeId = _sondeCuisineId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        // Seed Alertes
        var alerte1 = new Alerte
        {
            Id = _alerte1Id,
            SondeId = _sondeSalonId,
            SeuilAlerteId = _seuilSalonMinAlerteId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow.AddHours(-2),
            Message = "Température salon trop basse (critique)"
        };

        var alerte2 = new Alerte
        {
            Id = _alerte2Id,
            SondeId = _sondeSalonId,
            SeuilAlerteId = _seuilSalonMinAvertId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Avertissement,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow.AddHours(-1),
            Message = "Température salon trop basse (avertissement)"
        };

        var alerte3 = new Alerte
        {
            Id = _alerte3Id,
            SondeId = _sondeSalonId,
            SeuilAlerteId = _seuilSalonMaxAlerteId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Acquittee,
            DateCreation = DateTime.UtcNow.AddDays(-3),
            DateAcquittement = DateTime.UtcNow.AddDays(-2),
            Message = "Température salon trop élevée"
        };

        var alerte4 = new Alerte
        {
            Id = _alerte4Id,
            SondeId = _sondeSalonId,
            SeuilAlerteId = _seuilSalonMaxAvertId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Statut = StatutAlerte.Resolue,
            DateCreation = DateTime.UtcNow.AddDays(-5),
            DateResolution = DateTime.UtcNow.AddDays(-4),
            Message = "Température salon revenue à la normale"
        };

        var alerte5 = new Alerte
        {
            Id = _alerte5Id,
            SondeId = _sondeCuisineId,
            SeuilAlerteId = _seuilCuisineMinAlerteId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow.AddMinutes(-30),
            Message = "Température cuisine trop basse"
        };

        var alerte6 = new Alerte
        {
            Id = _alerte6Id,
            SondeId = _sondeCuisineId,
            SeuilAlerteId = _seuilCuisineMinAlerteId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Resolue,
            DateCreation = DateTime.UtcNow.AddDays(-10),
            DateResolution = DateTime.UtcNow.AddDays(-9),
            Message = "Température cuisine normalisée"
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(celsius);
        _context.Sondes.AddRange(sondeSalon, sondeCuisine);
        _context.SeuilsAlerte.AddRange(seuilSalonMinAvert, seuilSalonMinAlerte, seuilSalonMaxAvert, seuilSalonMaxAlerte, seuilCuisineMinAlerte);
        _context.Alertes.AddRange(alerte1, alerte2, alerte3, alerte4, alerte5, alerte6);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer une nouvelle alerte
        var nouvelleAlerte = new Alerte
        {
            SondeId = _sondeSalonId,
            SeuilAlerteId = _seuilSalonMaxAlerteId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Message = "Test nouvelle alerte"
        };

        // Act & Assert - CREATE
        var createdAlerte = await _repository.AddAsync(nouvelleAlerte);
        Assert.NotEqual(Guid.Empty, createdAlerte.Id);
        Assert.Equal("Test nouvelle alerte", createdAlerte.Message);
        Assert.Equal(StatutAlerte.Active, createdAlerte.Statut);
        Assert.NotEqual(default, createdAlerte.DateCreation);

        // Act & Assert - READ with eager loading
        var readAlerte = await _repository.GetByIdAsync(createdAlerte.Id);
        Assert.NotNull(readAlerte);
        Assert.Equal("Test nouvelle alerte", readAlerte.Message);
        Assert.NotNull(readAlerte.Sonde);
        Assert.Equal("Capteur Température Salon", readAlerte.Sonde.Nom);
        Assert.NotNull(readAlerte.SeuilAlerte);
        Assert.Equal(30.0m, readAlerte.SeuilAlerte.Valeur);

        // Act & Assert - UPDATE
        readAlerte.Statut = StatutAlerte.Acquittee;
        readAlerte.DateAcquittement = DateTime.UtcNow;
        await _repository.UpdateAsync(readAlerte);
        var updatedAlerte = await _repository.GetByIdAsync(createdAlerte.Id);
        Assert.NotNull(updatedAlerte);
        Assert.Equal(StatutAlerte.Acquittee, updatedAlerte.Statut);
        Assert.NotNull(updatedAlerte.DateAcquittement);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdAlerte.Id);
        var deletedAlerte = await _repository.GetByIdAsync(createdAlerte.Id);
        Assert.Null(deletedAlerte);

        // Vérifier que les alertes seedées n'ont pas été affectées
        var allAlertes = await _repository.GetAllAsync();
        Assert.Equal(6, allAlertes.Count()); // 6 alertes seedées toujours présentes
    }

    [Fact]
    public async Task Scenario_CycleVieAlerte()
    {
        // Arrange - Créer une nouvelle alerte
        var alerte = new Alerte
        {
            SondeId = _sondeSalonId,
            SeuilAlerteId = _seuilSalonMinAlerteId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Message = "Test cycle de vie"
        };

        // Act & Assert - État initial Active
        var created = await _repository.AddAsync(alerte);
        Assert.Equal(StatutAlerte.Active, created.Statut);
        Assert.Null(created.DateAcquittement);
        Assert.Null(created.DateResolution);

        // Act & Assert - Transition Active → Acquittee
        created.Statut = StatutAlerte.Acquittee;
        created.DateAcquittement = DateTime.UtcNow;
        await _repository.UpdateAsync(created);
        var acquittee = await _repository.GetByIdAsync(created.Id);
        Assert.NotNull(acquittee);
        Assert.Equal(StatutAlerte.Acquittee, acquittee.Statut);
        Assert.NotNull(acquittee.DateAcquittement);
        Assert.Null(acquittee.DateResolution);

        // Act & Assert - Transition Acquittee → Resolue
        acquittee.Statut = StatutAlerte.Resolue;
        acquittee.DateResolution = DateTime.UtcNow;
        await _repository.UpdateAsync(acquittee);
        var resolue = await _repository.GetByIdAsync(created.Id);
        Assert.NotNull(resolue);
        Assert.Equal(StatutAlerte.Resolue, resolue.Statut);
        Assert.NotNull(resolue.DateAcquittement);
        Assert.NotNull(resolue.DateResolution);
    }

    [Fact]
    public async Task GetBySondeAsync_SondeSalon_DevraitRetourner4Alertes()
    {
        // Act - Récupérer toutes les alertes de la sonde salon
        var alertes = await _repository.GetBySondeAsync(_sondeSalonId);
        var alertesList = alertes.ToList();

        // Assert - Vérifier que 4 alertes de la sonde salon sont retournées
        Assert.Equal(4, alertesList.Count);
        Assert.All(alertesList, a => Assert.Equal(_sondeSalonId, a.SondeId));

        // Vérifier le tri par DateCreation descendant (plus récentes en premier)
        // alerte2 (-1h) > alerte1 (-2h) > alerte3 (-3j) > alerte4 (-5j)
        Assert.Equal(_alerte2Id, alertesList[0].Id);
        Assert.Equal(_alerte1Id, alertesList[1].Id);
        Assert.Equal(_alerte3Id, alertesList[2].Id);
        Assert.Equal(_alerte4Id, alertesList[3].Id);

        // Vérifier le eager loading
        Assert.All(alertesList, a =>
        {
            Assert.NotNull(a.Sonde);
            Assert.Equal("Capteur Température Salon", a.Sonde.Nom);
            Assert.NotNull(a.SeuilAlerte);
        });
    }

    [Fact]
    public async Task GetActivesAsync_DevraitRetourner3AlertesActives()
    {
        // Act - Récupérer uniquement les alertes actives
        var alertes = await _repository.GetActivesAsync();
        var alertesList = alertes.ToList();

        // Assert - Vérifier que 3 alertes actives sont retournées (2 salon + 1 cuisine)
        Assert.Equal(3, alertesList.Count);
        Assert.All(alertesList, a => Assert.Equal(StatutAlerte.Active, a.Statut));

        // Vérifier les IDs
        Assert.Contains(alertesList, a => a.Id == _alerte1Id); // Salon Min Alerte
        Assert.Contains(alertesList, a => a.Id == _alerte2Id); // Salon Min Avert
        Assert.Contains(alertesList, a => a.Id == _alerte5Id); // Cuisine Min Alerte

        // Vérifier le tri par DateCreation descendant
        // alerte5 (-30min) > alerte2 (-1h) > alerte1 (-2h)
        Assert.Equal(_alerte5Id, alertesList[0].Id);
        Assert.Equal(_alerte2Id, alertesList[1].Id);
        Assert.Equal(_alerte1Id, alertesList[2].Id);
    }

    [Fact]
    public async Task GetByStatutAsync_Acquittee_DevraitRetourner1Alerte()
    {
        // Act - Récupérer les alertes acquittées
        var alertes = await _repository.GetByStatutAsync(StatutAlerte.Acquittee);
        var alertesList = alertes.ToList();

        // Assert - Vérifier qu'1 alerte acquittée est retournée
        Assert.Single(alertesList);
        Assert.Equal(_alerte3Id, alertesList[0].Id);
        Assert.Equal(StatutAlerte.Acquittee, alertesList[0].Statut);
        Assert.NotNull(alertesList[0].DateAcquittement);
    }

    [Fact]
    public async Task GetAlerteActiveForSeuilAsync_DevraitTrouverAlerteActive()
    {
        // Act - Chercher l'alerte active Min Alerte pour la sonde salon
        var alerte = await _repository.GetAlerteActiveForSeuilAsync(_sondeSalonId, _seuilSalonMinAlerteId, TypeAlerte.Alerte);

        // Assert - Doit trouver alerte1
        Assert.NotNull(alerte);
        Assert.Equal(_alerte1Id, alerte.Id);
        Assert.Equal(StatutAlerte.Active, alerte.Statut);
    }

    [Fact]
    public async Task GetAlerteActiveForSeuilAsync_TypeAlerteDifferent_DevraitRetournerNull()
    {
        // Act - Chercher une Alerte alors qu'il n'y a qu'un Avertissement actif
        // Il existe alerte2 (Min Avertissement Active) mais on cherche une Alerte
        var alerte = await _repository.GetAlerteActiveForSeuilAsync(_sondeSalonId, _seuilSalonMinAvertId, TypeAlerte.Alerte);

        // Assert - Ne doit PAS trouver l'Avertissement car TypeAlerte différent
        Assert.Null(alerte);
    }

    [Fact]
    public async Task CoexistenceAvertissementEtAlerte_MemeSeuil_DevraitEtrePossible()
    {
        // Act - Récupérer les alertes actives du salon avec TypeSeuil Minimum
        var alertes = await _repository.GetBySondeAsync(_sondeSalonId);
        var alertesMin = alertes.Where(a => a.TypeSeuil == TypeSeuil.Minimum && a.Statut == StatutAlerte.Active).ToList();

        // Assert - Vérifier qu'on a bien 2 alertes actives de type Minimum
        // alerte1 : Min Alerte Active
        // alerte2 : Min Avertissement Active
        Assert.Equal(2, alertesMin.Count);

        // Vérifier qu'on a bien une Alerte ET un Avertissement
        Assert.Contains(alertesMin, a => a.TypeAlerte == TypeAlerte.Alerte && a.Id == _alerte1Id);
        Assert.Contains(alertesMin, a => a.TypeAlerte == TypeAlerte.Avertissement && a.Id == _alerte2Id);

        // Vérifier que les deux alertent sur le même TypeSeuil (Minimum)
        Assert.All(alertesMin, a => Assert.Equal(TypeSeuil.Minimum, a.TypeSeuil));
    }

    [Fact]
    public async Task EagerLoading_DevraitChargerSondeEtSeuilAlerte()
    {
        // Arrange - Récupérer une alerte
        // Act
        var alerte = await _repository.GetByIdAsync(_alerte1Id);

        // Assert - Vérifier que Sonde et SeuilAlerte sont chargés
        Assert.NotNull(alerte);

        // Vérifier eager loading de Sonde
        Assert.NotNull(alerte.Sonde);
        Assert.Equal("Capteur Température Salon", alerte.Sonde.Nom);
        Assert.Equal(_sondeSalonId, alerte.Sonde.Id);

        // Vérifier eager loading de SeuilAlerte
        Assert.NotNull(alerte.SeuilAlerte);
        Assert.Equal(15.0m, alerte.SeuilAlerte.Valeur);
        Assert.Equal(TypeSeuil.Minimum, alerte.SeuilAlerte.TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, alerte.SeuilAlerte.TypeAlerte);

        // Vérifier les propriétés de l'alerte
        Assert.Equal(StatutAlerte.Active, alerte.Statut);
        Assert.Equal(TypeSeuil.Minimum, alerte.TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, alerte.TypeAlerte);
    }

    [Fact]
    public async Task CascadeDelete_SupprimerSonde_DevraitSupprimerAlertes()
    {
        // Arrange - Créer une nouvelle sonde avec des alertes
        var nouvelleLocalisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Test Cascade",
            DateCreation = DateTime.UtcNow
        };
        var nouvelleSonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Sonde Test Cascade",
            LocalisationId = nouvelleLocalisation.Id,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow,
            DateInstallation = DateTime.UtcNow
        };
        var nouveauSeuil = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = nouvelleSonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(nouvelleLocalisation);
        _context.Sondes.Add(nouvelleSonde);
        _context.SeuilsAlerte.Add(nouveauSeuil);
        await _context.SaveChangesAsync();

        var alerteTest1 = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = nouvelleSonde.Id,
            SeuilAlerteId = nouveauSeuil.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow
        };
        var alerteTest2 = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = nouvelleSonde.Id,
            SeuilAlerteId = nouveauSeuil.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Resolue,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.AddRange(alerteTest1, alerteTest2);
        await _context.SaveChangesAsync();

        // Vérifier que les alertes existent
        var alertesAvant = await _repository.GetBySondeAsync(nouvelleSonde.Id);
        Assert.Equal(2, alertesAvant.Count());

        // Act - Supprimer la sonde (cascade delete doit supprimer les alertes)
        _context.Sondes.Remove(nouvelleSonde);
        await _context.SaveChangesAsync();

        // Assert - Vérifier que les alertes ont été supprimées
        var alertesApres = await _repository.GetBySondeAsync(nouvelleSonde.Id);
        Assert.Empty(alertesApres);

        var alerte1Apres = await _repository.GetByIdAsync(alerteTest1.Id);
        var alerte2Apres = await _repository.GetByIdAsync(alerteTest2.Id);
        Assert.Null(alerte1Apres);
        Assert.Null(alerte2Apres);

        // Vérifier que les autres alertes n'ont pas été affectées
        var allAlertes = await _repository.GetAllAsync();
        Assert.Equal(6, allAlertes.Count()); // Les 6 alertes seedées sont toujours là
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
