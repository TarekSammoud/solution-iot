using Application.DTOs.SeuilAlerte;
using System.Net.Http.Json;

namespace Presentation.Web.Services
{
    public class SeuilAlerteApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "api/seuilalerte";

        public SeuilAlerteApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SeuilAlerteDto>> GetBySondeAsync(Guid sondeId)
        {
            return await _httpClient.GetFromJsonAsync<List<SeuilAlerteDto>>(
                $"{ApiBaseUrl}/sonde/{sondeId}"
            ) ?? new List<SeuilAlerteDto>();
        }

        public async Task<SeuilAlerteDto?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<SeuilAlerteDto>($"{ApiBaseUrl}/{id}");
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateSeuilAlerteDto dto)
        {
            return await _httpClient.PostAsJsonAsync(ApiBaseUrl, dto);
        }

        public async Task<HttpResponseMessage> UpdateAsync(Guid id, UpdateSeuilAlerteDto dto)
        {
            return await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{id}", dto);
        }

        public async Task<HttpResponseMessage> ToggleAsync(Guid id)
        {
            return await _httpClient.PutAsync($"{ApiBaseUrl}/{id}/toggle", null);
        }

        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            return await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
        }
    }
}
