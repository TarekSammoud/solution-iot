using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour le LocalisationRepository.
/// Utilise InMemoryDatabase pour tester les opérations CRUD sans dépendance à une vraie base de données.
/// </summary>
public class LocalisationRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILocalisationRepository _repository;
    private readonly string _databaseName;

    public LocalisationRepositoryTests()
    {
        // Crée une base de données InMemory avec un nom unique par test (évite les conflits)
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new LocalisationRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterLocalisation()
    {
        // Arrange - Préparer une localisation de test sans Id ni DateCreation
        var localisation = new Localisation
        {
            Nom = "Salon",
            Description = "Pièce principale de la maison"
        };

        // Act - Ajouter la localisation
        var result = await _repository.AddAsync(localisation);

        // Assert - Vérifier que l'Id et la DateCreation ont été générés
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.DateCreation);
        Assert.Equal("Salon", result.Nom);
        Assert.Equal("Pièce principale de la maison", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerLocalisation_QuandExiste()
    {
        // Arrange - Créer et ajouter une localisation de test
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Cuisine",
            Description = "Espace cuisine équipée",
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(localisation);

        // Act - Récupérer la localisation par son Id
        var result = await _repository.GetByIdAsync(localisation.Id);

        // Assert - Vérifier que la localisation est trouvée
        Assert.NotNull(result);
        Assert.Equal(localisation.Id, result.Id);
        Assert.Equal("Cuisine", result.Nom);
        Assert.Equal("Espace cuisine équipée", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas dans la base
        var nonExistentId = Guid.NewGuid();

        // Act - Tenter de récupérer une localisation inexistante
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert - Vérifier que le résultat est null
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerToutesLesLocalisations()
    {
        // Arrange - Créer et ajouter plusieurs localisations
        var salon = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salon",
            Description = "Pièce principale",
            DateCreation = DateTime.UtcNow
        };

        var chambre = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Chambre 1",
            Description = "Chambre à coucher",
            DateCreation = DateTime.UtcNow
        };

        var bureau = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Bureau",
            Description = null, // Description optionnelle
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(salon);
        await _repository.AddAsync(chambre);
        await _repository.AddAsync(bureau);

        // Act - Récupérer toutes les localisations
        var result = await _repository.GetAllAsync();
        var localisationsList = result.ToList();

        // Assert - Vérifier que toutes les localisations sont retournées
        Assert.Equal(3, localisationsList.Count);
        Assert.Contains(localisationsList, l => l.Nom == "Salon");
        Assert.Contains(localisationsList, l => l.Nom == "Chambre 1");
        Assert.Contains(localisationsList, l => l.Nom == "Bureau");
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerLocalisationsTrieesParNom()
    {
        // Arrange - Créer et ajouter des localisations dans un ordre non alphabétique
        var cuisine = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Cuisine",
            Description = "Espace repas",
            DateCreation = DateTime.UtcNow
        };

        var salon = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salon",
            Description = "Pièce principale",
            DateCreation = DateTime.UtcNow
        };

        var bureau = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Bureau",
            Description = "Espace de travail",
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(cuisine);
        await _repository.AddAsync(salon);
        await _repository.AddAsync(bureau);

        // Act - Récupérer toutes les localisations
        var result = await _repository.GetAllAsync();
        var localisationsList = result.ToList();

        // Assert - Vérifier que les localisations sont triées par nom (ordre alphabétique)
        Assert.Equal(3, localisationsList.Count);
        Assert.Equal("Bureau", localisationsList[0].Nom);
        Assert.Equal("Cuisine", localisationsList[1].Nom);
        Assert.Equal("Salon", localisationsList[2].Nom);
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourLocalisation()
    {
        // Arrange - Créer et ajouter une localisation
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Chambre",
            Description = "Ancienne description",
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(localisation);

        // Modifier le nom et la description
        localisation.Nom = "Chambre Principale";
        localisation.Description = "Nouvelle description mise à jour";

        // Act - Mettre à jour la localisation
        await _repository.UpdateAsync(localisation);

        // Assert - Vérifier que la modification est bien persistée
        var updatedLocalisation = await _repository.GetByIdAsync(localisation.Id);
        Assert.NotNull(updatedLocalisation);
        Assert.Equal("Chambre Principale", updatedLocalisation.Nom);
        Assert.Equal("Nouvelle description mise à jour", updatedLocalisation.Description);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerLocalisation()
    {
        // Arrange - Créer et ajouter une localisation
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Garage",
            Description = "Espace de stationnement",
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(localisation);

        // Act - Supprimer la localisation
        await _repository.DeleteAsync(localisation.Id);

        // Assert - Vérifier que la localisation n'existe plus
        var deletedLocalisation = await _repository.GetByIdAsync(localisation.Id);
        Assert.Null(deletedLocalisation);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandLocalisationExiste()
    {
        // Arrange - Créer et ajouter une localisation
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salle de bain",
            Description = "Salle d'eau avec douche",
            DateCreation = DateTime.UtcNow
        };
        await _repository.AddAsync(localisation);

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(localisation.Id);

        // Assert - La localisation doit exister
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerFalse_QuandLocalisationNExistePas()
    {
        // Arrange - Créer un Guid qui n'existe pas
        var nonExistentId = Guid.NewGuid();

        // Act - Vérifier l'existence
        var exists = await _repository.ExistsAsync(nonExistentId);

        // Assert - La localisation ne doit pas exister
        Assert.False(exists);
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterDescriptionNull()
    {
        // Arrange - Créer une localisation avec description null (optionnelle)
        var localisation = new Localisation
        {
            Nom = "Terrasse",
            Description = null // Description optionnelle
        };

        // Act - Ajouter la localisation
        var result = await _repository.AddAsync(localisation);

        // Assert - Vérifier que l'ajout fonctionne avec description null
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Terrasse", result.Nom);
        Assert.Null(result.Description);

        // Vérifier également la récupération
        var retrieved = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(retrieved);
        Assert.Null(retrieved.Description);
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
