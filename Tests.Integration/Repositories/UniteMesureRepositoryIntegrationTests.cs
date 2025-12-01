using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le UniteMesureRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// </summary>
public class UniteMesureRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUniteMesureRepository _repository;
    private readonly string _databaseName;

    // IDs des unités de mesure seedées pour référence dans les tests
    private readonly Guid _celsiusId;
    private readonly Guid _fahrenheitId;
    private readonly Guid _kelvinId;
    private readonly Guid _pourcentageId;
    private readonly Guid _particulesId;
    private readonly Guid _ppmId;
    private readonly Guid _iqaId;

    public UniteMesureRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new UniteMesureRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie
        _celsiusId = Guid.NewGuid();
        _fahrenheitId = Guid.NewGuid();
        _kelvinId = Guid.NewGuid();
        _pourcentageId = Guid.NewGuid();
        _particulesId = Guid.NewGuid();
        _ppmId = Guid.NewGuid();
        _iqaId = Guid.NewGuid();

        // Unités de mesure pour TEMPÉRATURE
        var celsius = new UniteMesure
        {
            Id = _celsiusId,
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow.AddDays(-30)
        };

        var fahrenheit = new UniteMesure
        {
            Id = _fahrenheitId,
            Nom = "Fahrenheit",
            Symbole = "°F",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow.AddDays(-25)
        };

        var kelvin = new UniteMesure
        {
            Id = _kelvinId,
            Nom = "Kelvin",
            Symbole = "K",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow.AddDays(-20)
        };

        // Unités de mesure pour HYDROMÉTRIE
        var pourcentage = new UniteMesure
        {
            Id = _pourcentageId,
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow.AddDays(-15)
        };

        // Unités de mesure pour QUALITÉ DE L'AIR
        var particules = new UniteMesure
        {
            Id = _particulesId,
            Nom = "Microgrammes par mètre cube",
            Symbole = "µg/m³",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var ppm = new UniteMesure
        {
            Id = _ppmId,
            Nom = "Parties par million",
            Symbole = "ppm",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow.AddDays(-5)
        };

        var iqa = new UniteMesure
        {
            Id = _iqaId,
            Nom = "Indice de qualité de l'air",
            Symbole = "IQA",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow.AddDays(-2)
        };

        _context.UnitesMesures.AddRange(celsius, fahrenheit, kelvin, pourcentage, particules, ppm, iqa);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer une nouvelle unité de mesure
        var nouvelleUnite = new UniteMesure
        {
            Nom = "Millibar",
            Symbole = "mbar",
            TypeSonde = TypeSonde.QualiteAir
        };

        // Act & Assert - CREATE
        var createdUnite = await _repository.AddAsync(nouvelleUnite);
        Assert.NotEqual(Guid.Empty, createdUnite.Id);
        Assert.Equal("Millibar", createdUnite.Nom);

        // Act & Assert - READ
        var readUnite = await _repository.GetByIdAsync(createdUnite.Id);
        Assert.NotNull(readUnite);
        Assert.Equal("Millibar", readUnite.Nom);
        Assert.Equal("mbar", readUnite.Symbole);

        // Act & Assert - UPDATE
        readUnite.Nom = "MilliBar Modifié";
        await _repository.UpdateAsync(readUnite);
        var updatedUnite = await _repository.GetByIdAsync(createdUnite.Id);
        Assert.NotNull(updatedUnite);
        Assert.Equal("MilliBar Modifié", updatedUnite.Nom);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdUnite.Id);
        var deletedUnite = await _repository.GetByIdAsync(createdUnite.Id);
        Assert.Null(deletedUnite);

        // Vérifier que les unités seedées n'ont pas été affectées
        var allUnites = await _repository.GetAllAsync();
        Assert.Equal(7, allUnites.Count()); // 3 temp + 1 hydro + 3 air
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerUnitesTrieesParTypeSondePuisNom()
    {
        // Arrange - Les données sont déjà seedées

        // Act - Récupérer toutes les unités
        var unites = await _repository.GetAllAsync();
        var unitesList = unites.ToList();

        // Assert - Vérifier le tri par TypeSonde puis Nom
        Assert.Equal(7, unitesList.Count);

        // Vérifier l'ordre des TypeSonde (Temperature=0, Hydrometrie=1, QualiteAir=2)
        // TEMPÉRATURE (3 unités) - triées par nom
        Assert.Equal(TypeSonde.Temperature, unitesList[0].TypeSonde);
        Assert.Equal("Celsius", unitesList[0].Nom);
        Assert.Equal(TypeSonde.Temperature, unitesList[1].TypeSonde);
        Assert.Equal("Fahrenheit", unitesList[1].Nom);
        Assert.Equal(TypeSonde.Temperature, unitesList[2].TypeSonde);
        Assert.Equal("Kelvin", unitesList[2].Nom);

        // HYDROMÉTRIE (1 unité)
        Assert.Equal(TypeSonde.Hydrometrie, unitesList[3].TypeSonde);
        Assert.Equal("Pourcentage", unitesList[3].Nom);

        // QUALITÉ AIR (3 unités) - triées par nom
        Assert.Equal(TypeSonde.QualiteAir, unitesList[4].TypeSonde);
        Assert.Equal("Indice de qualité de l'air", unitesList[4].Nom);
        Assert.Equal(TypeSonde.QualiteAir, unitesList[5].TypeSonde);
        Assert.Equal("Microgrammes par mètre cube", unitesList[5].Nom);
        Assert.Equal(TypeSonde.QualiteAir, unitesList[6].TypeSonde);
        Assert.Equal("Parties par million", unitesList[6].Nom);
    }

    [Fact]
    public async Task GetByTypeSondeAsync_Temperature_DevraitRetourner3Unites()
    {
        // Arrange - Les unités de température sont seedées (Celsius, Fahrenheit, Kelvin)

        // Act - Récupérer les unités de température
        var unites = await _repository.GetByTypeSondeAsync(TypeSonde.Temperature);
        var unitesList = unites.ToList();

        // Assert - Vérifier que les 3 unités de température sont retournées
        Assert.Equal(3, unitesList.Count);
        Assert.All(unitesList, u => Assert.Equal(TypeSonde.Temperature, u.TypeSonde));

        // Vérifier le tri par nom
        Assert.Equal("Celsius", unitesList[0].Nom);
        Assert.Equal("°C", unitesList[0].Symbole);
        Assert.Equal("Fahrenheit", unitesList[1].Nom);
        Assert.Equal("°F", unitesList[1].Symbole);
        Assert.Equal("Kelvin", unitesList[2].Nom);
        Assert.Equal("K", unitesList[2].Symbole);
    }

    [Fact]
    public async Task GetByTypeSondeAsync_Hydrometrie_DevraitRetourner1Unite()
    {
        // Arrange - L'unité d'hydrométrie est seedée (Pourcentage)

        // Act - Récupérer les unités d'hydrométrie
        var unites = await _repository.GetByTypeSondeAsync(TypeSonde.Hydrometrie);
        var unitesList = unites.ToList();

        // Assert - Vérifier qu'une seule unité d'hydrométrie est retournée
        Assert.Single(unitesList);
        Assert.Equal(TypeSonde.Hydrometrie, unitesList[0].TypeSonde);
        Assert.Equal("Pourcentage", unitesList[0].Nom);
        Assert.Equal("%", unitesList[0].Symbole);
    }

    [Fact]
    public async Task GetByTypeSondeAsync_QualiteAir_DevraitRetourner3Unites()
    {
        // Arrange - Les unités de qualité d'air sont seedées (Particules, PPM, IQA)

        // Act - Récupérer les unités de qualité d'air
        var unites = await _repository.GetByTypeSondeAsync(TypeSonde.QualiteAir);
        var unitesList = unites.ToList();

        // Assert - Vérifier que les 3 unités de qualité d'air sont retournées
        Assert.Equal(3, unitesList.Count);
        Assert.All(unitesList, u => Assert.Equal(TypeSonde.QualiteAir, u.TypeSonde));

        // Vérifier le tri par nom
        Assert.Equal("Indice de qualité de l'air", unitesList[0].Nom);
        Assert.Equal("IQA", unitesList[0].Symbole);
        Assert.Equal("Microgrammes par mètre cube", unitesList[1].Nom);
        Assert.Equal("µg/m³", unitesList[1].Symbole); // Vérifier les caractères spéciaux
        Assert.Equal("Parties par million", unitesList[2].Nom);
        Assert.Equal("ppm", unitesList[2].Symbole);
    }

    [Fact]
    public async Task CaracteresSpeciaux_DevraitEtrePreserves()
    {
        // Arrange - Les unités avec caractères spéciaux sont déjà seedées

        // Act - Récupérer les unités avec caractères spéciaux
        var celsius = await _repository.GetByIdAsync(_celsiusId);
        var fahrenheit = await _repository.GetByIdAsync(_fahrenheitId);
        var pourcentage = await _repository.GetByIdAsync(_pourcentageId);
        var particules = await _repository.GetByIdAsync(_particulesId);

        // Assert - Vérifier que tous les caractères spéciaux sont préservés
        Assert.NotNull(celsius);
        Assert.Equal("°C", celsius.Symbole); // Degré

        Assert.NotNull(fahrenheit);
        Assert.Equal("°F", fahrenheit.Symbole); // Degré

        Assert.NotNull(pourcentage);
        Assert.Equal("%", pourcentage.Symbole); // Pourcentage

        Assert.NotNull(particules);
        Assert.Equal("µg/m³", particules.Symbole); // Micro + Exposant 3

        // Vérifier également via GetByTypeSondeAsync
        var unitesQualiteAir = await _repository.GetByTypeSondeAsync(TypeSonde.QualiteAir);
        var particulesFromList = unitesQualiteAir.FirstOrDefault(u => u.Id == _particulesId);
        Assert.NotNull(particulesFromList);
        Assert.Equal("µg/m³", particulesFromList.Symbole);
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
