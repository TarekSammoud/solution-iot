using Application.DTOs.Actionneur;
using Domain.Enums;
using System.Net.Http.Json;

namespace Presentation.Web.Services
{
    /// <summary>
    /// Service HTTP pour communiquer avec l'API REST Actionneur.
    /// Encapsule les appels HTTP vers les endpoints CRUD de l'API.
    /// </summary>
    public class ActionneurApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "api/actionneur";

        public ActionneurApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ActionneurDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ActionneurDto>>(ApiBaseUrl)
                ?? new List<ActionneurDto>();
        }

        public async Task<ActionneurDto?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<ActionneurDto>($"{ApiBaseUrl}/{id}");
        }

        public async Task<List<ActionneurDto>> GetByLocalisationAsync(Guid localisationId)
        {
            return await _httpClient.GetFromJsonAsync<List<ActionneurDto>>($"{ApiBaseUrl}/localisation/{localisationId}")
                ?? new List<ActionneurDto>();
        }

        public async Task<List<ActionneurDto>> GetByTypeAsync(TypeActionneur type)
        {
            return await _httpClient.GetFromJsonAsync<List<ActionneurDto>>($"{ApiBaseUrl}/type/{type}")
                ?? new List<ActionneurDto>();
        }

        public async Task<ActionneurDto?> CreateAsync(CreateActionneurDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ActionneurDto>();
        }

        public async Task<ActionneurDto?> UpdateAsync(UpdateActionneurDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{dto.Id}", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ActionneurDto>();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
