using Application.DTOs.SystemePartenaire;
using Application.Mappers;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using IotPlatform.Application.DTOs.External;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Services;

/// <summary>
/// Service de gestion des SystemePartenaires.
/// Orchestre la logique métier CRUD en utilisant le repository et le mapper.
/// </summary>
public class SystemePartenaireService : ISystemePartenaireService
{
    private readonly ISystemePartenaireRepository _repository;
    private readonly SystemePartenaireMapper _mapper;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISondeRepository _sondeRepository;
    private readonly IUniteMesureRepository _uniteMesureRepository;
    private readonly ILocalisationRepository _localisationRepository;

    /// <summary>
    /// Initialise une nouvelle instance du service de SystemePartenaire.
    /// </summary>
    /// <param name="repository">Le repository pour accéder aux données.</param>
    /// <param name="mapper">Le mapper pour convertir entre entités et DTOs.</param>
    /// <param name="httpClientFactory">Le factory pour créer des HttpClient.</param>
    /// <param name="sondeRepository">Le repository pour les sondes.</param>
    /// <param name="uniteMesureRepository">Le repository pour les unités de mesure.</param>
    /// <param name="localisationRepository">Le repository pour les localisations.</param>
    public SystemePartenaireService(
        ISystemePartenaireRepository repository, 
        SystemePartenaireMapper mapper,
        IHttpClientFactory httpClientFactory,
        ISondeRepository sondeRepository,
        IUniteMesureRepository uniteMesureRepository,
        ILocalisationRepository localisationRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
        _sondeRepository = sondeRepository;
        _uniteMesureRepository = uniteMesureRepository;
        _localisationRepository = localisationRepository;
    }

    /// <summary>
    /// Récupère une SystemePartenaire par son identifiant.
    /// </summary>
    public async Task<SystemePartenaireDto?> GetByIdAsync(Guid id)
    {
        var SystemePartenaire = await _repository.GetByIdAsync(id);

        if (SystemePartenaire == null)
        {
            return null;
        }

        return _mapper.ToDto(SystemePartenaire);
    }

    /// <summary>
    /// Récupère toutes les SystemePartenaires.
    /// </summary>
    public async Task<IEnumerable<SystemePartenaireDto>> GetAllAsync()
    {
        var SystemePartenaires = await _repository.GetAllAsync();
        return _mapper.ToDtoList(SystemePartenaires);
    }

    /// <summary>
    /// Crée une nouvelle SystemePartenaire.
    /// Id et DateCreation sont générés automatiquement par le repository.
    /// </summary>
    public async Task<SystemePartenaireDto> CreateAsync(CreateSystemePartenaireDto dto)
    {
        // Mapper le DTO vers l'entité
        var SystemePartenaire = _mapper.ToEntity(dto);

        // Créer l'entité (Id et DateCreation générés par le repository)
        var created = await _repository.AddAsync(SystemePartenaire);

        // Retourner le DTO de l'entité créée
        return _mapper.ToDto(created);
    }

    /// <summary>
    /// Met à jour une SystemePartenaire existante.
    /// Préserve Id et DateCreation (propriétés immutables).
    /// </summary>
    public async Task<SystemePartenaireDto?> UpdateAsync(UpdateSystemePartenaireDto dto)
    {
        // Vérifier que la SystemePartenaire existe
        var SystemePartenaire = await _repository.GetByIdAsync(dto.Id);

        if (SystemePartenaire == null)
        {
            return null;
        }

        // Mettre à jour l'entité existante (préserve Id et DateCreation)
        _mapper.UpdateEntity(dto, SystemePartenaire);

        // Sauvegarder les modifications
        await _repository.UpdateAsync(SystemePartenaire);

        // Retourner le DTO de l'entité mise à jour
        return _mapper.ToDto(SystemePartenaire);
    }

    /// <summary>
    /// Supprime une SystemePartenaire.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Vérifier que la SystemePartenaire existe
        var exists = await _repository.ExistsAsync(id);

        if (!exists)
        {
            return false;
        }

        // Supprimer la SystemePartenaire
        await _repository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// Vérifie si une SystemePartenaire existe.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }

    /// <summary>
    /// Récupère les sondes disponibles depuis un système partenaire.
    /// </summary>
    public async Task<List<ExternalSondeDto>> GetSondesFromPartenaire(Guid systemePartenaireId)
    {
        // Récupérer le système partenaire
        var systemePartenaire = await _repository.GetByIdAsync(systemePartenaireId);
        if (systemePartenaire == null)
        {
            throw new ArgumentException("Le système partenaire n'existe pas");
        }

        // Vérifier que le système a les credentials nécessaires
        if (string.IsNullOrEmpty(systemePartenaire.UsernameAppel) || string.IsNullOrEmpty(systemePartenaire.PasswordChiffre))
        {
            throw new InvalidOperationException("Le système partenaire n'a pas de credentials configurés pour l'appel");
        }

        // Vérifier que l'URL est valide
        if (string.IsNullOrEmpty(systemePartenaire.UrlBase) || !Uri.TryCreate(systemePartenaire.UrlBase, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("L'URL du système partenaire n'est pas valide");
        }

        // Créer le HttpClient
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(systemePartenaire.UrlBase);
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        // Ajouter l'authentification Basic Auth
        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{systemePartenaire.UsernameAppel}:{systemePartenaire.PasswordChiffre}")
        );
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        try
        {
            // Effectuer l'appel GET
            var response = await httpClient.GetAsync("/api/Sonde");

            if (!response.IsSuccessStatusCode)
            {
                // Gérer les erreurs selon le code de statut
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Authentification échouée");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Endpoint non trouvé chez le partenaire");
                }
                else
                {
                    throw new Exception($"Erreur HTTP {response.StatusCode} lors de l'appel au système partenaire");
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            var sondes = JsonSerializer.Deserialize<List<ExternalSondeDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

            return sondes ?? new List<ExternalSondeDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Erreur de communication avec le système partenaire", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("Timeout lors de la communication", ex);
        }
    }

    /// <summary>
    /// Importe les sondes depuis un système partenaire vers une localisation cible.
    /// </summary>
    public async Task<ImportSondeResultDto> ImportSondesFromPartenaire(Guid systemePartenaireId, Guid localisationCibleId, List<Guid>? sondeIds = null)
    {
        // Récupérer les sondes du partenaire
        var sondesExternes = await GetSondesFromPartenaire(systemePartenaireId);

        // Filtrer les sondes si une liste d'IDs est fournie
        if (sondeIds != null && sondeIds.Any())
        {
            sondesExternes = sondesExternes.Where(s => sondeIds.Contains(s.Id)).ToList();
        }

        // Vérifier que la localisation cible existe
        var localisationCible = await _localisationRepository.GetByIdAsync(localisationCibleId);
        if (localisationCible == null)
        {
            throw new ArgumentException("La localisation cible n'existe pas");
        }

        var resultat = new ImportSondeResultDto();
        var sondesExistantes = await _sondeRepository.GetAllWithRelationsAsync();
        var unitesMesure = await _uniteMesureRepository.GetAllAsync();

        foreach (var sondeExterne in sondesExternes)
        {
            try
            {
                // Vérifier si la sonde existe déjà localement (par nom et type)
                var sondeExistante = sondesExistantes.FirstOrDefault(s => 
                    s.Nom.Equals(sondeExterne.Nom, StringComparison.OrdinalIgnoreCase) && 
                    s.TypeSonde == sondeExterne.TypeSonde);

                if (sondeExistante != null)
                {
                    resultat.NombreDoublons++;
                    resultat.Erreurs.Add($"Sonde '{sondeExterne.Nom}' déjà existante");
                    continue;
                }

                // Mapper/Créer l'unité de mesure
                var uniteMesure = unitesMesure.FirstOrDefault(u => 
                    u.Symbole.Equals(sondeExterne.UniteMesureSymbole, StringComparison.OrdinalIgnoreCase));

                if (uniteMesure == null)
                {
                    // Créer une nouvelle unité de mesure si elle n'existe pas
                    uniteMesure = new UniteMesure
                    {
                        Nom = $"Unité pour {sondeExterne.TypeSonde}",
                        Symbole = sondeExterne.UniteMesureSymbole,
                        TypeSonde = sondeExterne.TypeSonde,
                        DateCreation = DateTime.UtcNow
                    };
                    uniteMesure = await _uniteMesureRepository.AddAsync(uniteMesure);
                    unitesMesure = await _uniteMesureRepository.GetAllAsync(); // Rafraîchir la liste
                }

                // Créer la nouvelle sonde
                var nouvelleSonde = new Sonde
                {
                    Nom = sondeExterne.Nom + " (importée)",
                    TypeSonde = sondeExterne.TypeSonde,
                    UniteMesureId = uniteMesure.Id,
                    LocalisationId = localisationCibleId,
                    EstActif = false, // Inactif par défaut
                    DateInstallation = sondeExterne.DateInstallation,
                    DateCreation = DateTime.UtcNow,
                    CanalCommunication = CanalCommunication.HttpPush, // Par défaut
                    ValeurMin = null,
                    ValeurMax = null,
                    UrlDevice = null,
                    CredentialsDevice = null
                };

                await _sondeRepository.AddAsync(nouvelleSonde);
                resultat.NombreImportees++;
            }
            catch (Exception ex)
            {
                resultat.Erreurs.Add($"Erreur lors de l'import de '{sondeExterne.Nom}': {ex.Message}");
            }
        }

        return resultat;
    }
}
