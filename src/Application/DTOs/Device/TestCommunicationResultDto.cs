namespace Application.DTOs.Device;

public class TestCommunicationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal? Value { get; set; }
    public DateTime? Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
}
