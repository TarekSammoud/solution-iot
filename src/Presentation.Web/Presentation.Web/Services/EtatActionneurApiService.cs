using Application.DTOs;
using System.Net.Http.Json;

namespace Presentation.Web.Services
{
    /// <summary>
    /// Service HTTP pour communiquer avec l'API REST EtatActionneur.
    /// Encapsule les appels HTTP vers les endpoints de gestion de l'état des actionneurs.
    /// </summary>
    public class EtatActionneurApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "api/etatactionneur";

        public EtatActionneurApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EtatActionneurDto?> GetEtatByActionneurIdAsync(Guid actionneurId)
        {
            return await _httpClient.GetFromJsonAsync<EtatActionneurDto>($"{ApiBaseUrl}/actionneur/{actionneurId}");
        }

        public async Task<EtatActionneurDto?> UpdateEtatAsync(Guid actionneurId, UpdateEtatActionneurDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/actionneur/{actionneurId}", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EtatActionneurDto>();
        }
    }
}
