using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs;

public class UniteMesureDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(10)]
    public string Symbole { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    public TypeSonde TypeSonde { get; set; }

    public DateTime DateCreation { get; set; }
}
