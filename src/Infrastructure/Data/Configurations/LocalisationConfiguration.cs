using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Localisation.
/// Définit les contraintes, index et valeurs par défaut pour la table Localisations.
/// </summary>
public class LocalisationConfiguration : IEntityTypeConfiguration<Localisation>
{
    public void Configure(EntityTypeBuilder<Localisation> builder)
    {
        // Configuration de la clé primaire
        builder.HasKey(l => l.Id);

        // Configuration de Nom
        // Requis, longueur maximale de 200 caractères
        builder.Property(l => l.Nom)
            .IsRequired()
            .HasMaxLength(200);

        // Configuration de Description
        // Optionnel, longueur maximale de 500 caractères
        builder.Property(l => l.Description)
            .HasMaxLength(500);

        // Configuration de DateCreation
        // Valeur par défaut SQL pour définir automatiquement la date de création
        builder.Property(l => l.DateCreation)
            .HasDefaultValueSql("datetime('now')");

        // Configuration des relations avec Sondes et Actionneurs
        // Utilise HasMany<T>() sans navigation property dans Localisation (eager loading utilisé à la place)

        // Relation HasMany avec Sondes
        builder.HasMany<Sonde>()
            .WithOne(s => s.Localisation)
            .HasForeignKey(s => s.LocalisationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation HasMany avec Actionneurs
        builder.HasMany<Actionneur>()
            .WithOne(a => a.Localisation)
            .HasForeignKey(a => a.LocalisationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
