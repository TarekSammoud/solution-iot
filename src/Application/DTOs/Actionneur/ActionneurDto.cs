using Domain.Enums;
using System;

namespace Application.DTOs.Actionneur
{
    public class ActionneurDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public TypeActionneur TypeActionneur { get; set; }
        public Guid LocalisationId { get; set; }

        // État actuel
        public EtatActionneurDto? EtatActuel { get; set; }

        public DateTime DateCreation { get; set; }
        public DateTime DateInstallation { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
