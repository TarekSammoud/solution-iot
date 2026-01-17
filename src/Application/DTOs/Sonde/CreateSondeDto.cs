using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Sonde
{
    /// <summary>
    /// Data Transfer Object for creating a new Sonde device.
    /// Contains all necessary properties with validation attributes.
    /// </summary>
    public class CreateSondeDto
    {
        #region Device Properties

        /// <summary>
        /// Device name - required field with length validation (1-100 characters)
        /// </summary>
        [Required(ErrorMessage = "Le nom de la sonde est requis.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Le nom doit contenir entre 1 et 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Location identifier - required field
        /// </summary>
        [Required(ErrorMessage = "La localisation est requise.")]
        public Guid LocalisationId { get; set; }

        /// <summary>
        /// Device active status - defaults to true
        /// </summary>
        public bool EstActif { get; set; } = true;

        /// <summary>
        /// Installation date - required field with custom validation to prevent future dates
        /// </summary>
        [Required(ErrorMessage = "La date d'installation est requise.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreateSondeDto), nameof(ValidateDateInstallation))]
        public DateTime DateInstallation { get; set; }

        /// <summary>
        /// Communication channel - required field
        /// </summary>
        [Required(ErrorMessage = "Le canal de communication est requis.")]
        public CanalCommunication CanalCommunication { get; set; }

        /// <summary>
        /// Optional device URL for communication
        /// </summary>
        public string? UrlDevice { get; set; }

        /// <summary>
        /// Optional device credentials for authentication
        /// </summary>
        public string? CredentialsDevice { get; set; }

        #endregion

        #region Sonde-Specific Properties

        /// <summary>
        /// Sonde type - required field
        /// </summary>
        [Required(ErrorMessage = "Le type de sonde est requis.")]
        public TypeSonde TypeSonde { get; set; }

        /// <summary>
        /// Measurement unit identifier - required field
        /// </summary>
        [Required(ErrorMessage = "L'unité de mesure est requise.")]
        public Guid UniteMesureId { get; set; }

        /// <summary>
        /// Optional minimum measurement value
        /// </summary>
        public decimal? ValeurMin { get; set; }

        /// <summary>
        /// Optional maximum measurement value
        /// </summary>
        public decimal? ValeurMax { get; set; }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates that the installation date is not in the future.
        /// This is a custom validation method used by DataAnnotations.
        /// </summary>
        /// <param name="date">The installation date to validate</param>
        /// <param name="context">The validation context</param>
        /// <returns>ValidationResult.Success if valid, or a ValidationResult with error message if invalid</returns>
        public static ValidationResult? ValidateDateInstallation(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Now)
            {
                return new ValidationResult("La date d'installation ne peut pas être dans le futur.");
            }
            return ValidationResult.Success;
        }

        #endregion
    }
}