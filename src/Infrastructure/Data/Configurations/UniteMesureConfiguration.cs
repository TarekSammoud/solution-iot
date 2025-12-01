using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité UniteMesure.
/// Définit les contraintes, index et valeurs par défaut.
/// </summary>
public class UniteMesureConfiguration : IEntityTypeConfiguration<UniteMesure>
{
    public void Configure(EntityTypeBuilder<UniteMesure> builder)
    {
        // Clé primaire
        builder.HasKey(u => u.Id);

        // Propriété Nom - Requis, max 100 caractères
        builder.Property(u => u.Nom)
            .IsRequired()
            .HasMaxLength(100);

        // Propriété Symbole - Requis, max 20 caractères (pour °C, °F, %, µg/m³, ppm, etc.)
        builder.Property(u => u.Symbole)
            .IsRequired()
            .HasMaxLength(20);

        // Propriété TypeSonde - Enum stocké comme integer
        builder.Property(u => u.TypeSonde)
            .IsRequired()
            .HasConversion<int>();

        // Propriété DateCreation - Valeur par défaut SQL
        builder.Property(u => u.DateCreation)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        // Index composite sur (TypeSonde, Symbole) pour garantir l'unicité
        // et optimiser les recherches par type de sonde
        builder.HasIndex(u => new { u.TypeSonde, u.Symbole })
            .HasDatabaseName("IX_UnitesMesures_TypeSonde_Symbole")
            .IsUnique();

        // Navigation properties - TODO: Ajouter quand Sonde sera créée
        // builder.HasMany(u => u.Sondes)
        //     .WithOne(s => s.UniteMesure)
        //     .HasForeignKey(s => s.UniteMesureId);
    }
}
