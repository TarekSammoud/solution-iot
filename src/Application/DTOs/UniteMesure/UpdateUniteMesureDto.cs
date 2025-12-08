using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.UniteMesure;

public class UpdateUniteMesureDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(10)]
    public string Symbole { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    public TypeSonde TypeSonde { get; set; }
}
