using Application.DTOs.Localisation;
using System.Net.Http.Json;

namespace Presentation.Web.Services;

/// <summary>
/// Service HTTP pour communiquer avec l'API REST Localisation.
/// Encapsule les appels HTTP vers les endpoints CRUD de l'API.
/// </summary>
public class LocalisationApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "api/localisations";

    /// <summary>
    /// Initialise une nouvelle instance du service API Localisation.
    /// </summary>
    /// <param name="httpClient">Le client HTTP configuré pour communiquer avec l'API.</param>
    public LocalisationApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Récupère toutes les localisations depuis l'API.
    /// </summary>
    /// <returns>Une liste de toutes les localisations, ou une liste vide si aucune.</returns>
    public async Task<List<LocalisationDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<LocalisationDto>>(ApiBaseUrl)
            ?? new List<LocalisationDto>();
    }

    /// <summary>
    /// Récupère une localisation par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la localisation.</param>
    /// <returns>La localisation trouvée, ou null si elle n'existe pas.</returns>
    public async Task<LocalisationDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<LocalisationDto>($"{ApiBaseUrl}/{id}");
    }

    /// <summary>
    /// Crée une nouvelle localisation via l'API.
    /// </summary>
    /// <param name="dto">Les données de la localisation à créer.</param>
    /// <returns>La localisation créée avec son ID généré, ou null en cas d'erreur.</returns>
    /// <exception cref="HttpRequestException">Levée si la requête HTTP échoue.</exception>
    public async Task<LocalisationDto?> CreateAsync(CreateLocalisationDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LocalisationDto>();
    }

    /// <summary>
    /// Met à jour une localisation existante via l'API.
    /// </summary>
    /// <param name="dto">Les données de mise à jour de la localisation.</param>
    /// <returns>La localisation mise à jour, ou null en cas d'erreur.</returns>
    /// <exception cref="HttpRequestException">Levée si la requête HTTP échoue.</exception>
    public async Task<LocalisationDto?> UpdateAsync(UpdateLocalisationDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LocalisationDto>();
    }

    /// <summary>
    /// Supprime une localisation via l'API.
    /// </summary>
    /// <param name="id">L'identifiant de la localisation à supprimer.</param>
    /// <returns>True si la suppression a réussi, false sinon.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
