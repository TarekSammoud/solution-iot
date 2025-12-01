using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Représente un relevé de mesure effectué par une sonde à un instant donné.
/// Enregistre la valeur mesurée avec un timestamp précis et le type de relevé (manuel ou automatique).
/// </summary>
public class Releve
{
    /// <summary>
    /// Identifiant unique du relevé.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifiant de la sonde ayant effectué le relevé.
    /// </summary>
    public Guid SondeId { get; set; }

    /// <summary>
    /// Valeur mesurée par la sonde.
    /// Précision de 10 chiffres dont 2 décimales (ex: 12345678.90).
    /// </summary>
    public decimal Valeur { get; set; }

    /// <summary>
    /// Date et heure du relevé.
    /// </summary>
    public DateTime DateHeure { get; set; }

    /// <summary>
    /// Type de relevé (Manuel ou Automatique).
    /// </summary>
    public TypeReleve TypeReleve { get; set; }

    /// <summary>
    /// Navigation property vers la sonde qui a effectué le relevé.
    /// </summary>
    public Sonde Sonde { get; set; } = null!;

    // Note: PAS de navigation property vers Alerte comme demandé dans les spécifications
}
