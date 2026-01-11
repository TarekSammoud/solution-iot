using Application.DTOs.Actionneur;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IActionneurService
    {
        Task<IEnumerable<ActionneurDto>> GetAllAsync();
        Task<ActionneurDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ActionneurDto>> GetByLocalisationAsync(Guid localisationId);
        Task<IEnumerable<ActionneurDto>> GetByTypeAsync(TypeActionneur type);
        Task<ActionneurDto> CreateAsync(CreateActionneurDto dto);
        Task<ActionneurDto?> UpdateAsync(Guid id, UpdateActionneurDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
