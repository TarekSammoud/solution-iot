using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// DTO pour la mise à jour d'une localisation existante.
/// Utilisé pour les opérations PUT (modification de données).
/// DateCreation n'est pas inclus car c'est une propriété immutable.
/// </summary>
public class UpdateLocalisationDto
{
    [Required(ErrorMessage = "L'identifiant est requis.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Le nom est requis.")]
    [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères.")]
    public string Nom { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
    public string? Description { get; set; }
}
