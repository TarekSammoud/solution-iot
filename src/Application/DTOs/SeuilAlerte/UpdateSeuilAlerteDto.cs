using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.SeuilAlerte;

public class UpdateSeuilAlerteDto
{
    [Required]
    public TypeSeuil TypeSeuil { get; set; }

    [Required]
    public TypeAlerte TypeAlerte { get; set; }

    [Required]
    public decimal Valeur { get; set; }

    public bool EstActif { get; set; }
}
