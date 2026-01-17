using System;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace IotPlatform.Application.DTOs.External
{
    public class ExternalSondeDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("nom")]
        public string Nom { get; set; } = string.Empty;

        [JsonPropertyName("typeSonde")]
        public TypeSonde TypeSonde { get; set; }

        [JsonPropertyName("uniteMesureSymbole")]
        public string UniteMesureSymbole { get; set; } = string.Empty;

        [JsonPropertyName("localisationNom")]
        public string LocalisationNom { get; set; } = string.Empty;

        [JsonPropertyName("estActif")]
        public bool EstActif { get; set; }

        [JsonPropertyName("dateInstallation")]
        public DateTime DateInstallation { get; set; }
    }
}