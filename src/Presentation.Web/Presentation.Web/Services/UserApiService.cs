using Application.DTOs;
using Application.DTOs.User;
using System.Net.Http.Json;

namespace Presentation.Web.Services;

/// <summary>
/// Service HTTP pour communiquer avec l'API REST User.
/// Encapsule les appels HTTP vers les endpoints CRUD de l'API.
/// </summary>
public class UserApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "api/users";

    /// <summary>
    /// Initialise une nouvelle instance du service API User.
    /// </summary>
    /// <param name="httpClient">Le client HTTP configuré pour communiquer avec l'API.</param>
    public UserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Récupère toutes les utilisateurs depuis l'API.
    /// </summary>
    /// <returns>Une liste de toutes les utilisateurs, ou une liste vide si aucune.</returns>
    public async Task<List<UserDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>(ApiBaseUrl)
            ?? new List<UserDto>();
    }

    public async Task<List<UserDto>> SearchUsersAsync(string query)
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>($"{ApiBaseUrl}?searchString={query}")
            ?? new List<UserDto>();
    }

    /// <summary>
    /// Récupère une utilisateur par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant unique de la utilisateur.</param>
    /// <returns>La utilisateur trouvée, ou null si elle n'existe pas.</returns>
    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>($"{ApiBaseUrl}/{id}");
    }

    /// <summary>
    /// Crée une nouvelle utilisateur via l'API.
    /// </summary>
    /// <param name="dto">Les données de la utilisateur à créer.</param>
    /// <returns>La utilisateur créée avec son ID généré, ou null en cas d'erreur.</returns>
    /// <exception cref="HttpRequestException">Levée si la requête HTTP échoue.</exception>
    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    /// <summary>
    /// Met à jour une utilisateur existante via l'API.
    /// </summary>
    /// <param name="dto">Les données de mise à jour de la utilisateur.</param>
    /// <returns>La utilisateur mise à jour, ou null en cas d'erreur.</returns>
    /// <exception cref="HttpRequestException">Levée si la requête HTTP échoue.</exception>
    public async Task<UserDto?> UpdateAsync(UpdateUserDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    /// <summary>
    /// Supprime une utilisateur via l'API.
    /// </summary>
    /// <param name="id">L'identifiant de la utilisateur à supprimer.</param>
    /// <returns>True si la suppression a réussi, false sinon.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
