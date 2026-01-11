using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Actionneur
{
    public class UpdateActionneurDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public TypeActionneur TypeActionneur { get; set; }

        [Required]
        public Guid LocalisationId { get; set; }

        public DateTime DateInstallation { get; set; }

        // Communication
        public string? UrlDevice { get; set; }
        public string? CanalCommunication { get; set; }
        public string? CredentialsDevice { get; set; }
    }
}
