using Application.DTOs.Dashboard;

namespace Application.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}