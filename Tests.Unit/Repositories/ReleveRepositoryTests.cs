using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour ReleveRepository.
/// Valide tous les scénarios CRUD et requêtes spécifiques avec une base InMemory.
/// </summary>
public class ReleveRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IReleveRepository _repository;
    private readonly string _databaseName;

    public ReleveRepositoryTests()
    {
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new ReleveRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterReleve_AvecIdGenere()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Salon" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Température",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve = new Releve
        {
            SondeId = sonde.Id,
            Valeur = 23.5m,
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Automatique
        };

        // Act
        var result = await _repository.AddAsync(releve);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(23.5m, result.Valeur);
        Assert.Equal(TypeReleve.Automatique, result.TypeReleve);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerReleve_AvecEagerLoading()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Cuisine" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Pourcentage", Symbole = "%", TypeSonde = TypeSonde.Hydrometrie };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve = new Releve
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            Valeur = 65.8m,
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Manuel
        };
        _context.Releves.Add(releve);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(releve.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(releve.Id, result.Id);
        Assert.Equal(65.8m, result.Valeur);
        Assert.NotNull(result.Sonde);
        Assert.Equal("Capteur Humidité", result.Sonde.Nom);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_SiIdInexistant()
    {
        // Arrange
        var idInexistant = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(idInexistant);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerTousLesReleves_TriesParDateDecroissante()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Bureau" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "PPM", Symbole = "ppm", TypeSonde = TypeSonde.QualiteAir };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur CO2",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.QualiteAir,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve1 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 400m, DateHeure = DateTime.UtcNow.AddHours(-2), TypeReleve = TypeReleve.Automatique };
        var releve2 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 450m, DateHeure = DateTime.UtcNow.AddHours(-1), TypeReleve = TypeReleve.Automatique };
        var releve3 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 420m, DateHeure = DateTime.UtcNow, TypeReleve = TypeReleve.Manuel };
        _context.Releves.AddRange(releve1, releve2, releve3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var liste = result.ToList();

        // Assert
        Assert.Equal(3, liste.Count);
        Assert.Equal(releve3.Id, liste[0].Id); // Plus récent en premier
        Assert.Equal(releve2.Id, liste[1].Id);
        Assert.Equal(releve1.Id, liste[2].Id);
    }

    [Fact]
    public async Task UpdateAsync_DevraitModifierReleve()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Chambre" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Température",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve = new Releve
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            Valeur = 20.0m,
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Automatique
        };
        _context.Releves.Add(releve);
        await _context.SaveChangesAsync();

        // Act
        releve.Valeur = 22.5m;
        releve.TypeReleve = TypeReleve.Manuel;
        await _repository.UpdateAsync(releve);

        // Assert
        var updated = await _context.Releves.FindAsync(releve.Id);
        Assert.NotNull(updated);
        Assert.Equal(22.5m, updated.Valeur);
        Assert.Equal(TypeReleve.Manuel, updated.TypeReleve);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerReleve()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Garage" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Pourcentage", Symbole = "%", TypeSonde = TypeSonde.Hydrometrie };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve = new Releve
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            Valeur = 70.0m,
            DateHeure = DateTime.UtcNow,
            TypeReleve = TypeReleve.Automatique
        };
        _context.Releves.Add(releve);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(releve.Id);

        // Assert
        var deleted = await _context.Releves.FindAsync(releve.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_NeFaitRien_SiIdInexistant()
    {
        // Arrange
        var idInexistant = Guid.NewGuid();

        // Act & Assert - Ne doit pas lancer d'exception
        await _repository.DeleteAsync(idInexistant);
    }

    [Fact]
    public async Task GetBySondeAsync_DevraitRetournerRelevesUniquementPourSondeSpecifiee()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Jardin" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde1 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Ext 1",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var sonde2 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Ext 2",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.AddRange(sonde1, sonde2);
        await _context.SaveChangesAsync();

        var releve1 = new Releve { Id = Guid.NewGuid(), SondeId = sonde1.Id, Valeur = 15.0m, DateHeure = DateTime.UtcNow.AddHours(-1), TypeReleve = TypeReleve.Automatique };
        var releve2 = new Releve { Id = Guid.NewGuid(), SondeId = sonde1.Id, Valeur = 16.0m, DateHeure = DateTime.UtcNow, TypeReleve = TypeReleve.Automatique };
        var releve3 = new Releve { Id = Guid.NewGuid(), SondeId = sonde2.Id, Valeur = 14.0m, DateHeure = DateTime.UtcNow, TypeReleve = TypeReleve.Automatique };
        _context.Releves.AddRange(releve1, releve2, releve3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySondeAsync(sonde1.Id);
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, r => Assert.Equal(sonde1.Id, r.SondeId));
        Assert.Equal(releve2.Id, liste[0].Id); // Plus récent en premier
        Assert.Equal(releve1.Id, liste[1].Id);
    }

    [Fact]
    public async Task GetBySondeDateRangeAsync_DevraitFiltrerParPlageDeDates()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Cave" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Cave",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var dateDebut = DateTime.UtcNow.Date;
        var dateFin = dateDebut.AddDays(1);

        var releve1 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 12.0m, DateHeure = dateDebut.AddHours(-2), TypeReleve = TypeReleve.Automatique }; // Avant plage
        var releve2 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 13.0m, DateHeure = dateDebut.AddHours(6), TypeReleve = TypeReleve.Automatique }; // Dans plage
        var releve3 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 14.0m, DateHeure = dateDebut.AddHours(12), TypeReleve = TypeReleve.Manuel }; // Dans plage
        var releve4 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 15.0m, DateHeure = dateFin.AddHours(2), TypeReleve = TypeReleve.Automatique }; // Après plage
        _context.Releves.AddRange(releve1, releve2, releve3, releve4);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySondeDateRangeAsync(sonde.Id, dateDebut, dateFin);
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.Contains(liste, r => r.Id == releve2.Id);
        Assert.Contains(liste, r => r.Id == releve3.Id);
        Assert.DoesNotContain(liste, r => r.Id == releve1.Id);
        Assert.DoesNotContain(liste, r => r.Id == releve4.Id);
    }

    [Fact]
    public async Task GetLastBySondeAsync_DevraitRetournerRelevePlusRecent()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Terrasse" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Pourcentage", Symbole = "%", TypeSonde = TypeSonde.Hydrometrie };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Ext",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve1 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 60.0m, DateHeure = DateTime.UtcNow.AddHours(-3), TypeReleve = TypeReleve.Automatique };
        var releve2 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 65.0m, DateHeure = DateTime.UtcNow.AddHours(-1), TypeReleve = TypeReleve.Automatique };
        var releve3 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 62.0m, DateHeure = DateTime.UtcNow, TypeReleve = TypeReleve.Manuel };
        _context.Releves.AddRange(releve1, releve2, releve3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLastBySondeAsync(sonde.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(releve3.Id, result.Id);
        Assert.Equal(62.0m, result.Valeur);
    }

    [Fact]
    public async Task GetLastBySondeAsync_DevraitRetournerNull_SiAucunReleve()
    {
        // Arrange
        var sondeId = Guid.NewGuid();

        // Act
        var result = await _repository.GetLastBySondeAsync(sondeId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTypeAsync_DevraitFiltrerParTypeManuel()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Atelier" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "PPM", Symbole = "ppm", TypeSonde = TypeSonde.QualiteAir };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Qualité Air",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.QualiteAir,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve1 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 300m, DateHeure = DateTime.UtcNow.AddHours(-2), TypeReleve = TypeReleve.Manuel };
        var releve2 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 350m, DateHeure = DateTime.UtcNow.AddHours(-1), TypeReleve = TypeReleve.Automatique };
        var releve3 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 320m, DateHeure = DateTime.UtcNow, TypeReleve = TypeReleve.Manuel };
        _context.Releves.AddRange(releve1, releve2, releve3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTypeAsync(TypeReleve.Manuel);
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, r => Assert.Equal(TypeReleve.Manuel, r.TypeReleve));
        Assert.Equal(releve3.Id, liste[0].Id); // Plus récent en premier
        Assert.Equal(releve1.Id, liste[1].Id);
    }

    [Fact]
    public async Task GetByTypeAsync_DevraitFiltrerParTypeAutomatique()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Salle de bain" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Pourcentage", Symbole = "%", TypeSonde = TypeSonde.Hydrometrie };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve1 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 80.0m, DateHeure = DateTime.UtcNow.AddHours(-1), TypeReleve = TypeReleve.Automatique };
        var releve2 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 85.0m, DateHeure = DateTime.UtcNow, TypeReleve = TypeReleve.Manuel };
        _context.Releves.AddRange(releve1, releve2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTypeAsync(TypeReleve.Automatique);
        var liste = result.ToList();

        // Assert
        Assert.Single(liste);
        Assert.Equal(TypeReleve.Automatique, liste[0].TypeReleve);
        Assert.Equal(releve1.Id, liste[0].Id);
    }

    [Fact]
    public async Task CountBySondeAsync_DevraitRetournerNombreCorrect()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Grenier" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Grenier",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        await _context.SaveChangesAsync();

        var releve1 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 30.0m, DateHeure = DateTime.UtcNow.AddHours(-2), TypeReleve = TypeReleve.Automatique };
        var releve2 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 32.0m, DateHeure = DateTime.UtcNow.AddHours(-1), TypeReleve = TypeReleve.Automatique };
        var releve3 = new Releve { Id = Guid.NewGuid(), SondeId = sonde.Id, Valeur = 35.0m, DateHeure = DateTime.UtcNow, TypeReleve = TypeReleve.Manuel };
        _context.Releves.AddRange(releve1, releve2, releve3);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.CountBySondeAsync(sonde.Id);

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task CountBySondeAsync_DevraitRetournerZero_SiAucunReleve()
    {
        // Arrange
        var sondeId = Guid.NewGuid();

        // Act
        var count = await _repository.CountBySondeAsync(sondeId);

        // Assert
        Assert.Equal(0, count);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
