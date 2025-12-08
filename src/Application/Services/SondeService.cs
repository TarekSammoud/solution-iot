using Application.DTOs.Sonde;
using Application.Mappers;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SondeService : ISondeService
    {
        private readonly ISondeRepository _repo;
        private readonly IUniteMesureRepository _umRepo;

        public SondeService(ISondeRepository repo, IUniteMesureRepository umRepo)
        {
            _repo = repo;
            _umRepo = umRepo;
        }

        public async Task<SondeDto> CreateAsync(CreateSondeDto dto)
        {
            // Validation
            var unite = await _umRepo.GetByIdAsync(dto.UniteMesureId);
            if (unite == null) throw new Exception("Unité de mesure non trouvée");
            if (unite.TypeSonde != dto.TypeSonde)
                throw new Exception("Cohérence TypeSonde ↔ UniteMesure violée");
            if (dto.ValeurMin.HasValue && dto.ValeurMax.HasValue && dto.ValeurMin >= dto.ValeurMax)
                throw new Exception("ValeurMin doit être < ValeurMax");
            if (dto.CanalCommunication != CanalCommunication.HttpPush && string.IsNullOrWhiteSpace(dto.UrlDevice))
                throw new Exception("UrlDevice obligatoire pour ce canal");

            // Mapping DTO -> Entity using SondeMapper
            var sonde = dto.ToEntity();

            await _repo.AddAsync(sonde);

            // Mapping Entity -> DTO using SondeMapper
            return sonde.ToDto();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sonde = await _repo.GetByIdAsync(id);
            if (sonde == null) return false;
            await _repo.DeleteAsync(sonde);
            return true;
        }

        public async Task<IEnumerable<SondeDto>> GetAllAsync()
        {
            var sondes = await _repo.GetAllWithRelationsAsync();
            return sondes.Select(s => s.ToDto());
        }

        public async Task<SondeDto?> GetByIdAsync(Guid id)
        {
            var sonde = await _repo.GetByIdWithRelationsAsync(id);
            return sonde?.ToDto();
        }

        public async Task<IEnumerable<SondeDto>> GetByLocalisationAsync(Guid localisationId)
        {
            var sondes = await _repo.GetByLocalisationAsync(localisationId);
            return sondes.Select(s => s.ToDto());
        }

        public async Task<IEnumerable<SondeDto>> GetByTypeAsync(TypeSonde typeSonde)
        {
            var sondes = await _repo.GetByTypeAsync(typeSonde);
            return sondes.Select(s => s.ToDto());
        }

        public async Task<SondeDto?> UpdateAsync(UpdateSondeDto dto)
        {
            var sonde = await _repo.GetByIdAsync(dto.Id);
            if (sonde == null) return null;

            // Validation
            var unite = await _umRepo.GetByIdAsync(dto.UniteMesureId);
            if (unite == null) throw new Exception("Unité de mesure non trouvée");
            if (unite.TypeSonde != dto.TypeSonde)
                throw new Exception("Cohérence TypeSonde ↔ UniteMesure violée");
            if (dto.ValeurMin.HasValue && dto.ValeurMax.HasValue && dto.ValeurMin >= dto.ValeurMax)
                throw new Exception("ValeurMin doit être < ValeurMax");
            if (dto.CanalCommunication != CanalCommunication.HttpPush && string.IsNullOrWhiteSpace(dto.UrlDevice))
                throw new Exception("UrlDevice obligatoire pour ce canal");

            // Update entity using SondeMapper
            sonde.UpdateEntity(dto);
            await _repo.UpdateAsync(sonde);

            return sonde.ToDto();
        }
    }
}
