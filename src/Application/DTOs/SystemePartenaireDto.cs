using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

    public class SystemePartenaireDto
    {
        [Required(ErrorMessage = "L'identifiant est requis.")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Le nom est requis.")]
        [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères.")]
        public string Nom { get; set; } = string.Empty;
        [Required(ErrorMessage = "Le UrlBase est requis.")]
        [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères.")]
        public string UrlBase { get; set; } = string.Empty;
        public string? UsernameAppel { get; set; }   
        public string? PasswordChiffre { get; set; }
        public string? UsernameAcces { get; set; }
        public string? PasswordHashAcces { get; set; }
        public bool EstAppelant { get; set; } = false;
        public bool EstAppele { get; set; } = false;
        public bool EstActif { get; set; } = false;
        public DateTime DateCreation { get; set; }  = DateTime.Now;
    }

