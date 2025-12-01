namespace Domain.Entities;

/// <summary>
/// Représente un système partenaire IoT avec lequel notre plateforme peut communiquer.
/// Gère l'authentification Basic Auth bidirectionnelle (appels sortants et entrants).
/// </summary>
/// <remarks>
/// <para><strong>Authentification bidirectionnelle :</strong></para>
/// <list type="bullet">
/// <item>
/// <term>APPELER le partenaire (nous → lui)</term>
/// <description>Utilise UsernameAppel et PasswordChiffre (chiffré, déchiffrable pour faire l'appel)</description>
/// </item>
/// <item>
/// <term>ÊTRE APPELÉ par le partenaire (lui → nous)</term>
/// <description>Utilise UsernameAcces et PasswordHashAcces (haché, non déchiffrable, pour vérifier)</description>
/// </item>
/// </list>
/// <para><strong>Différence chiffrement vs hash :</strong></para>
/// <list type="bullet">
/// <item>PasswordChiffre : CHIFFRÉ (réversible) - on doit pouvoir le déchiffrer pour l'envoyer en Basic Auth</item>
/// <item>PasswordHashAcces : HACHÉ (irréversible) - on ne peut que vérifier, jamais récupérer le mot de passe original</item>
/// </list>
/// </remarks>
public class SystemePartenaire
{
    /// <summary>
    /// Identifiant unique du système partenaire.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nom du système partenaire (ex: "Plateforme IoT Ville", "Système Météo Régional").
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// URL de base du système partenaire (ex: "https://api.partenaire.com").
    /// </summary>
    public string UrlBase { get; set; } = string.Empty;

    // ===== POUR APPELER LE PARTENAIRE (nous → lui) =====

    /// <summary>
    /// Notre nom d'utilisateur pour appeler ce système partenaire.
    /// Optionnel : null si on n'appelle jamais ce système.
    /// </summary>
    public string? UsernameAppel { get; set; }

    /// <summary>
    /// Notre mot de passe CHIFFRÉ pour appeler ce système partenaire.
    /// IMPORTANT : Chiffré (réversible) car on doit pouvoir le déchiffrer pour l'envoyer en Basic Auth.
    /// Optionnel : null si on n'appelle jamais ce système.
    /// </summary>
    public string? PasswordChiffre { get; set; }

    // ===== POUR ÊTRE APPELÉ PAR LE PARTENAIRE (lui → nous) =====

    /// <summary>
    /// Le nom d'utilisateur que le partenaire doit utiliser pour nous appeler.
    /// Optionnel : null si ce système ne nous appelle jamais.
    /// </summary>
    public string? UsernameAcces { get; set; }

    /// <summary>
    /// Le mot de passe HACHÉ que le partenaire doit fournir pour nous appeler.
    /// IMPORTANT : Haché (irréversible) car on ne fait que vérifier, jamais envoyer.
    /// Optionnel : null si ce système ne nous appelle jamais.
    /// </summary>
    public string? PasswordHashAcces { get; set; }

    // ===== FLAGS =====

    /// <summary>
    /// Indique si ce système partenaire peut nous appeler (lui → nous).
    /// Si true, UsernameAcces et PasswordHashAcces doivent être définis.
    /// </summary>
    public bool EstAppelant { get; set; }

    /// <summary>
    /// Indique si on peut appeler ce système partenaire (nous → lui).
    /// Si true, UsernameAppel et PasswordChiffre doivent être définis.
    /// </summary>
    public bool EstAppele { get; set; }

    /// <summary>
    /// Indique si le système partenaire est actif.
    /// Un système inactif ne peut ni appeler ni être appelé.
    /// </summary>
    public bool EstActif { get; set; }

    /// <summary>
    /// Date et heure de création du système partenaire.
    /// </summary>
    public DateTime DateCreation { get; set; }
}
