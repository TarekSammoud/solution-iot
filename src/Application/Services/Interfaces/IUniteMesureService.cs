using Application.DTOs;
using Domain.Enums;

namespace Application.Services.Interfaces;

public interface IUniteMesureService
{
    Task<UniteMesureDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UniteMesureDto>> GetAllAsync();
    Task<IEnumerable<UniteMesureDto>> GetByTypeSondeAsync(TypeSonde typeSonde);
    Task<UniteMesureDto> CreateAsync(CreateUniteMesureDto dto);
    Task<UniteMesureDto?> UpdateAsync(UpdateUniteMesureDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
