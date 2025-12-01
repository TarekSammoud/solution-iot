using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

    public class CreateSystemePartenaireDto : IValidatableObject
    {

    [Required(ErrorMessage = "Le nom est requis.")]
    [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères.")]
    public string Nom { get; set; } = string.Empty;
    [Required(ErrorMessage = "Le UrlBase est requis.")]
    [Url(ErrorMessage = "Le UrlBase doit être une URL valide.")]
    [StringLength(200, ErrorMessage = "Le UrlBase ne peut pas dépasser 200 caractères.")]
    public string UrlBase { get; set; } = string.Empty;
        public string? UsernameAppel { get; set; }   
        public string? PasswordChiffre { get; set; }
        public string? UsernameAcces { get; set; }
        public string? PasswordHashAcces { get; set; }
        public bool EstAppelant { get; set; } = false;
        public bool EstAppele { get; set; } = false;
        public bool EstActif { get; set; } = false;
        public DateTime DateCreation { get; set; }  = DateTime.Now;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(UrlBase))
            {
                if (!Uri.TryCreate(UrlBase, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    yield return new ValidationResult(
                        "L'URL de base doit être une URL http(s) absolue.",
                        new[] { nameof(UrlBase) }
                    );
                }
            }
            if (!string.IsNullOrWhiteSpace(UsernameAppel) && string.IsNullOrWhiteSpace(PasswordChiffre))
            {
                yield return new ValidationResult(
                    "PasswordChiffre est requis lorsque UsernameAppel est renseigné.",
                    new[] { nameof(PasswordChiffre) }
                );
            }

            if (!string.IsNullOrWhiteSpace(UsernameAcces) && string.IsNullOrWhiteSpace(PasswordHashAcces))
            {
                yield return new ValidationResult(
                    "PasswordHashAcces est requis lorsque UsernameAcces est renseigné.",
                    new[] { nameof(PasswordHashAcces) }
                );
            }
        }
    }
