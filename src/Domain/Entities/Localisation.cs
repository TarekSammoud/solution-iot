namespace Domain.Entities;

/// <summary>
/// Représente une localisation physique où peuvent être installés des sondes et actionneurs.
/// Une localisation peut être une pièce, un espace ou une zone du bâtiment.
/// </summary>
public class Localisation
{
    /// <summary>
    /// Identifiant unique de la localisation.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nom de la localisation (ex: "Salon", "Cuisine", "Chambre 1").
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Description optionnelle de la localisation fournissant des détails supplémentaires.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date et heure de création de la localisation.
    /// </summary>
    public DateTime DateCreation { get; set; }

    // TODO: Décommenter quand l'entité Sonde sera créée
    // /// <summary>
    // /// Collection des sondes installées dans cette localisation.
    // /// Navigation property pour Entity Framework Core.
    // /// </summary>
    // public ICollection<Sonde> Sondes { get; set; } = new List<Sonde>();

    // TODO: Décommenter quand l'entité Actionneur sera créée
    // /// <summary>
    // /// Collection des actionneurs installés dans cette localisation.
    // /// Navigation property pour Entity Framework Core.
    // /// </summary>
    // public ICollection<Actionneur> Actionneurs { get; set; } = new List<Actionneur>();
}
