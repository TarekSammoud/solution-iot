using Application.DTOs.Alertes;
using Application.DTOs.SeuilAlerte;
using Application.DTOs.Sonde;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace IotPlatform.Application.Services;

public class AlerteService : IAlerteService
{
    private readonly IAlerteRepository _alerteRepository;
    private readonly ISondeRepository _sondeRepository;
    private readonly ISeuilAlerteRepository _seuilRepository;

    public AlerteService(
        IAlerteRepository alerteRepository,
        ISondeRepository sondeRepository,
        ISeuilAlerteRepository seuilRepository)
    {
        _alerteRepository = alerteRepository;
        _sondeRepository = sondeRepository;
        _seuilRepository = seuilRepository;
    }

    // ======================================================
    // INTERNAL LOGIC (used by ReleveService)
    // ======================================================

    public async Task<Alerte?> GetActiveBySondeAndSeuilAsync(Guid sondeId, Guid seuilId)
    {
        var alertes = await _alerteRepository.GetBySondeAsync(sondeId);

        return alertes.FirstOrDefault(a =>
            a.SeuilAlerteId == seuilId &&
            a.Statut == StatutAlerte.Active);
    }

    public async Task<IEnumerable<Alerte>> GetActiveBySondeAsync(Guid sondeId)
    {
        var alertes = await _alerteRepository.GetBySondeAsync(sondeId);
        return alertes.Where(a => a.Statut == StatutAlerte.Active);
    }

    public async Task CreerAlerteAsync(
        Guid sondeId,
        Guid seuilId,
        TypeSeuil typeSeuil,
        TypeAlerte typeAlerte,
        decimal valeurMesuree,
        decimal valeurSeuil,
        DateTime dateReleve)
    {
        var alerte = new Alerte
        {
            Id = Guid.NewGuid(),
            SondeId = sondeId,
            SeuilAlerteId = seuilId,
            TypeSeuil = typeSeuil,
            TypeAlerte = typeAlerte,
            Statut = StatutAlerte.Active,
            DateCreation = DateTime.UtcNow,
            Message =
                $"Valeur {valeurMesuree} détectée le {dateReleve:dd/MM/yyyy HH:mm} " +
                $"{(typeSeuil == TypeSeuil.Minimum ? "en dessous" : "au-dessus")} " +
                $"du seuil ({valeurSeuil})"
        };

        await _alerteRepository.AddAsync(alerte);
    }

    public async Task ResoudreAlertesSiNecessaireAsync(
        Guid sondeId,
        decimal valeurMesuree,
        DateTime dateReleve)
    {
        var alertesActives = await GetActiveBySondeAsync(sondeId);

        foreach (var alerte in alertesActives)
        {
            var seuil = await _seuilRepository.GetByIdAsync(alerte.SeuilAlerteId);
            if (seuil == null)
                continue;

            bool resolue = alerte.TypeSeuil switch
            {
                TypeSeuil.Minimum => valeurMesuree >= seuil.Valeur,
                TypeSeuil.Maximum => valeurMesuree <= seuil.Valeur,
                _ => false
            };

            if (!resolue)
                continue;

            alerte.Statut = StatutAlerte.Resolue;
            alerte.DateResolution = DateTime.UtcNow;
            alerte.Message +=
                $" - Résolu automatiquement par relevé du {dateReleve:dd/MM/yyyy HH:mm}";

            await _alerteRepository.UpdateAsync(alerte);
        }
    }

    // ======================================================
    // API / UI METHODS (DTO)
    // ======================================================

    public async Task<IEnumerable<AlerteDto>> GetDashboardAsync()
    {
        var alertes = await _alerteRepository.GetAllAsync();

        return alertes
            .OrderByDescending(a => a.DateCreation)
            .Select(MapToDto);
    }

    public async Task<IEnumerable<AlerteDto>> GetBySondeAsync(
        Guid sondeId,
        StatutAlerte? statut,
        TypeSeuil? typeSeuil)
    {
        var alertes = await _alerteRepository.GetBySondeAsync(sondeId);

        if (statut.HasValue)
            alertes = alertes.Where(a => a.Statut == statut.Value);

        if (typeSeuil.HasValue)
            alertes = alertes.Where(a => a.TypeSeuil == typeSeuil.Value);

        return alertes.Select(MapToDto);
    }

    public async Task<AlerteDetailsDto?> GetDetailsAsync(Guid id)
    {
        var alerte = await _alerteRepository.GetByIdAsync(id);
        if (alerte == null)
            return null;

        return new AlerteDetailsDto
        {
            Id = alerte.Id,
            Statut = alerte.Statut,
            TypeSeuil = alerte.TypeSeuil,
            TypeAlerte = alerte.TypeAlerte,
            DateCreation = alerte.DateCreation,
            DateAcquittement = alerte.DateAcquittement,
            DateResolution = alerte.DateResolution,
            Message = alerte.Message,
            Sonde = new SondeDto { Id = alerte.SondeId },
            SeuilAlerte = new SeuilAlerteDto
            {
                Id = alerte.SeuilAlerteId,
                TypeSeuil = alerte.TypeSeuil,
                TypeAlerte = alerte.TypeAlerte
            }
        };
    }

    public async Task AcquitterAsync(Guid alerteId, string? commentaire)
    {
        var alerte = await _alerteRepository.GetByIdAsync(alerteId);
        if (alerte == null || alerte.Statut != StatutAlerte.Active)
            return;

        alerte.Statut = StatutAlerte.Acquittee;
        alerte.DateAcquittement = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(commentaire))
            alerte.Message += $" - {commentaire}";

        await _alerteRepository.UpdateAsync(alerte);
    }

    public async Task ResoudreAsync(Guid alerteId, string? commentaire = null)
    {
        var alerte = await _alerteRepository.GetByIdAsync(alerteId);
        if (alerte == null)
            return;

        alerte.Statut = StatutAlerte.Resolue;
        alerte.DateResolution = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(commentaire))
            alerte.Message += $" - {commentaire}";

        await _alerteRepository.UpdateAsync(alerte);
    }

    // ======================================================
    // PRIVATE MAPPER
    // ======================================================

    private static AlerteDto MapToDto(Alerte a) => new()
    {
        Id = a.Id,
        SondeId = a.SondeId,
        TypeSeuil = a.TypeSeuil,
        TypeAlerte = a.TypeAlerte,
        Statut = a.Statut,
        DateCreation = a.DateCreation,
        DateResolution = a.DateResolution,
        Message = a.Message
    };
}
