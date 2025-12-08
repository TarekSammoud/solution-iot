using Application.DTOs.UniteMesure;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Sonde
{
    public class SondeDto
    {
        public Guid Id { get; set; }
        [Required]
        [Length(1, 100)]
        public string Nom { get; set; } = string.Empty;
        [Required]
        public Guid LocalisationId { get; set; }
        public string LocalisationNom { get; set; } = string.Empty;

        public bool EstActif { get; set; } = true;
        [Required]
        public DateTime DateInstallation { get; set; }
        [Required]
        public DateTime DateCreation { get; set; }
        [Required]
        public CanalCommunication CanalCommunication { get; set; }
        public string? UrlDevice { get; set; }
        public string? CredentialsDevice { get; set; }

        // Sonde properties
        public TypeSonde TypeSonde { get; set; }
        public Guid UniteMesureId { get; set; }
        public string UniteMesureSymbole { get; set; } = string.Empty; 
        public decimal? ValeurMin { get; set; }
        public decimal? ValeurMax { get; set; }
    }
}
