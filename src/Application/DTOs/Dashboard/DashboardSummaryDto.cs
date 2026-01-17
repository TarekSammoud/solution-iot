using Application.DTOs.Actionneur;
using Application.DTOs.Alertes;
using Application.DTOs.Releve;

namespace Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public StatistiquesDto Statistiques { get; set; } = new();
    
    public List<AlerteDto> AlertesActives { get; set; } = new();
    
    public List<ReleveDto> DerniersReleves { get; set; } = new();
    
    public List<ActionneurDto> ActionneursActifs { get; set; } = new();
}