using Domain.Enums;

namespace Application.DTOs.SeuilAlerte;

public class SeuilAlerteDto
{
    public Guid Id { get; set; }
    public Guid SondeId { get; set; }
    public TypeSeuil TypeSeuil { get; set; }
    public TypeAlerte TypeAlerte { get; set; }

    public decimal Valeur { get; set; }
    public bool EstActif { get; set; }

    public DateTime DateCreation { get; set; }
}
