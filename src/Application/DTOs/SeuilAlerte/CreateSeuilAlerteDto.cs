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

    [Range(-99999999.99, 99999999.99, ErrorMessage = "Valeur numérique invalide")]
    public decimal Valeur { get; set; } = 0m; 

    public bool EstActif { get; set; } = false;
}
