using System.Net.Http.Json;
using Application.DTOs.Dashboard;

namespace Presentation.Web.Services;

public class DashboardApiService
{
    private readonly HttpClient _http;

    public DashboardApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DashboardSummaryDto?> GetSummaryAsync()
    {
        return await _http.GetFromJsonAsync<DashboardSummaryDto>("api/dashboard/summary");
    }
}