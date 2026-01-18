using Application.DTOs.Device;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers;

[ApiController]
[Route("api/webhook")]
public class DeviceWebhookController : ControllerBase
{
    private readonly IDeviceCommunicationService _deviceCommunicationService;
    private readonly ILogger<DeviceWebhookController> _logger;

    public DeviceWebhookController(
        IDeviceCommunicationService deviceCommunicationService,
        ILogger<DeviceWebhookController> logger)
    {
        _deviceCommunicationService = deviceCommunicationService;
        _logger = logger;
    }

    [HttpPost("device/{deviceId}")]
    public async Task<IActionResult> ReceiveDeviceData(Guid deviceId, [FromBody] DeviceDataDto data)
    {
        try
        {
            var result = await _deviceCommunicationService.ReceiveDataFromDevice(deviceId, data);

            if (result == null)
            {
                return Ok("Data received"); 
            }

            return Created("", result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erreur webhook device {deviceId}: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}
