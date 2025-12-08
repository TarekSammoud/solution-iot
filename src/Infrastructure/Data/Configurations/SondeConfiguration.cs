using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Sonde.
/// Configure les propriétés spécifiques aux sondes et leurs relations.
/// </summary>
public class SondeConfiguration : IEntityTypeConfiguration<Sonde>
{
    public void Configure(EntityTypeBuilder<Sonde> builder)
    {
        // Propriété TypeSonde - Enum stocké comme integer
        builder.Property(s => s.TypeSonde)
            .IsRequired()
            .HasConversion<int>();

        // Relation vers UniteMesure - Many-to-One
        // DeleteBehavior.Restrict : impossible de supprimer une UniteMesure si elle est utilisée par des Sondes
        builder.HasOne(s => s.UniteMesure)
            .WithMany(u => u.Sondes) // ← lier à la collection existante
            .HasForeignKey(s => s.UniteMesureId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation vers Releves - One-to-Many
        // Pas de navigation property dans Sonde (utilisation de eager loading)
        builder.HasMany<Releve>()
            .WithOne(r => r.Sonde)
            .HasForeignKey(r => r.SondeId)
            .OnDelete(DeleteBehavior.Cascade); // Supprimer les relevés si la sonde est supprimée

        // Relation vers SeuilsAlerte - One-to-Many
        // Pas de navigation property dans Sonde (utilisation de eager loading)
        builder.HasMany<SeuilAlerte>()
            .WithOne(sa => sa.Sonde)
            .HasForeignKey(sa => sa.SondeId)
            .OnDelete(DeleteBehavior.Cascade); // Supprimer les seuils si la sonde est supprimée

        // Relation vers Alertes - One-to-Many
        // Pas de navigation property dans Sonde (utilisation de eager loading)
        builder.HasMany<Alerte>()
            .WithOne(a => a.Sonde)
            .HasForeignKey(a => a.SondeId)
            .OnDelete(DeleteBehavior.Cascade); // Supprimer les alertes si la sonde est supprimée
    }
}
