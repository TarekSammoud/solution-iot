using Application.DTOs.SystemePartenaire;
using IotPlatform.Application.DTOs.External;
using System.Net.Http.Json;

namespace Presentation.Web.Services;

public class SystemePartenaireApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "api/SystemePartenaires";

    public SystemePartenaireApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SystemePartenaireDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<SystemePartenaireDto>>(ApiBaseUrl)
            ?? new List<SystemePartenaireDto>();
    }

    public async Task<SystemePartenaireDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<SystemePartenaireDto>($"{ApiBaseUrl}/{id}");
    }

    public async Task<SystemePartenaireDto?> CreateAsync(CreateSystemePartenaireDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SystemePartenaireDto>();
    }

    public async Task<SystemePartenaireDto?> UpdateAsync(UpdateSystemePartenaireDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SystemePartenaireDto>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ExternalSondeDto>> GetSondesFromPartenaireAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<List<ExternalSondeDto>>($"{ApiBaseUrl}/{id}/sondes")
            ?? new List<ExternalSondeDto>();
    }

    public async Task<ImportSondeResultDto?> ImportSondesFromPartenaireAsync(Guid id, Guid localisationCibleId, List<Guid>? sondeIds = null)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/{id}/import-sondes?localisationCibleId={localisationCibleId}", sondeIds ?? new List<Guid>());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ImportSondeResultDto>();
    }
}
