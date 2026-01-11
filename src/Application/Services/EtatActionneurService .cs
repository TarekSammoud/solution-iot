using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EtatActionneurService : IEtatActionneurService
    {
        private readonly IEtatActionneurRepository _etatRepository;
        private readonly IActionneurRepository _actionneurRepository;

        public EtatActionneurService(IEtatActionneurRepository etatRepository, IActionneurRepository actionneurRepository)
        {
            _etatRepository = etatRepository;
            _actionneurRepository = actionneurRepository;
        }

        public async Task<EtatActionneurDto?> GetEtatByActionneurIdAsync(Guid actionneurId)
        {
            var etat = await _etatRepository.GetByActionneurAsync(actionneurId);
            if (etat == null) return null;

            return new EtatActionneurDto
            {
                Id = etat.Id,
                ActionneurId = etat.ActionneurId,
                EstActif = etat.EstActif,
                Pourcentage = etat.Pourcentage,
                DerniereModification = etat.DerniereModification
            };
        }

        public async Task<EtatActionneurDto?> UpdateEtatAsync(Guid actionneurId, UpdateEtatActionneurDto dto)
        {
            var etat = await _etatRepository.GetByActionneurAsync(actionneurId);
            if (etat == null) return null;

            var actionneur = await _actionneurRepository.GetByIdAsync(actionneurId);
            if (actionneur == null) return null;

            // Validation selon type
            switch (actionneur.TypeActionneur)
            {
                case TypeActionneur.AmpouleSimple:
                    dto.Pourcentage = 0; 
                    break;
                case TypeActionneur.AmpouleVariometre:
                case TypeActionneur.Moteur:
                    if (dto.EstActif && ( dto.Pourcentage < 0 || dto.Pourcentage > 100))
                        throw new ArgumentException("Pourcentage obligatoire entre 0 et 100 si EstActif = true");
                    break;
            }

            etat.EstActif = dto.EstActif;
            etat.Pourcentage = dto.Pourcentage;
            etat.DerniereModification = DateTime.UtcNow;

            await _etatRepository.UpdateAsync(etat);

            return new EtatActionneurDto
            {
                Id = etat.Id,
                ActionneurId = etat.ActionneurId,
                EstActif = etat.EstActif,
                Pourcentage = etat.Pourcentage,
                DerniereModification = etat.DerniereModification
            };
        }
    }
}
