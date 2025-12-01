using Application.DTOs;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

/// <summary>
/// Mapper pour les conversions entre l'entité Localisation et ses DTOs.
/// Utilise Riok.Mapperly pour générer le code de mapping à la compilation.
/// </summary>
[Mapper]
public partial class LocalisationMapper
{
    /// <summary>
    /// Mappe une entité Localisation vers un LocalisationDto.
    /// </summary>
    public partial LocalisationDto ToDto(Localisation localisation);

    /// <summary>
    /// Mappe une collection d'entités vers une collection de DTOs.
    /// </summary>
    public partial IEnumerable<LocalisationDto> ToDtoList(IEnumerable<Localisation> localisations);

    /// <summary>
    /// Mappe un CreateLocalisationDto vers une entité Localisation.
    /// Id et DateCreation ne sont pas mappés (générés par le repository).
    /// </summary>
    public partial Localisation ToEntity(CreateLocalisationDto dto);

    /// <summary>
    /// Mappe un UpdateLocalisationDto vers une entité Localisation.
    /// DateCreation ne sera pas mappé car absent du DTO (propriété immutable).
    /// </summary>
    public partial Localisation ToEntity(UpdateLocalisationDto dto);

    /// <summary>
    /// Met à jour une entité existante avec les données d'un UpdateLocalisationDto.
    /// Préserve Id et DateCreation de l'entité existante.
    /// </summary>
    [MapperIgnoreTarget(nameof(Localisation.Id))]
    public partial void UpdateEntity(UpdateLocalisationDto dto, Localisation existingEntity);
}
