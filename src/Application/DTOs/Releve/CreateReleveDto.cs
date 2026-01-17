using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Releve;

public class CreateReleveDto
{
    [Required(ErrorMessage = "La sonde est requise.")]
    public Guid SondeId { get; set; }

    [Required(ErrorMessage = "La valeur est requise.")]
    public decimal Valeur { get; set; }

    [Required(ErrorMessage = "La date et l'heure sont requises.")]
    public DateTime DateHeure { get; set; }

    [Required(ErrorMessage = "Le type de relevé est requis.")]
    public TypeReleve TypeReleve { get; set; }

}
