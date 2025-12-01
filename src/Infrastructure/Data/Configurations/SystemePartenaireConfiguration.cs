using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité SystemePartenaire.
/// Définit les contraintes, index et valeurs par défaut pour la table SystemesPartenaires.
/// </summary>
/// <remarks>
/// IMPORTANT - Différence chiffrement vs hash :
/// - PasswordChiffre : Stocke un mot de passe CHIFFRÉ (réversible). Nécessite plus d'espace (max 1000 caractères).
/// - PasswordHashAcces : Stocke un mot de passe HACHÉ (irréversible). Taille fixe selon l'algorithme (max 500 caractères).
/// </remarks>
public class SystemePartenaireConfiguration : IEntityTypeConfiguration<SystemePartenaire>
{
    public void Configure(EntityTypeBuilder<SystemePartenaire> builder)
    {
        // Configuration de la clé primaire
        builder.HasKey(s => s.Id);

        // Configuration de Nom
        // Requis, longueur maximale de 200 caractères
        builder.Property(s => s.Nom)
            .IsRequired()
            .HasMaxLength(200);

        // Configuration de UrlBase
        // Requis, longueur maximale de 500 caractères pour les URLs longues
        builder.Property(s => s.UrlBase)
            .IsRequired()
            .HasMaxLength(500);

        // ===== CREDENTIALS POUR APPELER LE PARTENAIRE (nous → lui) =====

        // Configuration de UsernameAppel
        // Optionnel, longueur maximale de 100 caractères
        builder.Property(s => s.UsernameAppel)
            .HasMaxLength(100);

        // Configuration de PasswordChiffre
        // Optionnel, longueur maximale de 1000 caractères
        // IMPORTANT : Le chiffrement prend plus de place que le texte original
        builder.Property(s => s.PasswordChiffre)
            .HasMaxLength(1000);

        // ===== CREDENTIALS POUR ÊTRE APPELÉ PAR LE PARTENAIRE (lui → nous) =====

        // Configuration de UsernameAcces
        // Optionnel, longueur maximale de 100 caractères
        // Index pour recherche rapide lors de l'authentification entrante
        builder.Property(s => s.UsernameAcces)
            .HasMaxLength(100);

        builder.HasIndex(s => s.UsernameAcces)
            .HasDatabaseName("IX_SystemesPartenaires_UsernameAcces");

        // Configuration de PasswordHashAcces
        // Optionnel, longueur maximale de 500 caractères
        // IMPORTANT : Le hash a une taille fixe selon l'algorithme (ex: SHA256 = 64 caractères)
        builder.Property(s => s.PasswordHashAcces)
            .HasMaxLength(500);

        // ===== FLAGS =====

        // Configuration de EstAppelant
        // Indique si le système peut nous appeler
        builder.Property(s => s.EstAppelant)
            .IsRequired();

        // Configuration de EstAppele
        // Indique si on peut appeler le système
        builder.Property(s => s.EstAppele)
            .IsRequired();

        // Configuration de EstActif
        // Indique si le système est actif
        builder.Property(s => s.EstActif)
            .IsRequired();

        // Configuration de DateCreation
        // Valeur par défaut SQL pour définir automatiquement la date de création
        builder.Property(s => s.DateCreation)
            .HasDefaultValueSql("datetime('now')");
    }
}
