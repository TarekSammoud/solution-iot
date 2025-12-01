using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories;

/// <summary>
/// Tests unitaires pour AlerteRepository.
/// Valide tous les scénarios CRUD et requêtes spécifiques avec une base InMemory.
/// Teste en particulier GetAlerteActiveForSeuilAsync qui filtre par TypeAlerte.
/// </summary>
public class AlerteRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAlerteRepository _repository;
    private readonly string _databaseName;

    public AlerteRepositoryTests()
    {
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        _context = new AppDbContext(options);
        _repository = new AlerteRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterAlerte()
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
        var seuilAlerte = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte = new Alerte
        {
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Message = "Température trop basse"
        };

        // Act
        var result = await _repository.AddAsync(alerte);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default, result.DateCreation);
        Assert.Equal("Température trop basse", result.Message);
    }

    [Fact]
    public async Task AddAsync_DevraitDefinirStatutActiveParDefaut()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Cuisine" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Temp",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var seuilAlerte = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Valeur = 28.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte = new Alerte
        {
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement
        };

        // Act
        var result = await _repository.AddAsync(alerte);

        // Assert
        Assert.Equal(StatutAlerte.Active, result.Statut);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerAlerte_AvecEagerLoading()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Bureau" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Pourcentage", Symbole = "%", TypeSonde = TypeSonde.Hydrometrie };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Humidité",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR
        };
        var seuilAlerte = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 30.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerte);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(alerte.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(alerte.Id, result.Id);
        Assert.NotNull(result.Sonde);
        Assert.Equal("Capteur Humidité", result.Sonde.Nom);
        Assert.NotNull(result.SeuilAlerte);
        Assert.Equal(30.0m, result.SeuilAlerte.Valeur);
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
    public async Task GetAllAsync_DevraitRetournerToutesLesAlertes()
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
        var seuilAlerte = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 15.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte1 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerte2 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Acquittee, DateCreation = DateTime.UtcNow };
        var alerte3 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Resolue, DateCreation = DateTime.UtcNow };
        _context.Alertes.AddRange(alerte1, alerte2, alerte3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var liste = result.ToList();

        // Assert
        Assert.Equal(3, liste.Count);
    }

    [Fact]
    public async Task GetAllAsync_DevraitTrierParDateCreationDescendant()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Terrasse" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Ext",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPull
        };
        var seuilAlerte = new SeuilAlerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Valeur = 30.0m,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte1 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow.AddHours(-3) };
        var alerte2 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow.AddHours(-1) };
        var alerte3 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        _context.Alertes.AddRange(alerte1, alerte2, alerte3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var liste = result.ToList();

        // Assert - Vérifier l'ordre (plus récentes en premier)
        Assert.Equal(3, liste.Count);
        Assert.Equal(alerte3.Id, liste[0].Id); // Plus récente
        Assert.Equal(alerte2.Id, liste[1].Id);
        Assert.Equal(alerte1.Id, liste[2].Id); // Plus ancienne
    }

    [Fact]
    public async Task GetBySondeAsync_DevraitRetournerSeulementAlertesDelaSonde()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Jardin" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde1 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur 1",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var sonde2 = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur 2",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var seuilAlerte1 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde1.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 15.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        var seuilAlerte2 = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde2.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 15.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.AddRange(sonde1, sonde2);
        _context.SeuilsAlerte.AddRange(seuilAlerte1, seuilAlerte2);
        await _context.SaveChangesAsync();

        var alerte1 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde1.Id, SeuilAlerteId = seuilAlerte1.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerte2 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde1.Id, SeuilAlerteId = seuilAlerte1.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Resolue, DateCreation = DateTime.UtcNow };
        var alerte3 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde2.Id, SeuilAlerteId = seuilAlerte2.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        _context.Alertes.AddRange(alerte1, alerte2, alerte3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySondeAsync(sonde1.Id);
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, a => Assert.Equal(sonde1.Id, a.SondeId));
    }

    [Fact]
    public async Task GetByStatutAsync_Active_DevraitRetournerSeulementAlertesActives()
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
            CanalCommunication = CanalCommunication.MQTT
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 12.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerteActive1 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerteActive2 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerteAcquittee = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Acquittee, DateCreation = DateTime.UtcNow };
        var alerteResolue = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Resolue, DateCreation = DateTime.UtcNow };
        _context.Alertes.AddRange(alerteActive1, alerteActive2, alerteAcquittee, alerteResolue);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatutAsync(StatutAlerte.Active);
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, a => Assert.Equal(StatutAlerte.Active, a.Statut));
    }

    [Fact]
    public async Task GetByStatutAsync_Acquittee_DevraitRetournerSeulementAlertesAcquitees()
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
            CanalCommunication = CanalCommunication.HttpPush
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 35.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerteActive = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerteAcquittee = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Acquittee, DateCreation = DateTime.UtcNow, DateAcquittement = DateTime.UtcNow };
        _context.Alertes.AddRange(alerteActive, alerteAcquittee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatutAsync(StatutAlerte.Acquittee);
        var liste = result.ToList();

        // Assert
        Assert.Single(liste);
        Assert.Equal(StatutAlerte.Acquittee, liste[0].Statut);
    }

    [Fact]
    public async Task GetByStatutAsync_Resolue_DevraitRetournerSeulementAlertesResolues()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Garage" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Pourcentage", Symbole = "%", TypeSonde = TypeSonde.Hydrometrie };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Garage",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 80.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerteActive = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Avertissement, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerteResolue = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Avertissement, Statut = StatutAlerte.Resolue, DateCreation = DateTime.UtcNow, DateResolution = DateTime.UtcNow };
        _context.Alertes.AddRange(alerteActive, alerteResolue);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatutAsync(StatutAlerte.Resolue);
        var liste = result.ToList();

        // Assert
        Assert.Single(liste);
        Assert.Equal(StatutAlerte.Resolue, liste[0].Statut);
    }

    [Fact]
    public async Task GetActivesAsync_DevraitRetournerSeulementAlertesActives()
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
            CanalCommunication = CanalCommunication.SignalR
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 1000.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerteActive1 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerteActive2 = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Active, DateCreation = DateTime.UtcNow };
        var alerteAcquittee = new Alerte { Id = Guid.NewGuid(), SondeId = sonde.Id, SeuilAlerteId = seuilAlerte.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Statut = StatutAlerte.Acquittee, DateCreation = DateTime.UtcNow };
        _context.Alertes.AddRange(alerteActive1, alerteActive2, alerteAcquittee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActivesAsync();
        var liste = result.ToList();

        // Assert
        Assert.Equal(2, liste.Count);
        Assert.All(liste, a => Assert.Equal(StatutAlerte.Active, a.Statut));
    }

    [Fact]
    public async Task GetAlerteActiveForSeuilAsync_DevraitRetournerAlerte_SiExiste()
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
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 30.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerteActive = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerteActive);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAlerteActiveForSeuilAsync(sonde.Id, seuilAlerte.Id, TypeAlerte.Alerte);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(alerteActive.Id, result.Id);
    }

    [Fact]
    public async Task GetAlerteActiveForSeuilAsync_DevraitRetournerNull_SiPasActive()
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
            CanalCommunication = CanalCommunication.HttpPull
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 15.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerteAcquittee = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Acquittee,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerteAcquittee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAlerteActiveForSeuilAsync(sonde.Id, seuilAlerte.Id, TypeAlerte.Alerte);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAlerteActiveForSeuilAsync_DevraitRetournerNull_SiTypeAlerteDifferent()
    {
        // Arrange - Test CRITIQUE : vérifier que Avertissement ≠ Alerte
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Entrée" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Entrée",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 18.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        // Créer un Avertissement actif
        var alerteAvertissement = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Avertissement,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerteAvertissement);
        await _context.SaveChangesAsync();

        // Act - Chercher une Alerte (pas un Avertissement)
        var result = await _repository.GetAlerteActiveForSeuilAsync(sonde.Id, seuilAlerte.Id, TypeAlerte.Alerte);

        // Assert - Ne doit PAS trouver l'Avertissement car TypeAlerte différent
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourStatut()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Piscine" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Piscine",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 10.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerte);
        await _context.SaveChangesAsync();

        // Act - Changement Active → Acquittee
        alerte.Statut = StatutAlerte.Acquittee;
        await _repository.UpdateAsync(alerte);

        // Assert
        var updated = await _context.Alertes.FindAsync(alerte.Id);
        Assert.NotNull(updated);
        Assert.Equal(StatutAlerte.Acquittee, updated.Statut);
    }

    [Fact]
    public async Task UpdateAsync_DevraitDefinirDateAcquittement()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Véranda" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Véranda",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.SignalR
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Alerte, Valeur = 35.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerte);
        await _context.SaveChangesAsync();

        // Act
        alerte.Statut = StatutAlerte.Acquittee;
        alerte.DateAcquittement = DateTime.UtcNow;
        await _repository.UpdateAsync(alerte);

        // Assert
        var updated = await _context.Alertes.FindAsync(alerte.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.DateAcquittement);
    }

    [Fact]
    public async Task DeleteAsync_DevraitSupprimerAlerte()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Buanderie" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Pourcentage", Symbole = "%", TypeSonde = TypeSonde.Hydrometrie };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Buanderie",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Hydrometrie,
            EstActif = true,
            CanalCommunication = CanalCommunication.HttpPush
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Maximum, TypeAlerte = TypeAlerte.Avertissement, Valeur = 75.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Maximum,
            TypeAlerte = TypeAlerte.Avertissement,
            Statut = StatutAlerte.Resolue,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerte);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(alerte.Id);

        // Assert
        var deleted = await _context.Alertes.FindAsync(alerte.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandAlerteExiste()
    {
        // Arrange
        var localisation = new Localisation { Id = Guid.NewGuid(), Nom = "Sous-sol" };
        var uniteMesure = new UniteMesure { Id = Guid.NewGuid(), Nom = "Celsius", Symbole = "°C", TypeSonde = TypeSonde.Temperature };
        var sonde = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur Sous-sol",
            LocalisationId = localisation.Id,
            UniteMesureId = uniteMesure.Id,
            TypeSonde = TypeSonde.Temperature,
            EstActif = true,
            CanalCommunication = CanalCommunication.MQTT
        };
        var seuilAlerte = new SeuilAlerte { Id = Guid.NewGuid(), SondeId = sonde.Id, TypeSeuil = TypeSeuil.Minimum, TypeAlerte = TypeAlerte.Alerte, Valeur = 8.0m, EstActif = true, DateCreation = DateTime.UtcNow };
        _context.Localisations.Add(localisation);
        _context.UnitesMesures.Add(uniteMesure);
        _context.Sondes.Add(sonde);
        _context.SeuilsAlerte.Add(seuilAlerte);
        await _context.SaveChangesAsync();

        var alerte = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sonde.Id,
            SeuilAlerteId = seuilAlerte.Id,
            TypeSeuil = TypeSeuil.Minimum,
            TypeAlerte = TypeAlerte.Alerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow
        };
        _context.Alertes.Add(alerte);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(alerte.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerFalse_QuandAlerteNExistePas()
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
