using Application.DTOs.UniteMesure;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

[Mapper]
public partial class UniteMesureMapper
{
    public partial UniteMesureDto ToDto(UniteMesure uniteMesure);

    public partial IEnumerable<UniteMesureDto> ToDtoList(IEnumerable<UniteMesure> unitesMesures);

    public partial UniteMesure ToEntity(CreateUniteMesureDto dto);

    public partial UniteMesure ToEntity(UpdateUniteMesureDto dto);

    [MapperIgnoreTarget(nameof(UniteMesure.Id))]
    public partial void UpdateEntity(UpdateUniteMesureDto dto, UniteMesure existingEntity);
}
