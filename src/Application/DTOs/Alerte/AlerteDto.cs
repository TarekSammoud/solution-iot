using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Alerte
{
    public class AlerteDto
    {
        public Guid Id { get; set; }
        public Guid SondeId { get; set; }
        public Guid SeuilAlerteId { get; set; }
        public Guid ReleveId { get; set; }
        public TypeSeuil TypeSeuil { get; set; }
        public StatutAlerte Statut { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateAcquittement { get; set; }
        public DateTime? DateResolution { get; set; }
        public string? Message { get; set; }
    }
}
