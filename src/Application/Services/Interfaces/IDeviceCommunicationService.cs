using Application.DTOs.Device;
using Application.DTOs.Releve;
using Application.DTOs.Sonde;

namespace Application.Services.Interfaces;

public interface IDeviceCommunicationService
{
    Task<ReleveDto?> PullDataFromDevice(SondeDto sonde);
    Task<ReleveDto?> ReceiveDataFromDevice(Guid deviceId, DeviceDataDto data);
    Task<TestCommunicationResultDto> TestConnection(SondeDto sonde);
}
