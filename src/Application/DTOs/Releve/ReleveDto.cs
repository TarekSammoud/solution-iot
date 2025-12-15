using Domain.Enums;
using Application.DTOs.Sonde;
namespace Application.DTOs.Releve;

public class ReleveDto
{
    public Guid Id { get; set; }

    public Guid SondeId { get; set; }

    public decimal Valeur { get; set; }

    public DateTime DateHeure { get; set; }

    public TypeReleve TypeReleve { get; set; }

    public SondeDto? Sonde { get; set; } = null!;

}
