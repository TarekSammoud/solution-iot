using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Device (classe abstraite).
/// Utilise Table Per Hierarchy (TPH) pour stocker Sonde et Actionneur dans la même table Devices.
/// Un discriminator "DeviceType" permet de différencier les types de devices.
/// </summary>
public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        // Clé primaire
        builder.HasKey(d => d.Id);

        // Propriété Nom - Requis, max 200 caractères
        builder.Property(d => d.Nom)
            .IsRequired()
            .HasMaxLength(200);

        // Propriété UrlDevice - Optionnel, max 500 caractères
        builder.Property(d => d.UrlDevice)
            .HasMaxLength(500);

        // Propriété CredentialsDevice - Optionnel, max 1000 caractères (chiffrement prend de la place)
        builder.Property(d => d.CredentialsDevice)
            .HasMaxLength(1000);

        // Propriété DateCreation - Valeur par défaut SQL
        builder.Property(d => d.DateCreation)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        // Propriété DateInstallation - Requis
        builder.Property(d => d.DateInstallation)
            .IsRequired();

        // Propriété CanalCommunication - Enum stocké comme integer
        builder.Property(d => d.CanalCommunication)
            .IsRequired()
            .HasConversion<int>();

        // Propriété EstActif - Requis
        builder.Property(d => d.EstActif)
            .IsRequired();

        // TABLE PER HIERARCHY (TPH) - Configuration du discriminator
        // Un seul table "Devices" contenant Sonde et Actionneur
        // Colonne "DeviceType" (string) pour différencier les types :
        //   - "Sonde" pour les instances de Sonde
        //   - "Actionneur" pour les instances de Actionneur
        builder.HasDiscriminator<string>("DeviceType")
            .HasValue<Sonde>("Sonde")
            .HasValue<Actionneur>("Actionneur");

        // Relation vers Localisation - Many-to-One
        // DeleteBehavior.Restrict : impossible de supprimer une Localisation si elle contient des Devices
        builder.HasOne(d => d.Localisation)
            .WithMany() // Pas de collection Devices dans Localisation (utilisation de eager loading)
            .HasForeignKey(d => d.LocalisationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
