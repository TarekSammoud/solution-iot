using Application.DTOs.Releve;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

/// <summary>
/// Mapper pour les conversions entre l'entité Releve et ses DTOs.
/// Utilise Riok.Mapperly pour générer le code de mapping à la compilation.
/// </summary>
[Mapper]
public partial class ReleveMapper
{
    /// <summary>
    /// Mappe une entité Releve vers un ReleveDto.
    /// </summary>
    public partial ReleveDto ToDto(Releve Releve);

    /// <summary>
    /// Mappe une collection d'entités vers une collection de DTOs.
    /// </summary>
    public partial IEnumerable<ReleveDto> ToDtoList(IEnumerable<Releve> Releves);

    /// <summary>
    /// Mappe un CreateReleveDto vers une entité Releve.
    /// Id et DateCreation ne sont pas mappés (générés par le repository).
    /// </summary>
    public partial Releve ToEntity(CreateReleveDto dto);

    /// <summary>
    /// Mappe un UpdateReleveDto vers une entité Releve.
    /// DateCreation ne sera pas mappé car absent du DTO (propriété immutable).
    /// </summary>
    public partial Releve ToEntity(UpdateReleveDto dto);

    /// <summary>
    /// Met à jour une entité existante avec les données d'un UpdateReleveDto.
    /// Préserve Id et DateCreation de l'entité existante.
    /// </summary>
    [MapperIgnoreTarget(nameof(Releve.Id))]
    public partial void UpdateEntity(UpdateReleveDto dto, Releve existingEntity);
}
