using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    /// <summary>
    /// DTO utilisé pour mettre à jour l'état actuel d'un actionneur.
    /// </summary>
    public class UpdateEtatActionneurDto
    {
        [Required]
        public bool EstActif { get; set; }

        /// <summary>
        /// Pourcentage d'intensité ou de vitesse (0-100).
        /// Pour AmpouleSimple, peut être ignoré ou fixé automatiquement.
        /// </summary>
        [Range(0, 100)]
        public int Pourcentage { get; set; }
    }
}
