using Domain.Enums;

namespace Application.DTOs.Releve;

public class CreateReleveDto
{

    public Guid SondeId { get; set; }

    public decimal Valeur { get; set; }

    public DateTime DateHeure { get; set; }

    public TypeReleve TypeReleve { get; set; }

}
