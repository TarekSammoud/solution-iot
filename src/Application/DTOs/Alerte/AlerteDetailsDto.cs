using Application.DTOs.SeuilAlerte;
using Application.DTOs.Sonde;
using Domain.Enums;

namespace Application.DTOs.Alertes;

public class AlerteDetailsDto
{
    public Guid Id { get; set; }

    public TypeSeuil TypeSeuil { get; set; }

    public TypeAlerte TypeAlerte { get; set; }

    public StatutAlerte Statut { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime? DateAcquittement { get; set; }

    public DateTime? DateResolution { get; set; }

    public string? Message { get; set; }

    // Relations détaillées
    public SondeDto Sonde { get; set; } = null!;

    public SeuilAlerteDto SeuilAlerte { get; set; } = null!;
}
