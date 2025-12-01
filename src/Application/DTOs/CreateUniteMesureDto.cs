using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs;

public class CreateUniteMesureDto
{
    [Required]
    [StringLength(10)]
    public string Symbole { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    public TypeSonde TypeSonde { get; set; }
}
