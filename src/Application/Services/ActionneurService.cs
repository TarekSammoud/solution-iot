using Application.DTOs;
using Application.DTOs.Actionneur;
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
    public class ActionneurService : IActionneurService
    {
        private readonly IActionneurRepository _repository;
        private readonly IEtatActionneurRepository _etatRepository;

        public ActionneurService(IActionneurRepository repository, IEtatActionneurRepository etatRepository)
        {
            _repository = repository;
            _etatRepository = etatRepository;
        }

        public async Task<ActionneurDto> CreateAsync(CreateActionneurDto dto)
        {
            var actionneur = new Actionneur
            {
                Nom = dto.Nom,
                TypeActionneur = dto.TypeActionneur,
                LocalisationId = dto.LocalisationId,
                DateInstallation = dto.DateInstallation,
                DateCreation = DateTime.UtcNow
            };

            await _repository.AddAsync(actionneur);

            // Création de l'état initial
            var etat = new EtatActionneur
            {
                ActionneurId = actionneur.Id,
                EstActif = false,
                Pourcentage = 0,
                DerniereModification = DateTime.UtcNow
            };
            await _etatRepository.AddAsync(etat);

            return MapToDto(actionneur, etat);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var actionneur = await _repository.GetByIdAsync(id);
            if (actionneur == null) return false;

            var etat = await _etatRepository.GetByActionneurAsync(id);
            if (etat != null) await _etatRepository.DeleteAsync(etat.Id);

            await _repository.DeleteAsync(actionneur.Id);
            return true;
        }

        public async Task<IEnumerable<ActionneurDto>> GetAllAsync()
        {
            var actionneurs = await _repository.GetAllAsync();
            var etats = await _etatRepository.GetAllAsync();
            return actionneurs.Select(a =>
            {
                var etat = etats.FirstOrDefault(e => e.ActionneurId == a.Id);
                return MapToDto(a, etat);
            });
        }

        public async Task<IEnumerable<ActionneurDto>> GetByLocalisationAsync(Guid localisationId)
        {
            var actionneurs = await _repository.GetByLocalisationAsync(localisationId);
            var etats = await _etatRepository.GetAllAsync();
            return actionneurs.Select(a => MapToDto(a, etats.FirstOrDefault(e => e.ActionneurId == a.Id)));
        }

        public async Task<IEnumerable<ActionneurDto>> GetByTypeAsync(TypeActionneur type)
        {
            var actionneurs = await _repository.GetByTypeAsync(type);
            var etats = await _etatRepository.GetAllAsync();
            return actionneurs.Select(a => MapToDto(a, etats.FirstOrDefault(e => e.ActionneurId == a.Id)));
        }

        public async Task<ActionneurDto?> GetByIdAsync(Guid id)
        {
            var actionneur = await _repository.GetByIdAsync(id);
            if (actionneur == null) return null;

            var etat = await _etatRepository.GetByActionneurAsync(id);
            return MapToDto(actionneur, etat);
        }

        public async Task<ActionneurDto?> UpdateAsync(Guid id, UpdateActionneurDto dto)
        {
            var actionneur = await _repository.GetByIdAsync(id);
            if (actionneur == null) return null;

            actionneur.Nom = dto.Nom;
            actionneur.TypeActionneur = dto.TypeActionneur;
            actionneur.LocalisationId = dto.LocalisationId;
            actionneur.DateInstallation = dto.DateInstallation;

            await _repository.UpdateAsync(actionneur);

            var etat = await _etatRepository.GetByActionneurAsync(id);
            return MapToDto(actionneur, etat);
        }

        private ActionneurDto MapToDto(Actionneur a, EtatActionneur? e)
        {
            return new ActionneurDto
            {
                Id = a.Id,
                Nom = a.Nom,
                TypeActionneur = a.TypeActionneur,
                LocalisationId = a.LocalisationId,
                EtatActuel = e != null ? new EtatActionneurDto
                {
                    Id = e.Id,
                    ActionneurId = e.ActionneurId,
                    EstActif = e.EstActif,
                    Pourcentage = e.Pourcentage,
                    DerniereModification = e.DerniereModification
                } : null,
                DateCreation = a.DateCreation,
                DateInstallation = a.DateInstallation,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
