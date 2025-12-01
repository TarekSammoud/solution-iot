using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le ReleveRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// Teste le eager loading, la précision décimale, et la cascade delete.
/// </summary>
public class ReleveRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IReleveRepository _repository;
    private readonly string _databaseName;

    // IDs des entités seedées pour référence dans les tests
    private readonly Guid _salonId;
    private readonly Guid _cuisineId;
    private readonly Guid _uniteCelsiusId;
    private readonly Guid _unitePourcentageId;
    private readonly Guid _sondeTempSalonId;
    private readonly Guid _sondeHumiCuisineId;
    private readonly Guid _releve1Id;
    private readonly Guid _releve2Id;
    private readonly Guid _releve3Id;
    private readonly Guid _releve4Id;
    private readonly Guid _releve5Id;

    public ReleveRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new ReleveRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie

        // IDs pour les localisations
        _salonId = Guid.NewGuid();
        _cuisineId = Guid.NewGuid();

        // IDs pour les unités de mesure
        _uniteCelsiusId = Guid.NewGuid();
        _unitePourcentageId = Guid.NewGuid();

        // IDs pour les sondes
        _sondeTempSalonId = Guid.NewGuid();
        _sondeHumiCuisineId = Guid.NewGuid();

        // IDs pour les relevés
        _releve1Id = Guid.NewGuid();
        _releve2Id = Guid.NewGuid();
        _releve3Id = Guid.NewGuid();
        _releve4Id = Guid.NewGuid();
        _releve5Id = Guid.NewGuid();

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
            DateCreation = DateTime.UtcNow.AddDays(-30)
        };

        // Seed UnitésMesures
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
        var sondeTempSalon = new Sonde
        {
            Id = _sondeTempSalonId,
            Nom = "Capteur Température Salon",
            LocalisationId = _salonId,
            UniteMesureId = _uniteCelsiusId,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT,
            DateCreation = DateTime.UtcNow.AddDays(-20),
            DateInstallation = DateTime.UtcNow.AddDays(-19)
        };

        var sondeHumiCuisine = new Sonde
        {
            Id = _sondeHumiCuisineId,
            Nom = "Capteur Humidité Cuisine",
            LocalisationId = _cuisineId,
            UniteMesureId = _unitePourcentageId,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR,
            DateCreation = DateTime.UtcNow.AddDays(-15),
            DateInstallation = DateTime.UtcNow.AddDays(-14)
        };

        // Seed Relevés - Mix de relevés manuels et automatiques sur différentes périodes
        var baseDate = DateTime.UtcNow.Date.AddDays(-7);

        var releve1 = new Releve
        {
            Id = _releve1Id,
            SondeId = _sondeTempSalonId,
            Valeur = 22.50m,
            DateHeure = baseDate.AddDays(1).AddHours(10),
            TypeReleve = TypeReleve.Automatique
        };

        var releve2 = new Releve
        {
            Id = _releve2Id,
            SondeId = _sondeTempSalonId,
            Valeur = 23.75m,
            DateHeure = baseDate.AddDays(2).AddHours(14),
            TypeReleve = TypeReleve.Automatique
        };

        var releve3 = new Releve
        {
            Id = _releve3Id,
            SondeId = _sondeTempSalonId,
            Valeur = 21.00m,
            DateHeure = baseDate.AddDays(3).AddHours(8),
            TypeReleve = TypeReleve.Manuel
        };

        var releve4 = new Releve
        {
            Id = _releve4Id,
            SondeId = _sondeHumiCuisineId,
            Valeur = 65.80m,
            DateHeure = baseDate.AddDays(2).AddHours(12),
            TypeReleve = TypeReleve.Automatique
        };

        var releve5 = new Releve
        {
            Id = _releve5Id,
            SondeId = _sondeHumiCuisineId,
            Valeur = 68.25m,
            DateHeure = baseDate.AddDays(4).AddHours(16),
            TypeReleve = TypeReleve.Manuel
        };

        _context.Localisations.AddRange(salon, cuisine);
        _context.UnitesMesures.AddRange(celsius, pourcentage);
        _context.Sondes.AddRange(sondeTempSalon, sondeHumiCuisine);
        _context.Releves.AddRange(releve1, releve2, releve3, releve4, releve5);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer un nouveau relevé
        var nouveauReleve = new Releve
        {
            SondeId = _sondeTempSalonId,
            Valeur = 24.50m,
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Automatique
        };

        // Act & Assert - CREATE
        var createdReleve = await _repository.AddAsync(nouveauReleve);
        Assert.NotEqual(Guid.Empty, createdReleve.Id);
        Assert.Equal(24.50m, createdReleve.Valeur);

        // Act & Assert - READ with eager loading
        var readReleve = await _repository.GetByIdAsync(createdReleve.Id);
        Assert.NotNull(readReleve);
        Assert.Equal(24.50m, readReleve.Valeur);
        Assert.NotNull(readReleve.Sonde);
        Assert.Equal("Capteur Température Salon", readReleve.Sonde.Nom);

        // Act & Assert - UPDATE
        readReleve.Valeur = 25.00m;
        readReleve.TypeReleve = TypeReleve.Manuel;
        await _repository.UpdateAsync(readReleve);
        var updatedReleve = await _repository.GetByIdAsync(createdReleve.Id);
        Assert.NotNull(updatedReleve);
        Assert.Equal(25.00m, updatedReleve.Valeur);
        Assert.Equal(TypeReleve.Manuel, updatedReleve.TypeReleve);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdReleve.Id);
        var deletedReleve = await _repository.GetByIdAsync(createdReleve.Id);
        Assert.Null(deletedReleve);

        // Vérifier que les relevés seedés n'ont pas été affectés
        var allReleves = await _repository.GetAllAsync();
        Assert.Equal(5, allReleves.Count()); // 5 relevés seedés toujours présents
    }

    [Fact]
    public async Task GetBySondeAsync_DevraitRetournerRelevesUniquementPourSondeSpecifiee()
    {
        // Act - Récupérer les relevés de la sonde température salon
        var relevesSalon = await _repository.GetBySondeAsync(_sondeTempSalonId);
        var relevesSalonList = relevesSalon.ToList();

        // Assert - Vérifier que 3 relevés de la sonde salon sont retournés
        Assert.Equal(3, relevesSalonList.Count);
        Assert.All(relevesSalonList, r => Assert.Equal(_sondeTempSalonId, r.SondeId));

        // Vérifier le tri par date décroissante (plus récent en premier)
        Assert.Equal(_releve3Id, relevesSalonList[0].Id); // baseDate + 3 jours
        Assert.Equal(_releve2Id, relevesSalonList[1].Id); // baseDate + 2 jours
        Assert.Equal(_releve1Id, relevesSalonList[2].Id); // baseDate + 1 jour

        // Vérifier le eager loading
        Assert.All(relevesSalonList, r =>
        {
            Assert.NotNull(r.Sonde);
            Assert.Equal("Capteur Température Salon", r.Sonde.Nom);
        });

        // Act - Récupérer les relevés de la sonde humidité cuisine
        var relevesCuisine = await _repository.GetBySondeAsync(_sondeHumiCuisineId);
        var relevesCuisineList = relevesCuisine.ToList();

        // Assert - Vérifier que 2 relevés de la sonde cuisine sont retournés
        Assert.Equal(2, relevesCuisineList.Count);
        Assert.All(relevesCuisineList, r => Assert.Equal(_sondeHumiCuisineId, r.SondeId));
    }

    [Fact]
    public async Task GetBySondeDateRangeAsync_DevraitFiltrerParPlageDeDates()
    {
        // Arrange
        var baseDate = DateTime.UtcNow.Date.AddDays(-7);
        var dateDebut = baseDate.AddDays(2);
        var dateFin = baseDate.AddDays(4);

        // Act - Récupérer les relevés de la sonde salon entre J+2 et J+4
        var releves = await _repository.GetBySondeDateRangeAsync(_sondeTempSalonId, dateDebut, dateFin);
        var relevesList = releves.ToList();

        // Assert - Vérifier que seuls les relevés dans la plage sont retournés
        // releve2 (J+2) et releve3 (J+3) doivent être retournés
        // releve1 (J+1) ne doit PAS être retourné
        Assert.Equal(2, relevesList.Count);
        Assert.Contains(relevesList, r => r.Id == _releve2Id);
        Assert.Contains(relevesList, r => r.Id == _releve3Id);
        Assert.DoesNotContain(relevesList, r => r.Id == _releve1Id);

        // Vérifier le tri par date décroissante
        Assert.Equal(_releve3Id, relevesList[0].Id);
        Assert.Equal(_releve2Id, relevesList[1].Id);
    }

    [Fact]
    public async Task GetLastBySondeAsync_DevraitRetournerRelevePlusRecent()
    {
        // Act - Récupérer le dernier relevé de la sonde température salon
        var dernierReleve = await _repository.GetLastBySondeAsync(_sondeTempSalonId);

        // Assert - Vérifier que le relevé le plus récent est retourné (releve3 à J+3)
        Assert.NotNull(dernierReleve);
        Assert.Equal(_releve3Id, dernierReleve.Id);
        Assert.Equal(21.00m, dernierReleve.Valeur);
        Assert.Equal(TypeReleve.Manuel, dernierReleve.TypeReleve);

        // Vérifier le eager loading
        Assert.NotNull(dernierReleve.Sonde);
        Assert.Equal("Capteur Température Salon", dernierReleve.Sonde.Nom);
    }

    [Fact]
    public async Task GetByTypeAsync_DevraitFiltrerParTypeReleve()
    {
        // Act - Récupérer les relevés automatiques
        var relevesAuto = await _repository.GetByTypeAsync(TypeReleve.Automatique);
        var relevesAutoList = relevesAuto.ToList();

        // Assert - Vérifier que 3 relevés automatiques sont retournés
        Assert.Equal(3, relevesAutoList.Count);
        Assert.All(relevesAutoList, r => Assert.Equal(TypeReleve.Automatique, r.TypeReleve));
        Assert.Contains(relevesAutoList, r => r.Id == _releve1Id);
        Assert.Contains(relevesAutoList, r => r.Id == _releve2Id);
        Assert.Contains(relevesAutoList, r => r.Id == _releve4Id);

        // Act - Récupérer les relevés manuels
        var relevesManuel = await _repository.GetByTypeAsync(TypeReleve.Manuel);
        var relevesManuelList = relevesManuel.ToList();

        // Assert - Vérifier que 2 relevés manuels sont retournés
        Assert.Equal(2, relevesManuelList.Count);
        Assert.All(relevesManuelList, r => Assert.Equal(TypeReleve.Manuel, r.TypeReleve));
        Assert.Contains(relevesManuelList, r => r.Id == _releve3Id);
        Assert.Contains(relevesManuelList, r => r.Id == _releve5Id);

        // Vérifier le tri par date décroissante
        Assert.Equal(_releve5Id, relevesManuelList[0].Id); // J+4
        Assert.Equal(_releve3Id, relevesManuelList[1].Id); // J+3
    }

    [Fact]
    public async Task CountBySondeAsync_DevraitRetournerNombreCorrect()
    {
        // Act - Compter les relevés de la sonde température salon
        var countSalon = await _repository.CountBySondeAsync(_sondeTempSalonId);

        // Assert - Vérifier que 3 relevés sont comptés
        Assert.Equal(3, countSalon);

        // Act - Compter les relevés de la sonde humidité cuisine
        var countCuisine = await _repository.CountBySondeAsync(_sondeHumiCuisineId);

        // Assert - Vérifier que 2 relevés sont comptés
        Assert.Equal(2, countCuisine);
    }

    [Fact]
    public async Task DecimalPrecision_DevraitStockerEtRecupererValeursPrecises()
    {
        // Arrange - Créer un relevé avec une valeur décimale précise
        var releve = new Releve
        {
            SondeId = _sondeTempSalonId,
            Valeur = 12345678.90m, // 10 chiffres dont 2 décimales
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Manuel
        };

        // Act - Ajouter et récupérer le relevé
        var created = await _repository.AddAsync(releve);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        // Assert - Vérifier que la valeur décimale est conservée avec précision
        Assert.NotNull(retrieved);
        Assert.Equal(12345678.90m, retrieved.Valeur);

        // Vérifier que les valeurs seedées conservent aussi leur précision
        var releve2 = await _repository.GetByIdAsync(_releve2Id);
        Assert.NotNull(releve2);
        Assert.Equal(23.75m, releve2.Valeur);
    }

    [Fact]
    public async Task CascadeDelete_DevraitSupprimerRelevesQuandSondeEstSupprimee()
    {
        // Arrange - Créer une nouvelle sonde avec des relevés
        var nouvelleLocalisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Bureau Test",
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

        var releveTest1 = new Releve
        {
            Id = Guid.NewGuid(),
            SondeId = nouvelleSonde.Id,
            Valeur = 20.0m,
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Automatique
        };
        var releveTest2 = new Releve
        {
            Id = Guid.NewGuid(),
            SondeId = nouvelleSonde.Id,
            Valeur = 21.0m,
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Automatique
        };
        _context.Releves.AddRange(releveTest1, releveTest2);
        await _context.SaveChangesAsync();

        // Vérifier que les relevés existent
        var countBefore = await _repository.CountBySondeAsync(nouvelleSonde.Id);
        Assert.Equal(2, countBefore);

        // Act - Supprimer la sonde (cascade delete doit supprimer les relevés)
        _context.Sondes.Remove(nouvelleSonde);
        await _context.SaveChangesAsync();

        // Assert - Vérifier que les relevés ont été supprimés
        var countAfter = await _repository.CountBySondeAsync(nouvelleSonde.Id);
        Assert.Equal(0, countAfter);

        var releve1After = await _repository.GetByIdAsync(releveTest1.Id);
        var releve2After = await _repository.GetByIdAsync(releveTest2.Id);
        Assert.Null(releve1After);
        Assert.Null(releve2After);

        // Vérifier que les autres relevés n'ont pas été affectés
        var allReleves = await _repository.GetAllAsync();
        Assert.Equal(5, allReleves.Count()); // Les 5 relevés seedés sont toujours là
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
