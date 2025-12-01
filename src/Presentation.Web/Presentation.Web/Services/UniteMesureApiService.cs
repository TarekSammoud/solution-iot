using Application.DTOs;
using Domain.Enums;
using System.Net.Http.Json;

namespace Presentation.Web.Services;

public class UniteMesureApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "api/unitemesure";

    public UniteMesureApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UniteMesureDto>> GetAllAsync(TypeSonde? filter = null)
    {
        var url = filter.HasValue ? $"{ApiBaseUrl}?typeSonde={(int)filter.Value}" : ApiBaseUrl;
        return await _httpClient.GetFromJsonAsync<List<UniteMesureDto>>(url) ?? new List<UniteMesureDto>();
    }

    public async Task<UniteMesureDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<UniteMesureDto>($"{ApiBaseUrl}/{id}");
    }

    public async Task<UniteMesureDto?> CreateAsync(CreateUniteMesureDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UniteMesureDto>();
    }

    public async Task<UniteMesureDto?> UpdateAsync(UpdateUniteMesureDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UniteMesureDto>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
