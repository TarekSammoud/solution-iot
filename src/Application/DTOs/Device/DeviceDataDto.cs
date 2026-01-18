using System.Text.Json.Serialization;

namespace Application.DTOs.Device;

public class DeviceDataDto
{
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }
}
