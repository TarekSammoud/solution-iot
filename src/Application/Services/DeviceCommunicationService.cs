using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.DTOs.Device;
using Application.DTOs.Releve;
using Application.DTOs.Sonde;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class DeviceCommunicationService : IDeviceCommunicationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISondeRepository _sondeRepository;
    private readonly IReleveService _releveService;
    private readonly IActionneurRepository _actionneurRepository;
    private readonly ILogger<DeviceCommunicationService> _logger;

    public DeviceCommunicationService(
        IHttpClientFactory httpClientFactory,
        ISondeRepository sondeRepository,
        IReleveService releveService,
        IActionneurRepository actionneurRepository,
        ILogger<DeviceCommunicationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _sondeRepository = sondeRepository;
        _releveService = releveService;
        _actionneurRepository = actionneurRepository;
        _logger = logger;
    }

    public async Task<ReleveDto?> PullDataFromDevice(SondeDto sonde)
    {
        if (sonde.CanalCommunication != CanalCommunication.HttpPull || 
            string.IsNullOrEmpty(sonde.UrlDevice) || 
            !sonde.EstActif)
        {
            return null;
        }

        try
        {
            var data = await GetDataFromDevice(sonde);
            if (data == null) return null;

            var createReleveDto = new CreateReleveDto
            {
                SondeId = sonde.Id,
                Valeur = data.Value,
                DateHeure = data.Timestamp ?? DateTime.UtcNow,
                TypeReleve = TypeReleve.Automatique
            };

            return await _releveService.CreateAsync(createReleveDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pulling data from device {SondeId}", sonde.Id);
            return null;
        }
    }

    public async Task<TestCommunicationResultDto> TestConnection(SondeDto sonde)
    {
        var result = new TestCommunicationResultDto { Success = false };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrEmpty(sonde.UrlDevice))
            {
                result.Message = "URL du device non définie";
                return result;
            }

            var data = await GetDataFromDevice(sonde);
            stopwatch.Stop();

            if (data != null)
            {
                result.Success = true;
                result.Value = data.Value;
                result.Timestamp = data.Timestamp;
                result.Duration = stopwatch.Elapsed;
                result.Message = "Connexion réussie";
            }
            else
            {
                result.Message = "Aucune donnée reçue ou erreur de parsing";
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Message = $"Erreur: {ex.Message}";
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<ReleveDto?> ReceiveDataFromDevice(Guid deviceId, DeviceDataDto data)
    {
        var sonde = await _sondeRepository.GetByIdAsync(deviceId);
        if (sonde != null)
        {
            if (!sonde.EstActif) throw new InvalidOperationException("Device inactive");

            if (data.Timestamp > DateTime.UtcNow.AddMinutes(5))
            {
                throw new ArgumentException("Timestamp cannot be in the future");
            }

            var createReleveDto = new CreateReleveDto
            {
                SondeId = sonde.Id,
                Valeur = data.Value,
                DateHeure = data.Timestamp ?? DateTime.UtcNow,
                TypeReleve = TypeReleve.Automatique
            };

            return await _releveService.CreateAsync(createReleveDto);
        }

        var actionneur = await _actionneurRepository.GetByIdAsync(deviceId);
        if (actionneur != null)
        {
            if (!actionneur.EstActif) throw new InvalidOperationException("Device inactive");
            // Logic to update actionneur state would go here
            return null; 
        }

        throw new KeyNotFoundException($"Device {deviceId} not found");
    }

    private async Task<DeviceDataDto?> GetDataFromDevice(SondeDto sonde)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(sonde.UrlDevice!);
        httpClient.Timeout = TimeSpan.FromSeconds(10);

        if (!string.IsNullOrEmpty(sonde.CredentialsDevice))
        {
            var parts = sonde.CredentialsDevice.Split(':');
            if (parts.Length == 2)
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{parts[0]}:{parts[1]}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            }
        }

        var response = await httpClient.GetAsync("data");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
             response = await httpClient.GetAsync("");
        }

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        try 
        {
            return JsonSerializer.Deserialize<DeviceDataDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            // If the response is not valid JSON (e.g. HTML error page)
            throw new Exception($"Invalid response format from device. Content preview: {json.Substring(0, Math.Min(json.Length, 100))}");
        }
    }
}
