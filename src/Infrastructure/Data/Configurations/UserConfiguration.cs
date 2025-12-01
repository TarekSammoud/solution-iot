using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité User.
/// Définit les contraintes, index et valeurs par défaut pour la table Users.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Configuration de la clé primaire
        builder.HasKey(u => u.Id);

        // Configuration de Username
        // Requis, longueur maximale de 100 caractères, avec index unique pour garantir l'unicité
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        // Configuration de Email
        // Requis, longueur maximale de 255 caractères, avec index unique pour garantir l'unicité
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // Configuration de PasswordHash
        // Requis, longueur maximale de 500 caractères pour stocker le hash du mot de passe
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        // Configuration de DateCreation
        // Valeur par défaut SQL pour définir automatiquement la date de création à la date/heure actuelle
        builder.Property(u => u.DateCreation)
            .HasDefaultValueSql("datetime('now')");

        // Configuration de Role
        // Stocké en tant qu'entier dans la base de données (enum)
        builder.Property(u => u.Role)
            .IsRequired();

        // Configuration de EstActif
        // Indique si le compte est actif
        builder.Property(u => u.EstActif)
            .IsRequired();
    }
}
