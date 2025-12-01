using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework Core pour l'entité EtatActionneur.
/// Définit la relation 1-to-1 avec Actionneur, l'index unique, et les contraintes.
/// </summary>
public class EtatActionneurConfiguration : IEntityTypeConfiguration<EtatActionneur>
{
    public void Configure(EntityTypeBuilder<EtatActionneur> builder)
    {
        // Configuration de la clé primaire
        builder.HasKey(e => e.Id);

        // Configuration de ActionneurId
        // Clé étrangère vers la table Devices (Actionneur est une entité dérivée)
        builder.Property(e => e.ActionneurId)
            .IsRequired();

        // Configuration de EstActif
        builder.Property(e => e.EstActif)
            .IsRequired();

        // Configuration de Pourcentage
        // Note : La validation 0-100 sera effectuée en Application Layer
        // EF Core ne supporte pas nativement les contraintes CHECK pour les valeurs min/max
        builder.Property(e => e.Pourcentage)
            .IsRequired();

        // Configuration de DerniereModification
        // Requis pour tracer quand l'état a été modifié
        builder.Property(e => e.DerniereModification)
            .IsRequired();

        // Index unique sur ActionneurId pour garantir la relation 1-to-1
        // Un actionneur ne peut avoir qu'un seul état actuel
        builder.HasIndex(e => e.ActionneurId)
            .IsUnique()
            .HasDatabaseName("IX_EtatsActionneur_ActionneurId_Unique");

        // Relation 1-to-1 vers Actionneur
        // HasOne : EtatActionneur a un Actionneur
        // WithOne : Actionneur a un EtatActuel (navigation property dans Actionneur)
        // HasForeignKey<EtatActionneur> : la clé étrangère est dans EtatActionneur
        // OnDelete(Cascade) : si un actionneur est supprimé, son état est également supprimé
        builder.HasOne(e => e.Actionneur)
            .WithOne(a => a.EtatActuel)
            .HasForeignKey<EtatActionneur>(e => e.ActionneurId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
