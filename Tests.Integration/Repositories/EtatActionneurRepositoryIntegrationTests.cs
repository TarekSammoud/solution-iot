using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.Integration.Repositories;

/// <summary>
/// Tests d'intégration pour EtatActionneurRepository.
/// Teste les scénarios complets avec données seed et vérification des contraintes.
/// Utilise InMemoryDatabase avec nom unique par test.
/// </summary>
public class EtatActionneurRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IEtatActionneurRepository _repository;
    private readonly string _databaseName;

    // Données seed
    private readonly Localisation localisation;
    private readonly Actionneur ampouleSimpleSalon;
    private readonly Actionneur ampouleVariometreCuisine;
    private readonly Actionneur moteurGarage;
    private readonly Actionneur ampouleSimpleChambre;
    private readonly Actionneur ampouleVariometreBureau; // Sans état
    private readonly List<EtatActionneur> etats;

    public EtatActionneurRepositoryIntegrationTests()
    {
        // Nom de base unique
        _databaseName = $"TestDb_Integration_{Guid.NewGuid()}";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new EtatActionneurRepository(_context);

        // Seed des données de test
        // 1. Créer localisation
        localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Maison Test",
            Description = "Maison de test pour les états actionneurs",
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.SaveChanges();

        // 2. Créer 5 actionneurs variés
        ampouleSimpleSalon = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Simple Salon",
            LocalisationId = localisation.Id,
            EstActif = true,
            DateInstallation = DateTime.UtcNow,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPush,
            TypeActionneur = TypeActionneur.AmpouleSimple
        };

        ampouleVariometreCuisine = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Variomètre Cuisine",
            LocalisationId = localisation.Id,
            EstActif = true,
            DateInstallation = DateTime.UtcNow,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPull,
            TypeActionneur = TypeActionneur.AmpouleVariometre
        };

        moteurGarage = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Moteur Garage",
            LocalisationId = localisation.Id,
            EstActif = true,
            DateInstallation = DateTime.UtcNow,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPush,
            TypeActionneur = TypeActionneur.Moteur
        };

        ampouleSimpleChambre = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Simple Chambre",
            LocalisationId = localisation.Id,
            EstActif = true,
            DateInstallation = DateTime.UtcNow,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPush,
            TypeActionneur = TypeActionneur.AmpouleSimple
        };

        ampouleVariometreBureau = new Actionneur
        {
            Id = Guid.NewGuid(),
            Nom = "Ampoule Variomètre Bureau",
            LocalisationId = localisation.Id,
            EstActif = true,
            DateInstallation = DateTime.UtcNow,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPull,
            TypeActionneur = TypeActionneur.AmpouleVariometre
        };

        _context.Actionneurs.AddRange(
            ampouleSimpleSalon,
            ampouleVariometreCuisine,
            moteurGarage,
            ampouleSimpleChambre,
            ampouleVariometreBureau
        );
        _context.SaveChanges();

        // 3. Créer 4 états (pas pour bureau)
        etats = new List<EtatActionneur>
        {
            // AmpouleSimple salon : ON (100%)
            new EtatActionneur
            {
                Id = Guid.NewGuid(),
                ActionneurId = ampouleSimpleSalon.Id,
                EstActif = true,
                Pourcentage = 100,
                DerniereModification = DateTime.UtcNow.AddHours(-1)
            },
            // AmpouleVariometre cuisine : ON (75% intensité)
            new EtatActionneur
            {
                Id = Guid.NewGuid(),
                ActionneurId = ampouleVariometreCuisine.Id,
                EstActif = true,
                Pourcentage = 75,
                DerniereModification = DateTime.UtcNow.AddMinutes(-30)
            },
            // Moteur garage : ON (50% vitesse)
            new EtatActionneur
            {
                Id = Guid.NewGuid(),
                ActionneurId = moteurGarage.Id,
                EstActif = true,
                Pourcentage = 50,
                DerniereModification = DateTime.UtcNow.AddMinutes(-10)
            },
            // AmpouleSimple chambre : OFF (0%)
            new EtatActionneur
            {
                Id = Guid.NewGuid(),
                ActionneurId = ampouleSimpleChambre.Id,
                EstActif = false,
                Pourcentage = 0,
                DerniereModification = DateTime.UtcNow
            }
            // Pas d'état pour ampouleVariometreBureau
        };

        _context.EtatsActionneur.AddRange(etats);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Scenario_CRUD_Complet()
    {
        // CREATE
        var nouvelEtat = new EtatActionneur
        {
            ActionneurId = ampouleVariometreBureau.Id, // Bureau n'a pas encore d'état
            EstActif = true,
            Pourcentage = 60
        };

        var etatCree = await _repository.AddAsync(nouvelEtat);
        Assert.NotEqual(Guid.Empty, etatCree.Id);
        Assert.Equal(60, etatCree.Pourcentage);

        // READ
        var etatLu = await _repository.GetByIdAsync(etatCree.Id);
        Assert.NotNull(etatLu);
        Assert.Equal(ampouleVariometreBureau.Id, etatLu.ActionneurId);

        // UPDATE
        etatLu.Pourcentage = 80;
        etatLu.EstActif = false;
        await _repository.UpdateAsync(etatLu);

        var etatMisAJour = await _repository.GetByIdAsync(etatCree.Id);
        Assert.NotNull(etatMisAJour);
        Assert.Equal(80, etatMisAJour.Pourcentage);
        Assert.False(etatMisAJour.EstActif);

        // DELETE
        await _repository.DeleteAsync(etatCree.Id);
        var etatSupprime = await _repository.GetByIdAsync(etatCree.Id);
        Assert.Null(etatSupprime);
    }

    [Fact]
    public async Task Scenario_ChangementEtat_OnToOff()
    {
        // Arrange
        var etatSalon = await _repository.GetByActionneurAsync(ampouleSimpleSalon.Id);
        Assert.NotNull(etatSalon);
        Assert.True(etatSalon.EstActif);

        var ancienneDateModification = etatSalon.DerniereModification;

        // Attendre un peu pour s'assurer que la date change
        await Task.Delay(10);

        // Act - Changement ON → OFF
        etatSalon.EstActif = false;
        etatSalon.Pourcentage = 0;
        await _repository.UpdateAsync(etatSalon);

        // Assert
        var etatMisAJour = await _repository.GetByActionneurAsync(ampouleSimpleSalon.Id);
        Assert.NotNull(etatMisAJour);
        Assert.False(etatMisAJour.EstActif);
        Assert.Equal(0, etatMisAJour.Pourcentage);
        Assert.True(etatMisAJour.DerniereModification > ancienneDateModification);
    }

    [Fact]
    public async Task Scenario_ModificationPourcentage_AmpouleVariometre()
    {
        // Arrange
        var etatCuisine = await _repository.GetByActionneurAsync(ampouleVariometreCuisine.Id);
        Assert.NotNull(etatCuisine);
        Assert.Equal(75, etatCuisine.Pourcentage);

        // Act - Modification intensité 75% → 50%
        etatCuisine.Pourcentage = 50;
        await _repository.UpdateAsync(etatCuisine);

        // Assert
        var etatMisAJour = await _repository.GetByActionneurAsync(ampouleVariometreCuisine.Id);
        Assert.NotNull(etatMisAJour);
        Assert.Equal(50, etatMisAJour.Pourcentage);
        Assert.True(etatMisAJour.EstActif); // Toujours actif
    }

    [Fact]
    public async Task GetByActionneurAsync_DevraitRetournerEtatUnique()
    {
        // Act
        var etatSalon = await _repository.GetByActionneurAsync(ampouleSimpleSalon.Id);

        // Assert
        Assert.NotNull(etatSalon);
        Assert.Equal(ampouleSimpleSalon.Id, etatSalon.ActionneurId);
        Assert.Equal(100, etatSalon.Pourcentage); // AmpouleSimple ON = 100%

        // Vérifier qu'on ne peut avoir qu'un seul état par actionneur
        var tousEtats = await _context.EtatsActionneur
            .Where(e => e.ActionneurId == ampouleSimpleSalon.Id)
            .ToListAsync();
        Assert.Single(tousEtats); // Relation 1-to-1 vérifiée
    }

    [Fact]
    public async Task GetActifsAsync_DevraitRetourner3EtatsActifs()
    {
        // Act
        var etatsActifs = await _repository.GetActifsAsync();

        // Assert
        Assert.Equal(3, etatsActifs.Count()); // Salon, Cuisine, Garage ON
        Assert.All(etatsActifs, e => Assert.True(e.EstActif));

        // Vérifier que chambre (OFF) n'est pas dans la liste
        Assert.DoesNotContain(etatsActifs, e => e.ActionneurId == ampouleSimpleChambre.Id);
    }

    [Fact]
    public async Task EagerLoading_DevraitChargerActionneur()
    {
        // Act
        var etatCuisine = await _repository.GetByActionneurAsync(ampouleVariometreCuisine.Id);

        // Assert
        Assert.NotNull(etatCuisine);
        Assert.NotNull(etatCuisine.Actionneur); // Eager loading vérifié
        Assert.Equal("Ampoule Variomètre Cuisine", etatCuisine.Actionneur.Nom);
        Assert.Equal(TypeActionneur.AmpouleVariometre, etatCuisine.Actionneur.TypeActionneur);
    }

    [Fact]
    public async Task CascadeDelete_SupprimerActionneur_DevraitSupprimerEtat()
    {
        // Arrange
        var etatSalon = await _repository.GetByActionneurAsync(ampouleSimpleSalon.Id);
        Assert.NotNull(etatSalon);

        // Act - Supprimer l'actionneur
        _context.Actionneurs.Remove(ampouleSimpleSalon);
        await _context.SaveChangesAsync();

        // Assert - L'état doit être supprimé automatiquement (cascade delete)
        var etatApresSuppressionActionneur = await _repository.GetByIdAsync(etatSalon.Id);
        Assert.Null(etatApresSuppressionActionneur);
    }

    [Fact]
    public async Task IndexUnique_DeuxEtatsMemeActionneur_DevraitEtrePrevenuParExistsByActionneurAsync()
    {
        // Arrange - L'actionneur salon a déjà un état
        // Note: InMemoryDatabase ne supporte pas les contraintes d'index unique comme SQLite
        // En production, l'index unique empêchera les doublons au niveau de la base de données
        // En test, nous utilisons ExistsByActionneurAsync pour vérifier avant l'insertion

        // Act - Vérifier si un état existe déjà pour cet actionneur
        var existeDeja = await _repository.ExistsByActionneurAsync(ampouleSimpleSalon.Id);

        // Assert - L'état existe déjà, donc on ne devrait pas en créer un nouveau
        Assert.True(existeDeja);

        // Vérifier qu'il n'y a qu'un seul état pour cet actionneur
        var etats = await _context.EtatsActionneur
            .Where(e => e.ActionneurId == ampouleSimpleSalon.Id)
            .ToListAsync();
        Assert.Single(etats); // Relation 1-to-1 vérifiée
    }

    [Fact]
    public async Task UpdateAsync_DevraitAutomatiquementMettreAJourDerniereModification()
    {
        // Arrange
        var etatGarage = await _repository.GetByActionneurAsync(moteurGarage.Id);
        Assert.NotNull(etatGarage);

        var ancienneDateModification = etatGarage.DerniereModification;

        // Attendre un peu pour s'assurer que la date change
        await Task.Delay(10);

        // Act - Modification du pourcentage de vitesse
        etatGarage.Pourcentage = 75; // Augmentation vitesse 50% → 75%
        await _repository.UpdateAsync(etatGarage);

        // Assert
        var etatMisAJour = await _repository.GetByActionneurAsync(moteurGarage.Id);
        Assert.NotNull(etatMisAJour);
        Assert.Equal(75, etatMisAJour.Pourcentage);
        Assert.True(etatMisAJour.DerniereModification > ancienneDateModification);
    }
}
