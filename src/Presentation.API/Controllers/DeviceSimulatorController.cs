using Application.DTOs.Device;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers;

[ApiController]
[Route("api/device-simulator")]
public class DeviceSimulatorController : ControllerBase
{
    private static readonly Random _random = new();

    [HttpGet]
    public ActionResult<DeviceDataDto> GetSimulatedData()
    {
        var data = new DeviceDataDto
        {
            Value = (decimal)(_random.NextDouble() * 30.0 + 10.0), // Random value between 10.0 and 40.0
            Timestamp = DateTime.UtcNow
        };
        return Ok(data);
    }
}
