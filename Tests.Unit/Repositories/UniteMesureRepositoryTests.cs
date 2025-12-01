using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour le UniteMesureRepository.
/// Utilise InMemoryDatabase pour tester les opérations CRUD sans dépendance à une vraie base de données.
/// </summary>
public class UniteMesureRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUniteMesureRepository _repository;
    private readonly string _databaseName;

    public UniteMesureRepositoryTests()
    {
        // Crée une base de données InMemory avec un nom unique par test (évite les conflits)
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new UniteMesureRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterUniteMesure()
    {
        // Arrange - Préparer une unité de mesure sans Id ni DateCreation
        var unite = new UniteMesure
        {
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature
        };

        // Act - Ajouter l'unité de mesure
        var result = await _repository.AddAsync(unite);

        // Assert - Vérifier que l'Id et la DateCreation ont été générés
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.DateCreation);
        Assert.Equal("Celsius", result.Nom);
        Assert.Equal("°C", result.Symbole);
        Assert.Equal(TypeSonde.Temperature, result.TypeSonde);
    }

    [Fact]
    public async Task AddAsync_AvecTypeSondeTemperature_DevraitReussir()
    {
        // Arrange - Créer une unité pour la température
        var unite = new UniteMesure
        {
            Nom = "Fahrenheit",
            Symbole = "°F",
            TypeSonde = TypeSonde.Temperature
        };

        // Act - Ajouter l'unité
        var result = await _repository.AddAsync(unite);

        // Assert - Vérifier le type de sonde
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TypeSonde.Temperature, result.TypeSonde);
        Assert.Equal("Fahrenheit", result.Nom);
    }

    [Fact]
    public async Task AddAsync_AvecTypeSondeHydrometrie_DevraitReussir()
    {
        // Arrange - Créer une unité pour l'hydrométrie
        var unite = new UniteMesure
        {
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie
        };

        // Act - Ajouter l'unité
        var result = await _repository.AddAsync(unite);

        // Assert - Vérifier le type de sonde
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TypeSonde.Hydrometrie, result.TypeSonde);
        Assert.Equal("Pourcentage", result.Nom);
    }

    [Fact]
    public async Task AddAsync_AvecTypeSondeQualiteAir_DevraitReussir()
    {
        // Arrange - Créer une unité pour la qualité de l'air
        var unite = new UniteMesure
        {
            Nom = "Microgrammes par mètre cube",
            Symbole = "µg/m³",
            TypeSonde = TypeSonde.QualiteAir
        };

        // Act - Ajouter l'unité
        var result = await _repository.AddAsync(unite);

        // Assert - Vérifier le type de sonde et les caractères spéciaux
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TypeSonde.QualiteAir, result.TypeSonde);
        Assert.Equal("Microgrammes par mètre cube", result.Nom);
        Assert.Equal("µg/m³", result.Symbole);
    }

    [Fact]
    public async Task AddAsync_AvecCaracteresSpeciaux_DevraitPreserverSymboles()
    {
        // Arrange - Créer des unités avec différents caractères spéciaux
        var celsius = new UniteMesure
        {
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature
        };

        var pourcentage = new UniteMesure
        {
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie
        };

        var particules = new UniteMesure
        {
            Nom = "Particules",
            Symbole = "µg/m³",
            TypeSonde = TypeSonde.QualiteAir
        };

        // Act - Ajouter les unités
        var resultCelsius = await _repository.AddAsync(celsius);
        var resultPourcentage = await _repository.AddAsync(pourcentage);
        var resultParticules = await _repository.AddAsync(particules);

        // Assert - Vérifier que tous les caractères spéciaux sont préservés
        Assert.Equal("°C", resultCelsius.Symbole);
        Assert.Equal("%", resultPourcentage.Symbole);
        Assert.Equal("µg/m³", resultParticules.Symbole);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerUnite_QuandExiste()
    {
        // Arrange - Créer et ajouter une unité de mesure
        var unite = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Kelvin",
            Symbole = "K",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(unite);

        // Act - Récupérer l'unité par son Id
        var result = await _repository.GetByIdAsync(unite.Id);

        // Assert - Vérifier que l'unité est trouvée
        Assert.NotNull(result);
        Assert.Equal(unite.Id, result.Id);
        Assert.Equal("Kelvin", result.Nom);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Tenter de récupérer une unité inexistante
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert - Vérifier que le résultat est null
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerToutesLesUnites_TrieesParTypeSondePuisNom()
    {
        // Arrange - Créer et ajouter plusieurs unités de différents types
        var fahrenheit = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Fahrenheit",
            Symbole = "°F",
            TypeSonde = TypeSonde.Temperature, // 0
            DateCreation = DateTime.UtcNow
        };

        var celsius = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature, // 0
            DateCreation = DateTime.UtcNow
        };

        var pourcentage = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie, // 1
            DateCreation = DateTime.UtcNow
        };

        var ppm = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Parties par million",
            Symbole = "ppm",
            TypeSonde = TypeSonde.QualiteAir, // 2
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(fahrenheit);
        await _repository.AddAsync(celsius);
        await _repository.AddAsync(pourcentage);
        await _repository.AddAsync(ppm);

        // Act - Récupérer toutes les unités
        var result = await _repository.GetAllAsync();
        var unitesList = result.ToList();

        // Assert - Vérifier le tri par TypeSonde puis Nom
        Assert.Equal(4, unitesList.Count);

        // TypeSonde.Temperature (0) en premier, trié par nom (Celsius < Fahrenheit)
        Assert.Equal("Celsius", unitesList[0].Nom);
        Assert.Equal(TypeSonde.Temperature, unitesList[0].TypeSonde);
        Assert.Equal("Fahrenheit", unitesList[1].Nom);
        Assert.Equal(TypeSonde.Temperature, unitesList[1].TypeSonde);

        // TypeSonde.Hydrometrie (1) ensuite
        Assert.Equal("Pourcentage", unitesList[2].Nom);
        Assert.Equal(TypeSonde.Hydrometrie, unitesList[2].TypeSonde);

        // TypeSonde.QualiteAir (2) en dernier
        Assert.Equal("Parties par million", unitesList[3].Nom);
        Assert.Equal(TypeSonde.QualiteAir, unitesList[3].TypeSonde);
    }

    [Fact]
    public async Task GetByTypeSondeAsync_DevraitRetournerSeulementTemperature()
    {
        // Arrange - Créer des unités de différents types
        var celsius = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        var fahrenheit = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Fahrenheit",
            Symbole = "°F",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        var pourcentage = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(celsius);
        await _repository.AddAsync(fahrenheit);
        await _repository.AddAsync(pourcentage);

        // Act - Récupérer uniquement les unités de température
        var result = await _repository.GetByTypeSondeAsync(TypeSonde.Temperature);
        var unitesList = result.ToList();

        // Assert - Vérifier que seules les unités de température sont retournées
        Assert.Equal(2, unitesList.Count);
        Assert.All(unitesList, u => Assert.Equal(TypeSonde.Temperature, u.TypeSonde));

        // Vérifier le tri par nom
        Assert.Equal("Celsius", unitesList[0].Nom);
        Assert.Equal("Fahrenheit", unitesList[1].Nom);
    }

    [Fact]
    public async Task GetByTypeSondeAsync_DevraitRetournerSeulementHydrometrie()
    {
        // Arrange - Créer des unités de différents types
        var pourcentage = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };

        var celsius = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(pourcentage);
        await _repository.AddAsync(celsius);

        // Act - Récupérer uniquement les unités d'hydrométrie
        var result = await _repository.GetByTypeSondeAsync(TypeSonde.Hydrometrie);
        var unitesList = result.ToList();

        // Assert - Vérifier que seules les unités d'hydrométrie sont retournées
        Assert.Single(unitesList);
        Assert.Equal(TypeSonde.Hydrometrie, unitesList[0].TypeSonde);
        Assert.Equal("Pourcentage", unitesList[0].Nom);
    }

    [Fact]
    public async Task GetByTypeSondeAsync_DevraitRetournerSeulementQualiteAir()
    {
        // Arrange - Créer des unités de différents types
        var particules = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Microgrammes par mètre cube",
            Symbole = "µg/m³",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow
        };

        var ppm = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Parties par million",
            Symbole = "ppm",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow
        };

        var celsius = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(particules);
        await _repository.AddAsync(ppm);
        await _repository.AddAsync(celsius);

        // Act - Récupérer uniquement les unités de qualité d'air
        var result = await _repository.GetByTypeSondeAsync(TypeSonde.QualiteAir);
        var unitesList = result.ToList();

        // Assert - Vérifier que seules les unités de qualité d'air sont retournées
        Assert.Equal(2, unitesList.Count);
        Assert.All(unitesList, u => Assert.Equal(TypeSonde.QualiteAir, u.TypeSonde));

        // Vérifier le tri par nom
        Assert.Equal("Microgrammes par mètre cube", unitesList[0].Nom);
        Assert.Equal("Parties par million", unitesList[1].Nom);
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourUnite()
    {
        // Arrange - Créer et ajouter une unité
        var unite = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Ancien Nom",
            Symbole = "old",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(unite);

        // Modifier le nom et le symbole
        unite.Nom = "Nouveau Nom";
        unite.Symbole = "new";

        // Act - Mettre à jour l'unité
        await _repository.UpdateAsync(unite);

        // Assert - Vérifier que la modification est persistée
        var updatedUnite = await _repository.GetByIdAsync(unite.Id);
        Assert.NotNull(updatedUnite);
        Assert.Equal("Nouveau Nom", updatedUnite.Nom);
        Assert.Equal("new", updatedUnite.Symbole);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerUnite()
    {
        // Arrange - Créer et ajouter une unité
        var unite = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Unité à supprimer",
            Symbole = "del",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(unite);

        // Act - Supprimer l'unité
        await _repository.DeleteAsync(unite.Id);

        // Assert - Vérifier que l'unité n'existe plus
        var deletedUnite = await _repository.GetByIdAsync(unite.Id);
        Assert.Null(deletedUnite);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandUniteExiste()
    {
        // Arrange - Créer et ajouter une unité
        var unite = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Unité Existante",
            Symbole = "exist",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(unite);

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(unite.Id);

        // Assert - L'unité doit exister
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerFalse_QuandUniteNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(nonExistentId);

        // Assert - L'unité ne doit pas exister
        Assert.False(exists);
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
