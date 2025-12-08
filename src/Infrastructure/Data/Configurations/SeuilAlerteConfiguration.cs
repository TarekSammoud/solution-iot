using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité SeuilAlerte.
/// Définit les contraintes, index, précision décimale et relations pour la table SeuilsAlerte.
/// </summary>
public class SeuilAlerteConfiguration : IEntityTypeConfiguration<SeuilAlerte>
{
    public void Configure(EntityTypeBuilder<SeuilAlerte> builder)
    {
        // Configuration de la clé primaire
        builder.HasKey(s => s.Id);

        // Configuration de SondeId
        // Clé étrangère vers la table Sondes
        builder.Property(s => s.SondeId)
            .IsRequired();

        // Configuration de TypeSeuil
        // Enum stocké comme entier en base de données
        builder.Property(s => s.TypeSeuil)
            .IsRequired();

        // Configuration de TypeAlerte
        // Enum stocké comme entier en base de données
        builder.Property(s => s.TypeAlerte)
            .IsRequired();

        // Configuration de Valeur
        // Précision de 10 chiffres dont 2 décimales (ex: 12345678.90)
        builder.Property(s => s.Valeur)
            .IsRequired()
            .HasPrecision(10, 2);

        // Configuration de EstActif
        builder.Property(s => s.EstActif)
            .IsRequired();

        // Configuration de DateCreation
        // Valeur par défaut SQL pour définir automatiquement la date de création
        builder.Property(s => s.DateCreation)
            .HasDefaultValueSql("datetime('now')");

        // Index sur SondeId pour optimiser les requêtes par sonde
        builder.HasIndex(s => s.SondeId)
            .HasDatabaseName("IX_SeuilsAlerte_SondeId");

        // Relation avec Sonde
        // Cascade delete: si une sonde est supprimée, tous ses seuils d'alerte sont supprimés
        builder.HasOne(s => s.Sonde)
            .WithMany(sonde => sonde.SeuilsAlerte) // <-- préciser la navigation inverse
            .HasForeignKey(s => s.SondeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation vers Alertes - One-to-Many
        // Pas de navigation property dans SeuilAlerte (utilisation de eager loading)
        builder.HasMany<Alerte>()
            .WithOne(a => a.SeuilAlerte)
            .HasForeignKey(a => a.SeuilAlerteId)
            .OnDelete(DeleteBehavior.Restrict); // Ne pas supprimer le seuil si des alertes existent
    }
}
