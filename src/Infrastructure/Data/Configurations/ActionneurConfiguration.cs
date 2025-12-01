using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Actionneur.
/// Configure les propriétés spécifiques aux actionneurs et leurs relations.
/// </summary>
public class ActionneurConfiguration : IEntityTypeConfiguration<Actionneur>
{
    public void Configure(EntityTypeBuilder<Actionneur> builder)
    {
        // Propriété TypeActionneur - Enum stocké comme integer
        builder.Property(a => a.TypeActionneur)
            .IsRequired()
            .HasConversion<int>();

        // Note : La relation 1-to-1 vers EtatActuel est configurée dans EtatActionneurConfiguration
        // pour éviter les conflits de configuration. Voir EtatActionneurConfiguration.Configure().
    }
}
