using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité Releve.
/// Définit les contraintes, index, précision décimale et relations pour la table Releves.
/// </summary>
public class ReleveConfiguration : IEntityTypeConfiguration<Releve>
{
    public void Configure(EntityTypeBuilder<Releve> builder)
    {
        // Configuration de la clé primaire
        builder.HasKey(r => r.Id);

        // Configuration de SondeId
        // Clé étrangère vers la table Sondes
        builder.Property(r => r.SondeId)
            .IsRequired();

        // Configuration de Valeur
        // Précision de 10 chiffres dont 2 décimales (ex: 12345678.90)
        builder.Property(r => r.Valeur)
            .IsRequired()
            .HasPrecision(10, 2);

        // Configuration de DateHeure
        // Requis pour le timestamp du relevé
        builder.Property(r => r.DateHeure)
            .IsRequired();

        // Configuration de TypeReleve
        // Enum stocké comme entier en base de données
        builder.Property(r => r.TypeReleve)
            .IsRequired();

        // Index composite sur (SondeId, DateHeure) pour optimiser les requêtes par sonde et plage de dates
        builder.HasIndex(r => new { r.SondeId, r.DateHeure })
            .HasDatabaseName("IX_Releves_SondeId_DateHeure");

        // Relation avec Sonde
        // Cascade delete: si une sonde est supprimée, tous ses relevés sont supprimés
        builder.HasOne(r => r.Sonde)
            .WithMany()
            .HasForeignKey(r => r.SondeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
