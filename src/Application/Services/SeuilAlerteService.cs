using Application.DTOs.SeuilAlerte;
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
    public class SeuilAlerteService : ISeuilAlerteService
    {
        private readonly ISeuilAlerteRepository _seuilRepo;
        private readonly ISondeRepository _sondeRepo;
        private readonly SeuilAlerteMapper _mapper;

        public SeuilAlerteService(
            ISeuilAlerteRepository seuilRepo,
            ISondeRepository sondeRepo,
            IAlerteRepository alerteRepo,
            SeuilAlerteMapper mapper)
        {
            _seuilRepo = seuilRepo;
            _sondeRepo = sondeRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SeuilAlerteDto>> GetBySondeAsync(Guid sondeId)
        {
            var sonde = await _sondeRepo.GetByIdAsync(sondeId);
            if (sonde == null) throw new KeyNotFoundException("Sonde introuvable.");

            var seuils = await _seuilRepo.GetBySondeIdAsync(sondeId);
            return seuils.Select(s =>
            {
                var dto = _mapper.Map(s);
                dto.SondeId = sonde.Id;
                return dto;
            });
        }

        public async Task<SeuilAlerteDto?> GetByIdAsync(Guid id)
        {
            var s = await _seuilRepo.GetByIdAsync(id);
            if (s == null) return null;

            var dto = _mapper.Map(s);

            var sonde = await _sondeRepo.GetByIdAsync(s.SondeId);
            if (sonde != null) dto.SondeId = sonde.Id;

            return dto;
        }

        public async Task<SeuilAlerteDto> CreateAsync(CreateSeuilAlerteDto dto)
        {
            // Vérifier sonde existante
            var sonde = await _sondeRepo.GetByIdAsync(dto.SondeId)
                        ?? throw new ArgumentException("Sonde introuvable.");

            // Si activation -> désactiver l'ancien actif du même type
            if (dto.EstActif)
            {
                var ancienActif = await _seuilRepo.FindActiveBySondeAndTypeAsync(dto.SondeId, dto.TypeSeuil);
                if (ancienActif != null)
                {
                    ancienActif.EstActif = false;
                    await _seuilRepo.UpdateAsync(ancienActif);
                }
            }

            var autreType = dto.TypeSeuil == TypeSeuil.Minimum ? TypeSeuil.Maximum : TypeSeuil.Minimum;
            var seuilAutreActif = await _seuilRepo.FindActiveBySondeAndTypeAsync(dto.SondeId, autreType);

            if (seuilAutreActif != null && dto.EstActif)
            {
                if (dto.TypeSeuil == TypeSeuil.Minimum && !((decimal) dto.Valeur < seuilAutreActif.Valeur))
                    throw new ArgumentException(
                        $"Cohérence requise : SeuilMin ({dto.Valeur}) < SeuilMax ({seuilAutreActif.Valeur})."
                    );

                if (dto.TypeSeuil == TypeSeuil.Maximum && !(seuilAutreActif.Valeur < (decimal)dto.Valeur))
                    throw new ArgumentException(
                        $"Cohérence requise : SeuilMin ({seuilAutreActif.Valeur}) < SeuilMax ({dto.Valeur})."
                    );
            }

            var entity = _mapper.Map(dto);
            entity.DateCreation = DateTime.UtcNow;

            await _seuilRepo.AddAsync(entity);
            await _seuilRepo.SaveChangesAsync();

            var result = _mapper.Map(entity);
            result.SondeId = sonde.Id;

            return result;
        }

        public async Task<SeuilAlerteDto> UpdateAsync(Guid id, UpdateSeuilAlerteDto dto)
        {
            var entity = await _seuilRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Seuil introuvable.");

            var sonde = await _sondeRepo.GetByIdAsync(entity.SondeId)
                        ?? throw new ArgumentException("Sonde introuvable.");

            var willActivate = !entity.EstActif && dto.EstActif;

            // Si activation -> désactiver autre actif du même type
            if (willActivate)
            {
                var ancienActif = await _seuilRepo.FindActiveBySondeAndTypeAsync(entity.SondeId, entity.TypeSeuil);
                if (ancienActif != null && ancienActif.Id != entity.Id)
                {
                    ancienActif.EstActif = false;
                    await _seuilRepo.UpdateAsync(ancienActif);
                }
            }

            // Vérifier cohérence min/max si les deux actifs
            var autreType = entity.TypeSeuil == TypeSeuil.Minimum ? TypeSeuil.Maximum : TypeSeuil.Minimum;
            var seuilAutreActif = await _seuilRepo.FindActiveBySondeAndTypeAsync(entity.SondeId, autreType);
            decimal newValeur = (decimal) dto.Valeur;

            if (seuilAutreActif != null && (dto.EstActif || entity.EstActif))
            {
                if (entity.TypeSeuil == TypeSeuil.Minimum)
                {
                    if (!(newValeur < seuilAutreActif.Valeur))
                        throw new ArgumentException(
                            $"Cohérence requise : SeuilMin ({newValeur}) < SeuilMax ({seuilAutreActif.Valeur})."
                        );
                }
                else
                {
                    if (!(seuilAutreActif.Valeur < newValeur))
                        throw new ArgumentException(
                            $"Cohérence requise : SeuilMin ({seuilAutreActif.Valeur}) < SeuilMax ({newValeur})."
                        );
                }
            }

            entity.Valeur = (decimal) dto.Valeur;
            entity.EstActif = dto.EstActif;

            await _seuilRepo.UpdateAsync(entity);
            await _seuilRepo.SaveChangesAsync();

            var result = _mapper.Map(entity);
            result.SondeId = sonde.Id;
            return result;
        }

        public async Task ToggleAsync(Guid id)
        {
            var entity = await _seuilRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Seuil introuvable.");

            var newState = !entity.EstActif;

            if (newState)
            {
                // Désactiver l'autre actif du même type
                var ancienActif = await _seuilRepo.FindActiveBySondeAndTypeAsync(entity.SondeId, entity.TypeSeuil);
                if (ancienActif != null && ancienActif.Id != entity.Id)
                {
                    ancienActif.EstActif = false;
                    await _seuilRepo.UpdateAsync(ancienActif);
                }

                // Vérifier cohérence min<max
                var autreType = entity.TypeSeuil == TypeSeuil.Minimum ? TypeSeuil.Maximum : TypeSeuil.Minimum;
                var seuilAutreActif = await _seuilRepo.FindActiveBySondeAndTypeAsync(entity.SondeId, autreType);

                if (seuilAutreActif != null)
                {
                    if (entity.TypeSeuil == TypeSeuil.Minimum && !(entity.Valeur < seuilAutreActif.Valeur))
                        throw new ArgumentException($"Cohérence requise : {entity.Valeur} < {seuilAutreActif.Valeur}");

                    if (entity.TypeSeuil == TypeSeuil.Maximum && !(seuilAutreActif.Valeur < entity.Valeur))
                        throw new ArgumentException($"Cohérence requise : {seuilAutreActif.Valeur} < {entity.Valeur}");
                }
            }

            entity.EstActif = newState;
            await _seuilRepo.UpdateAsync(entity);
            await _seuilRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
          /*  var entity = await _seuilRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Seuil introuvable.");

            var countActiveAlerts = await _alerteRepo.CountActiveAlertsBySeuilIdAsync(id);
            if (countActiveAlerts > 0)
                throw new InvalidOperationException("Impossible de supprimer un seuil possédant des alertes actives.");

            await _seuilRepo.DeleteAsync(entity.Id);*/
            await _seuilRepo.SaveChangesAsync();
        }
    }
}
