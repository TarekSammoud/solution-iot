using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour SeuilAlerteRepository.
/// Valide tous les scénarios CRUD et requêtes spécifiques avec une base InMemory.
/// Teste toutes les combinaisons TypeSeuil × TypeAlerte (4 combinaisons).
/// </summary>
public class SeuilAlerteRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISeuilAlerteRepository _repository;
    private readonly string _databaseName;

    public SeuilAlerteRepositoryTests()
    {
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new SeuilAlerteRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterSeuilAlerte()
    {
        // Arrange - Créer les entités prérequises
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

        var seuilAlerte = new SeuilAlerte
        {
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true
        };

        // Act
        var result = await _repository.AddAsync(seuilAlerte);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default, result.DateCreation);
        Assert.Equal(15.0m, result.Valeur);
        Assert.Equal(TypeSeuil.Minimum, result.TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, result.TypeAlerte);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerSeuil_AvecEagerLoading()
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

        var seuilAlerte = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 70.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(seuilAlerte.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(seuilAlerte.Id, result.Id);
        Assert.Equal(70.0m, result.Valeur);
        Assert.NotNull(result.Sonde);
        Assert.Equal("Capteur Humidité", result.Sonde.Nom);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange
        var idInexistant = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(idInexistant);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerTousLesSeuils()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Bureau" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Bureau",
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

        var seuil1 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 15.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil2 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 30.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil3 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 18.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.SeuilsAlerte.AddRange(seuil1, seuil2, seuil3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var liste = result.ToList();

        // Assert
        Assert.Equal(3, liste.Count);
    }

    [Fact]
    public async Task GetAllAsync_DevraitTrierParSondeTypSeuilTypeAlerte()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Chambre" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Chambre",
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

        // Ajouter dans le désordre
        var seuil1 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 26.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil2 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 15.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil3 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 30.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil4 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 18.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.SeuilsAlerte.AddRange(seuil1, seuil2, seuil3, seuil4);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var liste = result.ToList();

        // Assert - Vérifier l'ordre : Min Alerte, Min Avert, Max Alerte, Max Avert
        Assert.Equal(4, liste.Count);
        Assert.Equal(seuil2.Id, liste[0].Id); // Minimum Alerte
        Assert.Equal(seuil4.Id, liste[1].Id); // Minimum Avertissement
        Assert.Equal(seuil3.Id, liste[2].Id); // Maximum Alerte
        Assert.Equal(seuil1.Id, liste[3].Id); // Maximum Avertissement
    }

    [Fact]
    public async Task GetBySondeAsync_DevraitRetournerSeulementSeuilsDeLaSonde()
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

        var seuil1 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde1.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 10.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil2 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde1.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 35.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil3 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde2.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 5.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.SeuilsAlerte.AddRange(seuil1, seuil2, seuil3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySondeAsync(sonde1.Id);
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, s => Assert.Equal(sonde1.Id, s.SondeId));
    }

    [Fact]
    public async Task GetBySondeAsync_DevraitTrierParTypeSeuilPuisTypeAlerte()
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

        // Ajouter dans le désordre
        var seuil1 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 22.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil2 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 14.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuil3 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 12.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.SeuilsAlerte.AddRange(seuil1, seuil2, seuil3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySondeAsync(sonde.Id);
        var liste = result.ToList();

        // Assert - Vérifier l'ordre
        Assert.Equal(3, liste.Count);
        Assert.Equal(seuil3.Id, liste[0].Id); // Minimum Alerte
        Assert.Equal(seuil2.Id, liste[1].Id); // Minimum Avertissement
        Assert.Equal(seuil1.Id, liste[2].Id); // Maximum Alerte
    }

    [Fact]
    public async Task GetActifsAsync_DevraitRetournerSeulementSeuilsActifs()
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

        var seuilActif1 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 30.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuilActif2 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 80.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuilInactif = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 35.0m, EstActif = false, DateCreation = DateTime.UtcNow };
        _context.SeuilsAlerte.AddRange(seuilActif1, seuilActif2, seuilInactif);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActifsAsync();
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, s => Assert.True(s.EstActif));
        Assert.DoesNotContain(liste, s => s.Id == seuilInactif.Id);
    }

    [Fact]
    public async Task GetBySondeActifsAsync_DevraitRetournerSeuilsActifsDeLaSonde()
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

        var seuilActif1 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 1000.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuilActif2 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 800.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuilInactif = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 200.0m, EstActif = false, DateCreation = DateTime.UtcNow };
        _context.SeuilsAlerte.AddRange(seuilActif1, seuilActif2, seuilInactif);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySondeActifsAsync(sonde.Id);
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, s =>
        {
            Assert.Equal(sonde.Id, s.SondeId);
            Assert.True(s.EstActif);
        });
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterSeuilMinimumAlerte()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Test1" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Test Sonde",
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

        var seuil = new SeuilAlerte
        {
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true
        };

        // Act
        var result = await _repository.AddAsync(seuil);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TypeSeuil.Minimum, result.TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, result.TypeAlerte);
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterSeuilMaximumAlerte()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Test2" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Test Sonde",
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

        var seuil = new SeuilAlerte
        {
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 30.0m,
            EstActif = true
        };

        // Act
        var result = await _repository.AddAsync(seuil);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TypeSeuil.Maximum, result.TypeSeuil);
        Assert.Equal(TypeAlerte.Alerte, result.TypeAlerte);
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterSeuilMinimumAvertissement()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Test3" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Test Sonde",
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

        var seuil = new SeuilAlerte
        {
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 18.0m,
            EstActif = true
        };

        // Act
        var result = await _repository.AddAsync(seuil);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TypeSeuil.Minimum, result.TypeSeuil);
        Assert.Equal(TypeAlerte.Avertissement, result.TypeAlerte);
    }

    [Fact]
    public async Task AddAsync_DevraitAccepterSeuilMaximumAvertissement()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Test4" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Test Sonde",
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

        var seuil = new SeuilAlerte
        {
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 26.0m,
            EstActif = true
        };

        // Act
        var result = await _repository.AddAsync(seuil);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TypeSeuil.Maximum, result.TypeSeuil);
        Assert.Equal(TypeAlerte.Avertissement, result.TypeAlerte);
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourSeuil()
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

        var seuil = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 80.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.SeuilsAlerte.Add(seuil);
        await _context.SaveChangesAsync();

        // Act
        seuil.Valeur = 85.0m;
        seuil.EstActif = false;
        await _repository.UpdateAsync(seuil);

        // Assert
        var updated = await _context.SeuilsAlerte.FindAsync(seuil.Id);
        Assert.NotNull(updated);
        Assert.Equal(85.0m, updated.Valeur);
        Assert.False(updated.EstActif);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerSeuil()
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

        var seuil = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 5.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.SeuilsAlerte.Add(seuil);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(seuil.Id);

        // Assert
        var deleted = await _context.SeuilsAlerte.FindAsync(seuil.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandSeuilExiste()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Couloir" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Couloir",
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

        var seuil = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 25.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.SeuilsAlerte.Add(seuil);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(seuil.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerFalse_QuandSeuilNExistePas()
    {
        // Arrange
        var idInexistant = Guid.NewGuid();

        // Act
        var exists = await _repository.ExistsAsync(idInexistant);

        // Assert
        Assert.False(exists);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
