using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// DTO pour la lecture des informations d'une localisation.
/// Utilisé pour les opérations GET (récupération de données).
/// </summary>
public class LocalisationDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Le nom est requis.")]
    [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères.")]
    public string Nom { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
    public string? Description { get; set; }

    public DateTime DateCreation { get; set; }
}
