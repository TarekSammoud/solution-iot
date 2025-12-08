using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Sonde
{
    public class UpdateSondeDto
    {
        [Required]
        public Guid Id { get; set; }

        // Device properties
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public Guid LocalisationId { get; set; }

        public bool EstActif { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(UpdateSondeDto), nameof(ValidateDateInstallation))]
        public DateTime DateInstallation { get; set; }

        [Required]
        public CanalCommunication CanalCommunication { get; set; }

        public string? UrlDevice { get; set; }
        public string? CredentialsDevice { get; set; }

        // Sonde-specific properties
        [Required]
        public TypeSonde TypeSonde { get; set; }

        [Required]
        public Guid UniteMesureId { get; set; }

        public decimal? ValeurMin { get; set; }
        public decimal? ValeurMax { get; set; }

        public static ValidationResult? ValidateDateInstallation(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Now)
                return new ValidationResult("La date d'installation ne peut pas être dans le futur.");
            return ValidationResult.Success;
        }
    }
}