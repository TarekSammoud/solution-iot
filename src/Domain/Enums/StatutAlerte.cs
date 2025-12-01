namespace Domain.Enums;

/// <summary>
/// Statut d'une alerte dans son cycle de vie.
/// Une alerte passe par différents statuts depuis sa création jusqu'à sa résolution.
/// </summary>
public enum StatutAlerte
{
    /// <summary>
    /// Alerte active en cours.
    /// Le seuil est toujours dépassé et l'alerte n'a pas encore été prise en compte.
    /// État initial lors de la création de l'alerte.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Alerte acquittée par un utilisateur.
    /// L'alerte a été prise en compte mais le problème n'est pas encore résolu.
    /// Le seuil peut toujours être dépassé.
    /// Transition : Active → Acquittee (avec DateAcquittement définie).
    /// </summary>
    Acquittee = 1,

    /// <summary>
    /// Alerte résolue.
    /// Le relevé est revenu dans les limites normales (seuil non dépassé).
    /// Transition : Active/Acquittee → Resolue (avec DateResolution définie).
    /// État final de l'alerte.
    /// </summary>
    Resolue = 2
}
