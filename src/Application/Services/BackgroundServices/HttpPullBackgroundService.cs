using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services.BackgroundServices;

public class HttpPullBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HttpPullBackgroundService> _logger;

    public HttpPullBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<HttpPullBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var communicationService = scope.ServiceProvider.GetRequiredService<IDeviceCommunicationService>();
                var sondeService = scope.ServiceProvider.GetRequiredService<ISondeService>();

                // Récupérer les sondes HttpPull actives
                var allSondes = await sondeService.GetAllAsync();
                var sondes = allSondes
                    .Where(s => s.CanalCommunication == CanalCommunication.HttpPull && s.EstActif)
                    .ToList();

                _logger.LogInformation($"Interrogation de {sondes.Count} sondes HttpPull");

                int success = 0;
                int errors = 0;

                foreach (var sonde in sondes)
                {
                    var result = await communicationService.PullDataFromDevice(sonde);
                    if (result != null)
                        success++;
                    else
                        errors++;
                }

                _logger.LogInformation($"Résultat: {success} succès, {errors} échecs");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur dans HttpPullBackgroundService: {ex.Message}");
            }

            // Attendre 5 minutes
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
