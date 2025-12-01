using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le SeuilAlerteRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// Teste le eager loading, la précision décimale, le tri, et la cascade delete.
/// </summary>
public class SeuilAlerteRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISeuilAlerteRepository _repository;
    private readonly string _databaseName;

    // IDs des entités seedées pour référence dans les tests
    private readonly Guid _localisationId;
    private readonly Guid _uniteCelsiusId;
    private readonly Guid _unitePourcentageId;
    private readonly Guid _sondeTempId;
    private readonly Guid _sondeHumiId;
    private readonly Guid _seuilTempMinAlerteId;
    private readonly Guid _seuilTempMinAvertId;
    private readonly Guid _seuilTempMaxAlerteId;
    private readonly Guid _seuilTempMaxAvertId;
    private readonly Guid _seuilTempInactifId;
    private readonly Guid _seuilHumiMinAlerteId;
    private readonly Guid _seuilHumiMaxAlerteId;

    public SeuilAlerteRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new SeuilAlerteRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie

        // IDs pour les entités
        _localisationId = Guid.NewGuid();
        _uniteCelsiusId = Guid.NewGuid();
        _unitePourcentageId = Guid.NewGuid();
        _sondeTempId = Guid.NewGuid();
        _sondeHumiId = Guid.NewGuid();
        _seuilTempMinAlerteId = Guid.NewGuid();
        _seuilTempMinAvertId = Guid.NewGuid();
        _seuilTempMaxAlerteId = Guid.NewGuid();
        _seuilTempMaxAvertId = Guid.NewGuid();
        _seuilTempInactifId = Guid.NewGuid();
        _seuilHumiMinAlerteId = Guid.NewGuid();
        _seuilHumiMaxAlerteId = Guid.NewGuid();

        // Seed Localisation
        var localisation = new Localisation
        {
            Id = _localisationId,
            Nom = "Bureau",
            Description = "Bureau principal",
            DateCreation = DateTime.UtcNow.AddDays(-30)
        };

        // Seed Unités de Mesure
        var celsius = new UniteMesure
        {
            Id = _uniteCelsiusId,
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow.AddDays(-25)
        };

        var pourcentage = new UniteMesure
        {
            Id = _unitePourcentageId,
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow.AddDays(-25)
        };

        // Seed Sondes
        var sondeTemp = new Sonde
        {
            Id = _sondeTempId,
            Nom = "Capteur Température Bureau",
            LocalisationId = _localisationId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow.AddDays(-20),
            DateInstallation = DateTime.UtcNow.AddDays(-19)
        };

        var sondeHumi = new Sonde
        {
            Id = _sondeHumiId,
            Nom = "Capteur Humidité Bureau",
            LocalisationId = _localisationId,
            UniteMesureId = _unitePourcentageId,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow.AddDays(-20),
            DateInstallation = DateTime.UtcNow.AddDays(-19)
        };

        // Seed Seuils d'Alerte pour la sonde température (6 seuils)
        var seuilTempMinAlerte = new SeuilAlerte
        {
            Id = _seuilTempMinAlerteId,
            SondeId = _sondeTempId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var seuilTempMinAvert = new SeuilAlerte
        {
            Id = _seuilTempMinAvertId,
            SondeId = _sondeTempId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 16.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var seuilTempMaxAvert = new SeuilAlerte
        {
            Id = _seuilTempMaxAvertId,
            SondeId = _sondeTempId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 28.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var seuilTempMaxAlerte = new SeuilAlerte
        {
            Id = _seuilTempMaxAlerteId,
            SondeId = _sondeTempId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 30.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var seuilTempInactif = new SeuilAlerte
        {
            Id = _seuilTempInactifId,
            SondeId = _sondeTempId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 32.0m,
            EstActif = false, // INACTIF
            DateCreation = DateTime.UtcNow.AddDays(-5)
        };

        // Seed Seuils d'Alerte pour la sonde humidité (2 seuils)
        var seuilHumiMinAlerte = new SeuilAlerte
        {
            Id = _seuilHumiMinAlerteId,
            SondeId = _sondeHumiId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 30.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-8)
        };

        var seuilHumiMaxAlerte = new SeuilAlerte
        {
            Id = _seuilHumiMaxAlerteId,
            SondeId = _sondeHumiId,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 80.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow.AddDays(-8)
        };

        _context.Localisations.Add(localisation);
        _context.UnitesMesures.AddRange(celsius, pourcentage);
        _context.Sondes.AddRange(sondeTemp, sondeHumi);
        _context.SeuilsAlerte.AddRange(seuilTempMinAlerte, seuilTempMinAvert, seuilTempMaxAvert, seuilTempMaxAlerte, seuilTempInactif, seuilHumiMinAlerte, seuilHumiMaxAlerte);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer un nouveau seuil
        var nouveauSeuil = new SeuilAlerte
        {
            SondeId = _sondeTempId,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 17.5m,
            EstActif = true
        };

        // Act & Assert - CREATE
        var createdSeuil = await _repository.AddAsync(nouveauSeuil);
        Assert.NotEqual(Guid.Empty, createdSeuil.Id);
        Assert.Equal(17.5m, createdSeuil.Valeur);
        Assert.NotEqual(default, createdSeuil.DateCreation);

        // Act & Assert - READ with eager loading
        var readSeuil = await _repository.GetByIdAsync(createdSeuil.Id);
        Assert.NotNull(readSeuil);
        Assert.Equal(17.5m, readSeuil.Valeur);
        Assert.NotNull(readSeuil.Sonde);
        Assert.Equal("Capteur Température Bureau", readSeuil.Sonde.Nom);

        // Act & Assert - UPDATE
        readSeuil.Valeur = 18.0m;
        readSeuil.EstActif = false;
        await _repository.UpdateAsync(readSeuil);
        var updatedSeuil = await _repository.GetByIdAsync(createdSeuil.Id);
        Assert.NotNull(updatedSeuil);
        Assert.Equal(18.0m, updatedSeuil.Valeur);
        Assert.False(updatedSeuil.EstActif);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdSeuil.Id);
        var deletedSeuil = await _repository.GetByIdAsync(createdSeuil.Id);
        Assert.Null(deletedSeuil);

        // Vérifier que les seuils seedés n'ont pas été affectés
        var allSeuils = await _repository.GetAllAsync();
        Assert.Equal(7, allSeuils.Count()); // 7 seuils seedés toujours présents
    }

    [Fact]
    public async Task GetBySondeAsync_SondeTemperature_DevraitRetourner5Seuils()
    {
        // Act - Récupérer tous les seuils de la sonde température (incluant inactif)
        var seuils = await _repository.GetBySondeAsync(_sondeTempId);
        var seuilsList = seuils.ToList();

        // Assert - Vérifier que 5 seuils de la sonde sont retournés (4 actifs + 1 inactif)
        Assert.Equal(5, seuilsList.Count);
        Assert.All(seuilsList, s => Assert.Equal(_sondeTempId, s.SondeId));

        // Vérifier le eager loading
        Assert.All(seuilsList, s =>
        {
            Assert.NotNull(s.Sonde);
            Assert.Equal("Capteur Température Bureau", s.Sonde.Nom);
        });
    }

    [Fact]
    public async Task GetBySondeActifsAsync_SondeTemperature_DevraitRetourner4Seuils()
    {
        // Act - Récupérer uniquement les seuils actifs de la sonde température
        var seuils = await _repository.GetBySondeActifsAsync(_sondeTempId);
        var seuilsList = seuils.ToList();

        // Assert - Vérifier que 4 seuils actifs sont retournés (exclure l'inactif)
        Assert.Equal(4, seuilsList.Count);
        Assert.All(seuilsList, s =>
        {
            Assert.Equal(_sondeTempId, s.SondeId);
            Assert.True(s.EstActif);
        });

        // Vérifier que le seuil inactif n'est pas retourné
        Assert.DoesNotContain(seuilsList, s => s.Id == _seuilTempInactifId);
    }

    [Fact]
    public async Task GetActifsAsync_DevraitRetourner6SeuilsActifs()
    {
        // Act - Récupérer tous les seuils actifs toutes sondes confondues
        var seuils = await _repository.GetActifsAsync();
        var seuilsList = seuils.ToList();

        // Assert - Vérifier que 6 seuils actifs sont retournés
        // 4 de la sonde température + 2 de la sonde humidité
        Assert.Equal(6, seuilsList.Count);
        Assert.All(seuilsList, s => Assert.True(s.EstActif));

        // Vérifier que le seuil inactif n'est pas retourné
        Assert.DoesNotContain(seuilsList, s => s.Id == _seuilTempInactifId);
    }

    [Fact]
    public async Task Tri_DevraitOrganiserParTypeSeuilPuisTypeAlerte()
    {
        // Act - Récupérer les seuils de la sonde température
        var seuils = await _repository.GetBySondeAsync(_sondeTempId);
        var seuilsList = seuils.ToList();

        // Assert - Vérifier l'ordre : Min Alerte, Min Avert, Max Alerte, Max Avert, Max Avert (inactif)
        Assert.Equal(5, seuilsList.Count);

        // Ordre attendu : Minimum Alerte, Minimum Avertissement, Maximum Alerte, Maximum Avertissement (actif), Maximum Avertissement (inactif)
        Assert.Equal(_seuilTempMinAlerteId, seuilsList[0].Id); // Minimum Alerte
        Assert.Equal(TypeSeuil.Minimum, seuilsList[0].TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, seuilsList[0].TypeAlerte);

        Assert.Equal(_seuilTempMinAvertId, seuilsList[1].Id); // Minimum Avertissement
        Assert.Equal(TypeSeuil.Minimum, seuilsList[1].TypeSeuil);
        Assert.Equal(TypeAlerte.Avertissement, seuilsList[1].TypeAlerte);

        Assert.Equal(_seuilTempMaxAlerteId, seuilsList[2].Id); // Maximum Alerte
        Assert.Equal(TypeSeuil.Maximum, seuilsList[2].TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, seuilsList[2].TypeAlerte);

        Assert.Equal(_seuilTempMaxAvertId, seuilsList[3].Id); // Maximum Avertissement (actif)
        Assert.Equal(TypeSeuil.Maximum, seuilsList[3].TypeSeuil);
        Assert.Equal(TypeAlerte.Avertissement, seuilsList[3].TypeAlerte);

        Assert.Equal(_seuilTempInactifId, seuilsList[4].Id); // Maximum Avertissement (inactif)
        Assert.Equal(TypeSeuil.Maximum, seuilsList[4].TypeSeuil);
        Assert.Equal(TypeAlerte.Avertissement, seuilsList[4].TypeAlerte);
    }

    [Fact]
    public async Task EagerLoading_DevraitChargerSonde()
    {
        // Arrange - Récupérer un seuil
        // Act
        var seuil = await _repository.GetByIdAsync(_seuilTempMinAlerteId);

        // Assert - Vérifier que Sonde est chargée
        Assert.NotNull(seuil);

        // Vérifier eager loading de Sonde
        Assert.NotNull(seuil.Sonde);
        Assert.Equal("Capteur Température Bureau", seuil.Sonde.Nom);
        Assert.Equal(_sondeTempId, seuil.Sonde.Id);

        // Vérifier les propriétés du seuil
        Assert.Equal(15.0m, seuil.Valeur);
        Assert.Equal(TypeSeuil.Minimum, seuil.TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, seuil.TypeAlerte);
        Assert.True(seuil.EstActif);
    }

    [Fact]
    public async Task CascadeDelete_SupprimerSonde_DevraitSupprimerSeuils()
    {
        // Arrange - Créer une nouvelle sonde avec des seuils
        var nouvelleLocalisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Garage Test",
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
        _context.Localisations.Add(nouvelleLocalisation);
        _context.Sondes.Add(nouvelleSonde);
        await _context.SaveChangesAsync();

        var seuilTest1 = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = nouvelleSonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 10.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        var seuilTest2 = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = nouvelleSonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 35.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.SeuilsAlerte.AddRange(seuilTest1, seuilTest2);
        await _context.SaveChangesAsync();

        // Vérifier que les seuils existent
        var seuilsAvant = await _repository.GetBySondeAsync(nouvelleSonde.Id);
        Assert.Equal(2, seuilsAvant.Count());

        // Act - Supprimer la sonde (cascade delete doit supprimer les seuils)
        _context.Sondes.Remove(nouvelleSonde);
        await _context.SaveChangesAsync();

        // Assert - Vérifier que les seuils ont été supprimés
        var seuilsApres = await _repository.GetBySondeAsync(nouvelleSonde.Id);
        Assert.Empty(seuilsApres);

        var seuil1Apres = await _repository.GetByIdAsync(seuilTest1.Id);
        var seuil2Apres = await _repository.GetByIdAsync(seuilTest2.Id);
        Assert.Null(seuil1Apres);
        Assert.Null(seuil2Apres);

        // Vérifier que les autres seuils n'ont pas été affectés
        var allSeuils = await _repository.GetAllAsync();
        Assert.Equal(7, allSeuils.Count()); // Les 7 seuils seedés sont toujours là
    }

    [Fact]
    public async Task Configuration_DevraitAccepterPlusieursSeuilsMemeType()
    {
        // Act - Récupérer les seuils Minimum de la sonde température
        var seuils = await _repository.GetBySondeAsync(_sondeTempId);
        var seuilsMinimum = seuils.Where(s => s.TypeSeuil == TypeSeuil.Minimum).ToList();

        // Assert - Vérifier qu'on peut avoir plusieurs seuils du même TypeSeuil avec différents TypeAlerte
        Assert.Equal(2, seuilsMinimum.Count);

        // Vérifier qu'on a bien Min Alerte ET Min Avertissement
        Assert.Contains(seuilsMinimum, s => s.TypeAlerte == TypeAlerte.Alerte && s.Valeur == 15.0m);
        Assert.Contains(seuilsMinimum, s => s.TypeAlerte == TypeAlerte.Avertissement && s.Valeur == 16.0m);

        // Vérifier de même pour Maximum
        var seuilsMaximum = seuils.Where(s => s.TypeSeuil == TypeSeuil.Maximum).ToList();
        Assert.Equal(3, seuilsMaximum.Count); // 2 actifs + 1 inactif

        // Vérifier qu'on a bien Max Alerte ET Max Avertissement (actif et inactif)
        Assert.Contains(seuilsMaximum, s => s.TypeAlerte == TypeAlerte.Alerte && s.Valeur == 30.0m);
        Assert.Contains(seuilsMaximum, s => s.TypeAlerte == TypeAlerte.Avertissement && s.Valeur == 28.0m && s.EstActif);
        Assert.Contains(seuilsMaximum, s => s.TypeAlerte == TypeAlerte.Avertissement && s.Valeur == 32.0m && !s.EstActif);
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
