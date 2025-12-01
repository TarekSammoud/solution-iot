using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// DTO pour la création d'une nouvelle localisation.
/// Utilisé pour les opérations POST (création de données).
/// Id et DateCreation sont générés automatiquement par le repository.
/// </summary>
public class CreateLocalisationDto
{
    [Required(ErrorMessage = "Le nom est requis.")]
    [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères.")]
    public string Nom { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
    public string? Description { get; set; }
}
