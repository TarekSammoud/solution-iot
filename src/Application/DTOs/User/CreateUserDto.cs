using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.User
{
    public class CreateUserDto
    {

        /// <summary>
        /// Identifiant unique de l'utilisateur.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nom d'utilisateur unique utilisé pour l'authentification.
        /// </summary>
        [Required(ErrorMessage = "Le Username est requis.")]
        
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Adresse email unique de l'utilisateur.
        /// </summary>
        [Required(ErrorMessage = "L'adresse mail est requise.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash du mot de passe (non réversible) pour la sécurité.
        /// Ne jamais stocker le mot de passe en clair.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'utilisateur déterminant ses droits d'accès.
        /// </summary>

        [Required]
        public RoleUtilisateur Role { get; set; }

        /// <summary>
        /// Indique si le compte utilisateur est actif.
        /// Un compte inactif ne peut pas se connecter au système.
        /// </summary>
        [Required]
        public bool EstActif { get; set; }

    }
}
