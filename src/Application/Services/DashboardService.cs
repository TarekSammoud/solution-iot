using Application.DTOs.Dashboard;
using Application.Services.Interfaces;
using Domain.Enums;

namespace Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAlerteService _alerteService;
    private readonly IReleveService _releveService;
    private readonly ISondeService _sondeService;
    private readonly IActionneurService _actionneurService;
    private readonly IEtatActionneurService _etatActionneurService;

    public DashboardService(
        IAlerteService alerteService,
        IReleveService releveService,
        ISondeService sondeService,
        IActionneurService actionneurService,
        IEtatActionneurService etatActionneurService)
    {
        _alerteService = alerteService;
        _releveService = releveService;
        _sondeService = sondeService;
        _actionneurService = actionneurService;
        _etatActionneurService = etatActionneurService;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var summary = new DashboardSummaryDto();

        // 1. Get all sondes and calculate statistics
        var sondes = await _sondeService.GetAllAsync();
        var sondesList = sondes.ToList();
        
        summary.Statistiques.TotalSondes = sondesList.Count;
        summary.Statistiques.SondesActives = sondesList.Count(s => s.EstActif);
        summary.Statistiques.SondesInactives = sondesList.Count(s => !s.EstActif);

        // 2. Get all actionneurs and calculate statistics
        var actionneurs = await _actionneurService.GetAllAsync();
        var actionneursList = actionneurs.ToList();
        
        summary.Statistiques.TotalActionneurs = actionneursList.Count;
        summary.Statistiques.ActionneursActifs = actionneursList.Count(a => a.EtatActuel?.EstActif == true);
        summary.Statistiques.ActionneursInactifs = actionneursList.Count(a => a.EtatActuel?.EstActif != true);

        // 3. Get active actionneurs with their current state
        var activeActionneurs = actionneursList.Where(a => a.EtatActuel?.EstActif == true).ToList();
        summary.ActionneursActifs = activeActionneurs;

        // 4. Get all alertes and calculate statistics
        var allAlertes = await _alerteService.GetDashboardAsync();
        var alertesList = allAlertes.ToList();
        
        summary.Statistiques.AlertesActives = alertesList.Count(a => a.Statut == StatutAlerte.Active);
        summary.Statistiques.AlertesAcquittees = alertesList.Count(a => a.Statut == StatutAlerte.Acquittee);
        summary.Statistiques.AlertesResoluesAujourdHui = alertesList.Count(a => 
            a.Statut == StatutAlerte.Resolue && 
            a.DateResolution.HasValue && 
            a.DateResolution.Value.Date == DateTime.Today);

        // 5. Get active alertes (limited to 10 most recent)
        summary.AlertesActives = alertesList
            .Where(a => a.Statut == StatutAlerte.Active)
            .OrderByDescending(a => a.DateCreation)
            .Take(10)
            .ToList();

        // 6. Get recent relevés (limited to 20 most recent)
        var allReleves = await _releveService.GetAllAsync(0, 20, null, null, null);
        summary.DerniersReleves = allReleves.releves
            .OrderByDescending(r => r.DateHeure)
            .Take(20)
            .ToList();

        // 7. Count relevés for today
        var todayReleves = await _releveService.GetAllAsync(0, int.MaxValue, null, DateTime.Today, DateTime.Today.AddDays(1).AddTicks(-1));
        summary.Statistiques.RelevesAujourdHui = todayReleves.total;

        return summary;
    }
}