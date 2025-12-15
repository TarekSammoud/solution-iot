using Application.DTOs.Releve;
using System.Net.Http.Json;

namespace Presentation.Web.Services;

/// <summary>
/// Service HTTP pour communiquer avec l'API REST Releve.
/// Encapsule les appels HTTP vers les endpoints CRUD de l'API.
/// </summary>
public class ReleveApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "api/Releves";

    /// <summary>
    /// Initialise une nouvelle instance du service API Releve.
    /// </summary>
    /// <param name="httpClient">Le client HTTP configuré pour communiquer avec l'API.</param>
    public ReleveApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Récupère toutes les Releves depuis l'API.
    /// </summary>
    /// <returns>Une liste de toutes les Releves, ou une liste vide si aucune.</returns>
    public async Task<List<ReleveDto>> GetAllAsync(int page = 0, int limit = int.MaxValue)
    {
        return await _httpClient.GetFromJsonAsync<List<ReleveDto>>($"{ApiBaseUrl}?page={page}&limit={limit}")
            ?? new List<ReleveDto>();
    }

    /// <summary>
    /// Récupère une Releve par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la Releve.</param>
    /// <returns>La Releve trouvée, ou null si elle n'existe pas.</returns>
    public async Task<ReleveDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ReleveDto>($"{ApiBaseUrl}/{id}");
    }

    /// <summary>
    /// Crée une nouvelle Releve via l'API.
    /// </summary>
    /// <param name="dto">Les données de la Releve à créer.</param>
    /// <returns>La Releve créée avec son ID généré, ou null en cas d'erreur.</returns>
    /// <exception cref="HttpRequestException">Levée si la requête HTTP échoue.</exception>
    public async Task<ReleveDto?> CreateAsync(CreateReleveDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReleveDto>();
    }

    public async Task<List<ReleveDto>> GetBySondeAsync(Guid idSonde)
    {
        return await _httpClient.GetFromJsonAsync<List<ReleveDto>>($"{ApiBaseUrl}/sonde/{idSonde}");
    }
    public async Task<List<ReleveDto>> GetRecentBySondeAsync(Guid idSonde,int n = 5)
    {
        return await _httpClient.GetFromJsonAsync<List<ReleveDto>>($"{ApiBaseUrl}/sonde/{idSonde}/recent?n={n}");
    }


    /// <summary>
    /// Supprime une Releve via l'API.
    /// </summary>
    /// <param name="id">L'identifiant de la Releve à supprimer.</param>
    /// <returns>True si la suppression a réussi, false sinon.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
