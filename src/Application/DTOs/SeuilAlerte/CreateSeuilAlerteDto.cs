using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.SeuilAlerte;

public class CreateSeuilAlerteDto
{
    [Required]
    public Guid SondeId { get; set; }

    [Required]
    public TypeSeuil TypeSeuil { get; set; }

    [Required]
    public TypeAlerte TypeAlerte { get; set; }

    [Required]
    public double Valeur { get; set; }

    public bool EstActif { get; set; } = false;
}
