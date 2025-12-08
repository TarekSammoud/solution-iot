using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISondeRepository
    {
        Task<Sonde?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get a Sonde including relations (Localisation + UniteMesure)
        /// </summary>
        Task<Sonde?> GetByIdWithRelationsAsync(Guid id);

        Task<IEnumerable<Sonde>> GetAllAsync();

        /// <summary>
        /// Get all sondes including relations
        /// </summary>
        Task<IEnumerable<Sonde>> GetAllWithRelationsAsync();

        Task<IEnumerable<Sonde>> GetByLocalisationAsync(Guid localisationId);

        /// <summary>
        /// Get all sondes of a localisation including relations
        /// </summary>
        Task<IEnumerable<Sonde>> GetByLocalisationWithRelationsAsync(Guid localisationId);

        Task<IEnumerable<Sonde>> GetByTypeAsync(TypeSonde type);

        /// <summary>
        /// Get all sondes of a type including relations
        /// </summary>
        Task<IEnumerable<Sonde>> GetByTypeWithRelationsAsync(TypeSonde type);

        Task<IEnumerable<Sonde>> GetActivesAsync();

        Task<Sonde> AddAsync(Sonde sonde);

        Task UpdateAsync(Sonde sonde);

        Task DeleteAsync(Sonde sonde);

        Task DeleteAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
    }
}
