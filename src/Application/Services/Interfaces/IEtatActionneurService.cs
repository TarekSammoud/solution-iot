using Application.DTOs;
using System;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IEtatActionneurService
    {
        Task<EtatActionneurDto?> GetEtatByActionneurIdAsync(Guid actionneurId);
        Task<EtatActionneurDto?> UpdateEtatAsync(Guid actionneurId, UpdateEtatActionneurDto dto);
    }
}
