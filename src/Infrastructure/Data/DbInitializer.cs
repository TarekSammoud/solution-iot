using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Appliquer les migrations en attente
        await context.Database.MigrateAsync();

        // Vérifier si la base contient déjà des données
        if (await context.Localisations.AnyAsync())
        {
            return; // La base est déjà initialisée
        }

        // Seed des Localisations
        var locSalleServeur = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Batiment A - Salle serveur",
            Description = "Salle des serveurs informatiques - niveau sous-sol",
            DateCreation = DateTime.UtcNow
        };
        var locBureau = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Batiment A - Bureau étage 1",
            Description = "Open space principal - premier étage",
            DateCreation = DateTime.UtcNow
        };
        var locEntrepot = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Batiment B - Entrepôt",
            Description = "Zone de stockage principal",
            DateCreation = DateTime.UtcNow
        };
        var locLabo = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Batiment C - Laboratoire",
            Description = "Laboratoire de recherche et développement",
            DateCreation = DateTime.UtcNow
        };

        var localisations = new[] { locSalleServeur, locBureau, locEntrepot, locLabo };
        await context.Localisations.AddRangeAsync(localisations);

        // Seed des Unités de Mesure
        var uniteCelsius = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Celsius",
            Symbole = "°C",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        var uniteFahrenheit = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Fahrenheit",
            Symbole = "°F",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        var uniteKelvin = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Kelvin",
            Symbole = "K",
            TypeSonde = TypeSonde.Temperature,
            DateCreation = DateTime.UtcNow
        };
        var unitePourcent = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Pourcentage",
            Symbole = "%",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };
        var uniteHumidite = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Humidité Relative",
            Symbole = "% HR",
            TypeSonde = TypeSonde.Hydrometrie,
            DateCreation = DateTime.UtcNow
        };
        var uniteMicroGrammes = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Microgrammes par mètre cube",
            Symbole = "µg/m³",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow
        };
        var unitePpm = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Parties par million",
            Symbole = "ppm",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow
        };
        var uniteIQA = new UniteMesure
        {
            Id = Guid.NewGuid(),
            Nom = "Indice de qualité de l'air",
            Symbole = "IQA",
            TypeSonde = TypeSonde.QualiteAir,
            DateCreation = DateTime.UtcNow
        };

        var unitesMesures = new[] { uniteCelsius, uniteFahrenheit, uniteKelvin, unitePourcent, uniteHumidite, uniteMicroGrammes, unitePpm, uniteIQA };
        await context.UnitesMesures.AddRangeAsync(unitesMesures);

        // Seed des Utilisateurs
        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@iot.local",
                // Password: "Admin123!" - Hash BCrypt
                PasswordHash = "$2a$11$LQKfJwzkXVvvPWvDvVqN3eZGJvXhKb8QSKG5GIkX7PzN3c7/V6JQy",
                Role = RoleUtilisateur.Administrateur,
                EstActif = true,
                DateCreation = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "user",
                Email = "user@iot.local",
                // Password: "User123!" - Hash BCrypt
                PasswordHash = "$2a$11$n8kGxJBvPcKz5Zn8qYW8OuQvZzIvJGQqHXfG5xZ5YrKxTdXzP6XYm",
                Role = RoleUtilisateur.Utilisateur,
                EstActif = true,
                DateCreation = DateTime.UtcNow
            }
        };
        await context.Users.AddRangeAsync(users);

        // Seed des Sondes
        var dateInstallation = DateTime.UtcNow.AddMonths(-3);

        var sondeTempServeur = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur température - Salle serveur",
            LocalisationId = locSalleServeur.Id,
            EstActif = true,
            DateInstallation = dateInstallation,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPush,
            TypeSonde = TypeSonde.Temperature,
            UniteMesureId = uniteCelsius.Id
        };

        var sondeHumiditeServeur = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur humidité - Salle serveur",
            LocalisationId = locSalleServeur.Id,
            EstActif = true,
            DateInstallation = dateInstallation,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.HttpPush,
            TypeSonde = TypeSonde.Hydrometrie,
            UniteMesureId = unitePourcent.Id
        };

        var sondeTempBureau = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur température - Bureau",
            LocalisationId = locBureau.Id,
            EstActif = true,
            DateInstallation = dateInstallation,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.MQTT,
            TypeSonde = TypeSonde.Temperature,
            UniteMesureId = uniteCelsius.Id
        };

        var sondeQualiteAirBureau = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur qualité air - Bureau",
            LocalisationId = locBureau.Id,
            EstActif = true,
            DateInstallation = dateInstallation,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.MQTT,
            TypeSonde = TypeSonde.QualiteAir,
            UniteMesureId = unitePpm.Id
        };

        var sondeTempLabo = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur température - Laboratoire",
            LocalisationId = locLabo.Id,
            EstActif = true,
            DateInstallation = dateInstallation,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.SignalR,
            TypeSonde = TypeSonde.Temperature,
            UniteMesureId = uniteCelsius.Id
        };

        var sondeHumiditeLabo = new Sonde
        {
            Id = Guid.NewGuid(),
            Nom = "Capteur humidité - Laboratoire",
            LocalisationId = locLabo.Id,
            EstActif = true,
            DateInstallation = dateInstallation,
            DateCreation = DateTime.UtcNow,
            CanalCommunication = CanalCommunication.SignalR,
            TypeSonde = TypeSonde.Hydrometrie,
            UniteMesureId = uniteHumidite.Id
        };

        var sondes = new[] { sondeTempServeur, sondeHumiditeServeur, sondeTempBureau, sondeQualiteAirBureau, sondeTempLabo, sondeHumiditeLabo };
        await context.Devices.AddRangeAsync(sondes);

        // Seed des Relevés (données des 7 derniers jours)
        var releves = new List<Releve>();
        var random = new Random(42); // Seed fixe pour reproductibilité

        // Générer des relevés pour chaque sonde sur les 7 derniers jours (toutes les 2 heures)
        for (int jour = 7; jour >= 0; jour--)
        {
            for (int heure = 0; heure < 24; heure += 2)
            {
                var dateReleve = DateTime.UtcNow.AddDays(-jour).AddHours(heure);

                // Sonde température salle serveur (20-22°C - stable)
                releves.Add(new Releve
                {
                    Id = Guid.NewGuid(),
                    SondeId = sondeTempServeur.Id,
                    Valeur = 20.5m + (decimal)(random.NextDouble() * 1.5),
                    DateHeure = dateReleve,
                    TypeReleve = TypeReleve.Automatique
                });

                // Sonde humidité salle serveur (40-50%)
                releves.Add(new Releve
                {
                    Id = Guid.NewGuid(),
                    SondeId = sondeHumiditeServeur.Id,
                    Valeur = 42m + (decimal)(random.NextDouble() * 8),
                    DateHeure = dateReleve,
                    TypeReleve = TypeReleve.Automatique
                });

                // Sonde température bureau (18-24°C - varie plus)
                releves.Add(new Releve
                {
                    Id = Guid.NewGuid(),
                    SondeId = sondeTempBureau.Id,
                    Valeur = 19m + (decimal)(random.NextDouble() * 5),
                    DateHeure = dateReleve,
                    TypeReleve = TypeReleve.Automatique
                });

                // Sonde qualité air bureau (400-800 ppm)
                releves.Add(new Releve
                {
                    Id = Guid.NewGuid(),
                    SondeId = sondeQualiteAirBureau.Id,
                    Valeur = 400m + (decimal)(random.NextDouble() * 400),
                    DateHeure = dateReleve,
                    TypeReleve = TypeReleve.Automatique
                });

                // Sonde température laboratoire (21-23°C - très stable)
                releves.Add(new Releve
                {
                    Id = Guid.NewGuid(),
                    SondeId = sondeTempLabo.Id,
                    Valeur = 21.5m + (decimal)(random.NextDouble() * 1.5),
                    DateHeure = dateReleve,
                    TypeReleve = TypeReleve.Automatique
                });

                // Sonde humidité laboratoire (45-55% HR)
                releves.Add(new Releve
                {
                    Id = Guid.NewGuid(),
                    SondeId = sondeHumiditeLabo.Id,
                    Valeur = 48m + (decimal)(random.NextDouble() * 7),
                    DateHeure = dateReleve,
                    TypeReleve = TypeReleve.Automatique
                });
            }
        }

        // Ajouter quelques relevés manuels récents
        releves.Add(new Releve
        {
            Id = Guid.NewGuid(),
            SondeId = sondeTempServeur.Id,
            Valeur = 21.2m,
            DateHeure = DateTime.UtcNow.AddHours(-1),
            TypeReleve = TypeReleve.Manuel
        });

        await context.Releves.AddRangeAsync(releves);

        // Sauvegarder les changements
        await context.SaveChangesAsync();
    }
}
