using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour le LocalisationRepository.
/// Ces tests vérifient le bon fonctionnement du repository avec une base de données réelle (InMemory).
/// Les données sont seedées au début et utilisées dans tous les tests.
/// </summary>
public class LocalisationRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILocalisationRepository _repository;
    private readonly string _databaseName;

    // IDs des localisations seedées pour référence dans les tests
    private readonly Guid _salonId;
    private readonly Guid _cuisineId;
    private readonly Guid _chambreId;
    private readonly Guid _bureauId;

    public LocalisationRepositoryIntegrationTests()
    {
        // Crée une base de données InMemory avec un nom unique
        _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new LocalisationRepository(_context);

        // Seed des données de test - simulant une base de données pré-remplie
        _salonId = Guid.NewGuid();
        _cuisineId = Guid.NewGuid();
        _chambreId = Guid.NewGuid();
        _bureauId = Guid.NewGuid();

        var salon = new Localisation
        {
            Id = _salonId,
            Nom = "Salon",
            Description = "Pièce principale",
            DateCreation = DateTime.UtcNow.AddDays(-30) // Créé il y a 30 jours
        };

        var cuisine = new Localisation
        {
            Id = _cuisineId,
            Nom = "Cuisine",
            Description = "Espace cuisine équipée",
            DateCreation = DateTime.UtcNow.AddDays(-20) // Créé il y a 20 jours
        };

        var chambre = new Localisation
        {
            Id = _chambreId,
            Nom = "Chambre 1",
            Description = null, // Description null pour tester l'optionnalité
            DateCreation = DateTime.UtcNow.AddDays(-10) // Créé il y a 10 jours
        };

        var bureau = new Localisation
        {
            Id = _bureauId,
            Nom = "Bureau",
            Description = "Espace de travail",
            DateCreation = DateTime.UtcNow.AddDays(-5) // Créé il y a 5 jours
        };

        _context.Localisations.AddRange(salon, cuisine, chambre, bureau);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // Arrange - Préparer une nouvelle localisation
        var nouvelleLocalisation = new Localisation
        {
            Nom = "Garage",
            Description = "Espace de stationnement et rangement"
        };

        // Act & Assert - CREATE
        var createdLocalisation = await _repository.AddAsync(nouvelleLocalisation);
        Assert.NotEqual(Guid.Empty, createdLocalisation.Id);
        Assert.Equal("Garage", createdLocalisation.Nom);

        // Act & Assert - READ
        var readLocalisation = await _repository.GetByIdAsync(createdLocalisation.Id);
        Assert.NotNull(readLocalisation);
        Assert.Equal("Garage", readLocalisation.Nom);
        Assert.Equal("Espace de stationnement et rangement", readLocalisation.Description);

        // Act & Assert - UPDATE
        readLocalisation.Nom = "Grand Garage";
        readLocalisation.Description = "Espace de stationnement pour 2 voitures";
        await _repository.UpdateAsync(readLocalisation);
        var updatedLocalisation = await _repository.GetByIdAsync(createdLocalisation.Id);
        Assert.NotNull(updatedLocalisation);
        Assert.Equal("Grand Garage", updatedLocalisation.Nom);
        Assert.Equal("Espace de stationnement pour 2 voitures", updatedLocalisation.Description);

        // Act & Assert - DELETE
        await _repository.DeleteAsync(createdLocalisation.Id);
        var deletedLocalisation = await _repository.GetByIdAsync(createdLocalisation.Id);
        Assert.Null(deletedLocalisation);

        // Vérifier que les localisations seedées n'ont pas été affectées
        var allLocalisations = await _repository.GetAllAsync();
        Assert.Equal(4, allLocalisations.Count()); // Salon, Cuisine, Chambre, Bureau toujours présents
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerLocalisationsTrieesParNom()
    {
        // Arrange - Les données sont déjà seedées (Salon, Cuisine, Chambre 1, Bureau)

        // Act - Récupérer toutes les localisations
        var localisations = await _repository.GetAllAsync();
        var localisationsList = localisations.ToList();

        // Assert - Vérifier le tri par nom (ordre alphabétique)
        Assert.Equal(4, localisationsList.Count);
        Assert.Equal("Bureau", localisationsList[0].Nom); // 1er alphabétiquement
        Assert.Equal("Chambre 1", localisationsList[1].Nom); // 2ème alphabétiquement
        Assert.Equal("Cuisine", localisationsList[2].Nom); // 3ème alphabétiquement
        Assert.Equal("Salon", localisationsList[3].Nom); // 4ème alphabétiquement

        // Vérifier également que toutes les localisations sont bien présentes
        Assert.Contains(localisationsList, l => l.Id == _salonId);
        Assert.Contains(localisationsList, l => l.Id == _cuisineId);
        Assert.Contains(localisationsList, l => l.Id == _chambreId);
        Assert.Contains(localisationsList, l => l.Id == _bureauId);
    }

    [Fact]
    public async Task UpdateAsync_DevraitModifierNomEtDescription()
    {
        // Arrange - Récupérer la localisation Bureau seedée
        var bureau = await _repository.GetByIdAsync(_bureauId);
        Assert.NotNull(bureau);

        var nomOriginal = bureau.Nom;
        var descriptionOriginale = bureau.Description;

        // Act - Modifier à la fois le nom et la description
        bureau.Nom = "Bureau Principal";
        bureau.Description = "Grand espace de travail avec vue sur le jardin";
        await _repository.UpdateAsync(bureau);

        // Assert - Vérifier que les deux propriétés ont été modifiées
        var bureauModifie = await _repository.GetByIdAsync(_bureauId);
        Assert.NotNull(bureauModifie);
        Assert.Equal("Bureau Principal", bureauModifie.Nom);
        Assert.Equal("Grand espace de travail avec vue sur le jardin", bureauModifie.Description);

        // Vérifier que ce n'est plus l'ancien nom/description
        Assert.NotEqual(nomOriginal, bureauModifie.Nom);
        Assert.NotEqual(descriptionOriginale, bureauModifie.Description);
    }

    [Fact]
    public async Task AddAsync_AvecDescriptionNull_DevraitReussir()
    {
        // Arrange - Créer une localisation avec description null
        var nouvelleLocalisation = new Localisation
        {
            Nom = "Terrasse",
            Description = null // Description explicitement null
        };

        // Act - Ajouter la localisation
        var result = await _repository.AddAsync(nouvelleLocalisation);

        // Assert - Vérifier que l'ajout a réussi
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Terrasse", result.Nom);
        Assert.Null(result.Description);

        // Vérifier également la récupération
        var retrieved = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("Terrasse", retrieved.Nom);
        Assert.Null(retrieved.Description);

        // Vérifier que les autres localisations ne sont pas affectées
        var allLocalisations = await _repository.GetAllAsync();
        Assert.Equal(5, allLocalisations.Count()); // 4 seedées + 1 ajoutée
    }

    [Fact]
    public async Task DeleteAsync_LocalisationInexistante_NeLevePasException()
    {
        // Arrange - Créer un Guid qui n'existe pas dans la base
        var idInexistant = Guid.NewGuid();

        // Act - Tenter de supprimer une localisation inexistante
        // Ne devrait pas lever d'exception
        await _repository.DeleteAsync(idInexistant);

        // Assert - Vérifier que les localisations seedées sont toujours présentes
        var allLocalisations = await _repository.GetAllAsync();
        Assert.Equal(4, allLocalisations.Count());

        // Vérifier que toutes les localisations seedées sont toujours là
        Assert.True(await _repository.ExistsAsync(_salonId));
        Assert.True(await _repository.ExistsAsync(_cuisineId));
        Assert.True(await _repository.ExistsAsync(_chambreId));
        Assert.True(await _repository.ExistsAsync(_bureauId));
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
