using Application.DTOs.SystemePartenaire;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

/// <summary>
/// Mapper pour les conversions entre l'entité SystemePartenaire et ses DTOs.
/// Utilise Riok.Mapperly pour générer le code de mapping à la compilation.
/// </summary>
[Mapper]
public partial class SystemePartenaireMapper
{
    /// <summary>
    /// Mappe une entité SystemePartenaire vers un SystemePartenaireDto.
    /// </summary>
    public partial SystemePartenaireDto ToDto(SystemePartenaire SystemePartenaire);

    /// <summary>
    /// Mappe une collection d'entités vers une collection de DTOs.
    /// </summary>
    public partial IEnumerable<SystemePartenaireDto> ToDtoList(IEnumerable<SystemePartenaire> SystemePartenaires);

    /// <summary>
    /// Mappe un CreateSystemePartenaireDto vers une entité SystemePartenaire.
    /// Id et DateCreation ne sont pas mappés (générés par le repository).
    /// </summary>
    public partial SystemePartenaire ToEntity(CreateSystemePartenaireDto dto);

    /// <summary>
    /// Mappe un UpdateSystemePartenaireDto vers une entité SystemePartenaire.
    /// DateCreation ne sera pas mappé car absent du DTO (propriété immutable).
    /// </summary>
    public partial SystemePartenaire ToEntity(UpdateSystemePartenaireDto dto);

    /// <summary>
    /// Met à jour une entité existante avec les données d'un UpdateSystemePartenaireDto.
    /// Préserve Id et DateCreation de l'entité existante.
    /// </summary>
    [MapperIgnoreTarget(nameof(SystemePartenaire.Id))]
    public partial void UpdateEntity(UpdateSystemePartenaireDto dto, SystemePartenaire existingEntity);
}
