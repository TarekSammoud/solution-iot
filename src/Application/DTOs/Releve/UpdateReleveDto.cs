using Application.DTOs.Sonde;
using Domain.Enums;

namespace Application.DTOs.Releve;

public class UpdateReleveDto
{
    public Guid Id { get; set; }

    public Guid SondeId { get; set; }

    public decimal Valeur { get; set; }

    public DateTime DateHeure { get; set; }

    public TypeReleve TypeReleve { get; set; }

    public SondeDto? Sonde { get; set; } = null!;

}
