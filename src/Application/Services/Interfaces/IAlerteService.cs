using Application.DTOs.Alertes;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Interfaces;

public interface IAlerteService
{
    // ---------- INTERNAL (used by ReleveService) ----------

    Task<Alerte?> GetActiveBySondeAndSeuilAsync(Guid sondeId, Guid seuilId);

    Task<IEnumerable<Alerte>> GetActiveBySondeAsync(Guid sondeId);

    Task CreerAlerteAsync(
        Guid sondeId,
        Guid seuilId,
        TypeSeuil typeSeuil,
        TypeAlerte typeAlerte,
        decimal valeurMesuree,
        decimal valeurSeuil,
        DateTime dateReleve);

    Task ResoudreAlertesSiNecessaireAsync(
        Guid sondeId,
        decimal valeurMesuree,
        DateTime dateReleve);

    // ---------- API / UI (DTO based) ----------

    Task<IEnumerable<AlerteDto>> GetDashboardAsync();

    Task<IEnumerable<AlerteDto>> GetBySondeAsync(
        Guid sondeId,
        StatutAlerte? statut,
        TypeSeuil? typeSeuil);

    Task<AlerteDetailsDto?> GetDetailsAsync(Guid id);

    Task AcquitterAsync(Guid alerteId, string? commentaire);

    Task ResoudreAsync(Guid alerteId, string? commentaire = null);
}
