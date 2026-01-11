using System.Net.Http.Json;
using Application.DTOs.Alertes;
using Domain.Enums;

namespace Presentation.Web.Services
{
    public class AlerteApiService
    {
        private readonly HttpClient _http;

        public AlerteApiService(HttpClient http)
        {
            _http = http;
        }

        // ---------------- DASHBOARD ----------------

        public async Task<IEnumerable<AlerteDto>> GetDashboardAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<AlerteDto>>("api/alerte/dashboard")
                   ?? Array.Empty<AlerteDto>();
        }

        // ---------------- BY SONDE ----------------

        public async Task<IEnumerable<AlerteDto>> GetBySondeAsync(
            Guid sondeId,
            StatutAlerte? statut = null,
            TypeSeuil? typeSeuil = null)
        {
            var url = $"api/alerte/sonde/{sondeId}";

            var query = new List<string>();
            if (statut.HasValue) query.Add($"statut={statut.Value}");
            if (typeSeuil.HasValue) query.Add($"typeSeuil={typeSeuil.Value}");
            if (query.Any())
                url += "?" + string.Join("&", query);

            return await _http.GetFromJsonAsync<IEnumerable<AlerteDto>>(url)
                   ?? Array.Empty<AlerteDto>();
        }

        // ---------------- DETAILS ----------------

        public async Task<AlerteDetailsDto?> GetDetailsAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<AlerteDetailsDto>($"api/alerte/{id}");
        }

        // ---------------- COMMANDS ----------------

        public async Task AcquitterAsync(Guid alerteId, string? commentaire = null)
        {
            var dto = new { Commentaire = commentaire };
            await _http.PostAsJsonAsync($"api/alerte/{alerteId}/acquitter", dto);
        }

        public async Task ResoudreAsync(Guid alerteId, string? commentaire = null)
        {
            var dto = new { Commentaire = commentaire };
            await _http.PostAsJsonAsync($"api/alerte/{alerteId}/resoudre", dto);
        }
    }
}
