using Application.DTOs.Sonde;
using Domain.Enums;
using System.Net.Http;
using System.Net.Http.Json;

namespace Presentation.Web.Services
{
    public class SondeApiService
    {
        private readonly HttpClient _http;

        public SondeApiService(HttpClient http)
        {
            _http = http;
        }

        // Get all sondes
        public async Task<IEnumerable<SondeDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<SondeDto>>("api/sonde") ?? Enumerable.Empty<SondeDto>();
        }

        // Get sonde by id
        public async Task<SondeDto?> GetByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<SondeDto>($"api/sonde/{id}");
        }

        // Get sondes by localisation
        public async Task<IEnumerable<SondeDto>> GetByLocalisationAsync(Guid localisationId)
        {
            return await _http.GetFromJsonAsync<IEnumerable<SondeDto>>($"api/sonde/localisation/{localisationId}") ?? Enumerable.Empty<SondeDto>();
        }

        // Get sondes by type
        public async Task<IEnumerable<SondeDto>> GetByTypeAsync(TypeSonde type)
        {
            return await _http.GetFromJsonAsync<IEnumerable<SondeDto>>($"api/sonde/type/{type}") ?? Enumerable.Empty<SondeDto>();
        }

        // Create a new sonde
        public async Task<SondeDto?> CreateAsync(CreateSondeDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/sonde", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SondeDto>();
        }

        // Update existing sonde
        public async Task<SondeDto?> UpdateAsync(UpdateSondeDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/sonde/{dto.Id}", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SondeDto>();
        }

        // Delete sonde
        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/sonde/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
