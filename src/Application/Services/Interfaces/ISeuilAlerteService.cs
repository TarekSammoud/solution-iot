using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.SeuilAlerte;

namespace Application.Services.Interfaces
{
    public interface ISeuilAlerteService
    {
        Task<IEnumerable<SeuilAlerteDto>> GetBySondeAsync(Guid sondeId);
        Task<SeuilAlerteDto?> GetByIdAsync(Guid id);
        Task<SeuilAlerteDto> CreateAsync(CreateSeuilAlerteDto dto);
        Task<SeuilAlerteDto> UpdateAsync(Guid id, UpdateSeuilAlerteDto dto);
        Task ToggleAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
