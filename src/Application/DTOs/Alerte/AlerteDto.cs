using Domain.Enums;

namespace Application.DTOs.Alertes;

public class AlerteDto
{
    public Guid Id { get; set; }

    public Guid SondeId { get; set; }

    public Guid SeuilAlerteId { get; set; }

    public TypeSeuil TypeSeuil { get; set; }

    public TypeAlerte TypeAlerte { get; set; }

    public StatutAlerte Statut { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime? DateAcquittement { get; set; }

    public DateTime? DateResolution { get; set; }

    public string? Message { get; set; }
}
