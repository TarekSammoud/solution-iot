using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Alerte.
/// Définit les contraintes, index et relations pour la table Alertes.
/// </summary>
public class AlerteConfiguration : IEntityTypeConfiguration<Alerte>
{
    public void Configure(EntityTypeBuilder<Alerte> builder)
    {
        // Configuration de la clé primaire
        builder.HasKey(a => a.Id);

        // Configuration de SondeId
        // Clé étrangère vers la table Sondes
        builder.Property(a => a.SondeId)
            .IsRequired();

        // Configuration de SeuilAlerteId
        // Clé étrangère vers la table SeuilsAlerte
        builder.Property(a => a.SeuilAlerteId)
            .IsRequired();

        // Configuration de TypeSeuil
        // Enum stocké comme entier en base de données
        builder.Property(a => a.TypeSeuil)
            .IsRequired();

        // Configuration de TypeAlerte
        // Enum stocké comme entier en base de données
        builder.Property(a => a.TypeAlerte)
            .IsRequired();

        // Configuration de Statut
        // Enum stocké comme entier en base de données
        builder.Property(a => a.Statut)
            .IsRequired();

        // Configuration de DateCreation
        // Valeur par défaut SQL pour définir automatiquement la date de création
        builder.Property(a => a.DateCreation)
            .HasDefaultValueSql("datetime('now')");

        // Configuration de DateAcquittement
        // Nullable, définie lorsque l'alerte est acquittée
        builder.Property(a => a.DateAcquittement)
            .IsRequired(false);

        // Configuration de DateResolution
        // Nullable, définie lorsque l'alerte est résolue
        builder.Property(a => a.DateResolution)
            .IsRequired(false);

        // Configuration de Message
        // Optionnel, longueur maximale de 1000 caractères
        builder.Property(a => a.Message)
            .HasMaxLength(1000)
            .IsRequired(false);

        // Index composé sur (SondeId, Statut) pour optimiser les recherches d'alertes actives par sonde
        // Utilisé par GetBySondeAsync et GetActivesAsync filtrées par sonde
        builder.HasIndex(a => new { a.SondeId, a.Statut })
            .HasDatabaseName("IX_Alertes_SondeId_Statut");

        // Index sur SeuilAlerteId pour optimiser les recherches par seuil
        // Utilisé par GetAlerteActiveForSeuilAsync
        builder.HasIndex(a => a.SeuilAlerteId)
            .HasDatabaseName("IX_Alertes_SeuilAlerteId");

        // Relation avec Sonde
        // Cascade delete: si une sonde est supprimée, toutes ses alertes sont supprimées
        // Logique : les alertes n'ont plus de sens si la sonde n'existe plus
        builder.HasOne(a => a.Sonde)
            .WithMany()
            .HasForeignKey(a => a.SondeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation avec SeuilAlerte
        // Restrict delete: impossible de supprimer un seuil si des alertes y font référence
        // Logique : protéger l'historique des alertes (on doit garder la trace du seuil qui a déclenché l'alerte)
        builder.HasOne(a => a.SeuilAlerte)
            .WithMany()
            .HasForeignKey(a => a.SeuilAlerteId)
            .OnDelete(DeleteBehavior.Restrict);

        // NOTE IMPORTANTE : PAS de relation vers Releve
        // Un relevé peut DÉCLENCHER une alerte via la logique métier,
        // mais il n'y a pas de relation physique (FK) entre Alerte et Releve.
    }
}
