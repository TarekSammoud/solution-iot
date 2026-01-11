using System;

namespace Application.DTOs
{
    /// <summary>
    /// DTO pour l'affichage de l'état actuel d'un actionneur.
    /// </summary>
    public class EtatActionneurDto
    {
        public Guid Id { get; set; }
        public Guid ActionneurId { get; set; }

        /// <summary>
        /// État de l'actionneur : true = ON, false = OFF.
        /// </summary>
        public bool EstActif { get; set; }

        /// <summary>
        /// Pourcentage d'intensité ou de vitesse selon le type d'actionneur.
        /// </summary>
        public int Pourcentage { get; set; }

        /// <summary>
        /// Date de la dernière modification.
        /// </summary>
        public DateTime DerniereModification { get; set; }
    }
}
