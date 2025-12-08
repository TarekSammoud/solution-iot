using Application.DTOs.Sonde;
using Domain.Entities;

namespace Application.Mappers
{
    public static class SondeMapper
    {
        // Entity -> DTO
        public static SondeDto ToDto(this Sonde sonde)
        {
            if (sonde == null) return null!;

            return new SondeDto
            {
                Id = sonde.Id,
                Nom = sonde.Nom,
                LocalisationId = sonde.LocalisationId,
                LocalisationNom = sonde.Localisation?.Nom ?? string.Empty,
                EstActif = sonde.EstActif,
                DateInstallation = sonde.DateInstallation,
                DateCreation = sonde.DateCreation,
                CanalCommunication = sonde.CanalCommunication,
                UrlDevice = sonde.UrlDevice,
                CredentialsDevice = sonde.CredentialsDevice,
                TypeSonde = sonde.TypeSonde,
                UniteMesureId = sonde.UniteMesureId,
                UniteMesureSymbole = sonde.UniteMesure?.Symbole ?? string.Empty,
              //  ValeurMin = sonde.ValeurMin,
               // ValeurMax = sonde.ValeurMax
            };
        }

        // Create DTO -> Entity
        public static Sonde ToEntity(this CreateSondeDto dto)
        {
            return new Sonde
            {
                Id = Guid.NewGuid(),
                Nom = dto.Nom,
                LocalisationId = dto.LocalisationId,
                EstActif = dto.EstActif,
                DateInstallation = dto.DateInstallation,
                DateCreation = DateTime.UtcNow,
                CanalCommunication = dto.CanalCommunication,
                UrlDevice = dto.UrlDevice,
                CredentialsDevice = dto.CredentialsDevice,
                TypeSonde = dto.TypeSonde,
                UniteMesureId = dto.UniteMesureId,
               // V = dto.ValeurMin,
              //  ValeurMax = dto.ValeurMax
            };
        }

        // Update DTO -> Entity
        public static void UpdateEntity(this Sonde sonde, UpdateSondeDto dto)
        {
            sonde.Nom = dto.Nom;
            sonde.LocalisationId = dto.LocalisationId;
            sonde.EstActif = dto.EstActif;
            sonde.DateInstallation = dto.DateInstallation;
            sonde.CanalCommunication = dto.CanalCommunication;
            sonde.UrlDevice = dto.UrlDevice;
            sonde.CredentialsDevice = dto.CredentialsDevice;
            sonde.TypeSonde = dto.TypeSonde;
            sonde.UniteMesureId = dto.UniteMesureId;
          /*  sonde.ValeurMin = dto.ValeurMin;
            sonde.ValeurMax = dto.ValeurMax;*/
        }
    }
}
