using Application.DTOs.Sonde;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ISondeService
    {
        Task<IEnumerable<SondeDto>> GetAllAsync();
        Task<SondeDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<SondeDto>> GetByLocalisationAsync(Guid localisationId);
        Task<IEnumerable<SondeDto>> GetByTypeAsync(Domain.Enums.TypeSonde typeSonde);
        Task<SondeDto> CreateAsync(CreateSondeDto dto);
        Task<SondeDto?> UpdateAsync(UpdateSondeDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
