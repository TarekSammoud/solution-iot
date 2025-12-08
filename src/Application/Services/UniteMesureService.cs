using Application.DTOs.UniteMesure;
using Application.Mappers;
using Application.Services.Interfaces;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class UniteMesureService : IUniteMesureService
{
    private readonly IUniteMesureRepository _repository;
    private readonly UniteMesureMapper _mapper;

    public UniteMesureService(IUniteMesureRepository repository, UniteMesureMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UniteMesureDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.ToDto(entity);
    }

    public async Task<IEnumerable<UniteMesureDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.ToDtoList(entities);
    }

    public async Task<IEnumerable<UniteMesureDto>> GetByTypeSondeAsync(TypeSonde typeSonde)
    {
        var entities = await _repository.GetByTypeSondeAsync(typeSonde);
        return _mapper.ToDtoList(entities);
    }

    public async Task<UniteMesureDto> CreateAsync(CreateUniteMesureDto dto)
    {
        var existing = await _repository.GetByTypeSondeAsync(dto.TypeSonde);
        var duplicate = existing.Any(u => string.Equals(u.Symbole.Trim(), dto.Symbole.Trim(), StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            throw new InvalidOperationException("Une unité avec ce symbole et type existe déjà.");
        }

        var entity = _mapper.ToEntity(dto);
        var created = await _repository.AddAsync(entity);
        return _mapper.ToDto(created);
    }

    public async Task<UniteMesureDto?> UpdateAsync(UpdateUniteMesureDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity is null)
        {
            return null;
        }

        var existing = await _repository.GetByTypeSondeAsync(dto.TypeSonde);
        var duplicate = existing.Any(u => u.Id != dto.Id && string.Equals(u.Symbole.Trim(), dto.Symbole.Trim(), StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            throw new InvalidOperationException("Une unité avec ce symbole et type existe déjà.");
        }

        _mapper.UpdateEntity(dto, entity);
        await _repository.UpdateAsync(entity);
        return _mapper.ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        return true;
    }

    public Task<bool> ExistsAsync(Guid id) => _repository.ExistsAsync(id);
}
