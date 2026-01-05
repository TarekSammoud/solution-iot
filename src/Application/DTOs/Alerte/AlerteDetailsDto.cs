using Application.DTOs.Releve;
using Application.DTOs.SeuilAlerte;
using Application.DTOs.Sonde;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Alerte
{
    public class AlerteDetailsDto
    {
        public Guid Id { get; set; }
      //  public Guid SondeId { get; set; }
        public SondeDto Sonde { get; set; }
      //  public Guid SeuilAlerteId { get; set; }
        public SeuilAlerteDto SeuilAlerte { get; set; }
      //  public Guid ReleveId { get; set; }
        public ReleveDto Releve { get; set; }
        public TypeSeuil TypeSeuil { get; set; }
        public StatutAlerte Statut { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateAcquittement { get; set; }
        public DateTime? DateResolution { get; set; }
        public string? Message { get; set; }
    }
}
