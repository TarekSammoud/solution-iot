using Application.DTOs.Releve;
using Application.DTOs.Sonde;
using Application.Mappers;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System.Collections.ObjectModel;

namespace Application.Services;

/// <summary>
/// Service de gestion des Releves.
/// Orchestre la logique métier CRUD en utilisant le repository et le mapper.
/// </summary>
public class ReleveService : IReleveService
{
    private readonly IReleveRepository _repository;
    private readonly ISondeRepository _sondeRepository;
    private readonly ISeuilAlerteService _seuilAlerteService;
    private readonly IAlerteService _alerteService;
    private readonly ReleveMapper _mapper;

    /// <summary>
    /// Initialise une nouvelle instance du service de Releve.
    /// </summary>
    /// <param name="repository">Le repository pour accéder aux données.</param>
    /// <param name="mapper">Le mapper pour convertir entre entités et DTOs.</param>
    public ReleveService(
        IReleveRepository repository,
        ReleveMapper mapper,
        ISondeRepository sondeRepository,
        ISeuilAlerteService seuilAlerteService,
        IAlerteService alerteService)
    {
        _repository = repository;
        _mapper = mapper;
        _sondeRepository = sondeRepository;
        _seuilAlerteService = seuilAlerteService;
        _alerteService = alerteService;
    }


    /// <summary>
    /// Récupère une Releve par son identifiant.
    /// </summary>
    public async Task<ReleveDto?> GetByIdAsync(Guid id)
    {
        var Releve = await _repository.GetByIdAsync(id);

        if (Releve == null)
        {
            return null;
        }

        return _mapper.ToDto(Releve);
    }

    /// <summary>
    /// Récupère toutes les Releves.
    /// </summary>
    public async Task<IEnumerable<ReleveDto>> GetAllAsync()
    {
        var Releves = await _repository.GetAllAsync();
        return _mapper.ToDtoList(Releves);
    }
    /// <summary>
    /// Récupère toutes les Releves par filtres.
    /// </summary>
    /// <returns>Une collection de DTOs de toutes les Releves.</returns>
    public async Task<RelevePageDto> GetAllAsync(int page = 0, int limit = int.MaxValue, TypeReleve? type = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var Releves = _mapper.ToDtoList (await _repository.GetAllAsync(page, limit, type, startDate, endDate));
        int total = await _repository.CountRelevesAsync(type,startDate,endDate);
        return new RelevePageDto(page, total, limit, Releves,startDate,endDate,type);
    }


    /// <summary>
    /// Crée une nouvelle Releve.
    /// Id et DateCreation sont générés automatiquement par le repository.
    /// </summary>
    public async Task<ReleveDto> CreateAsync(CreateReleveDto dto)
    {
        // Mapper le DTO vers l'entité
        var Releve = _mapper.ToEntity(dto);

        // Créer l'entité (Id et DateCreation générés par le repository)
        var created = await _repository.AddAsync(Releve);
        await VerifierEtGererAlertes(created.Id);
        if (created!=null)
            created.Sonde = await _sondeRepository.GetByIdAsync(created.SondeId);
        // Retourner le DTO de l'entité créée
        return _mapper.ToDto(created);
    }

    /// <summary>
    /// Met à jour une Releve existante.
    /// Préserve Id et DateCreation (propriétés immutables).
    /// </summary>
    public async Task<ReleveDto?> UpdateAsync(UpdateReleveDto dto)
    {
        // Vérifier que la Releve existe
        var Releve = await _repository.GetByIdAsync(dto.Id);

        if (Releve == null)
        {
            return null;
        }

        // Mettre à jour l'entité existante (préserve Id et DateCreation)
        _mapper.UpdateEntity(dto, Releve);

        // Sauvegarder les modifications
        await _repository.UpdateAsync(Releve);
        Releve.Sonde = await _sondeRepository.GetByIdAsync(Releve.SondeId);
        // Retourner le DTO de l'entité mise à jour
        return _mapper.ToDto(Releve);
    }

    /// <summary>
    /// Supprime une Releve.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Vérifier que la Releve existe
        var exists = await _repository.GetByIdAsync(id);

        if (exists==null)
        {
            return false;
        }

        // Supprimer la Releve
        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<ReleveDto>> GetBySondeAync(Guid id)
    {
        var releves = await _repository.GetBySondeAsync(id);
        return _mapper.ToDtoList(releves);
    }

    public async Task<IEnumerable<ReleveDto>> GetRecentBySondeAync(Guid id, int n)
    {
        var releves = await _repository.GetBySondeAsync(id);
        return _mapper.ToDtoList(releves).OrderByDescending(r=>r.DateHeure).Take(n);
    }

    private async Task VerifierEtGererAlertes(Guid releveId)
    {
        // Relevé
        var releve = await _repository.GetByIdAsync(releveId);
        if (releve == null)
            return;

        var valeur = releve.Valeur;
        var dateReleve = releve.DateHeure;

        // Seuils actifs
        var seuils = await _seuilAlerteService.GetBySondeAsync(releve.SondeId);
        var seuilsActifs = seuils.Where(s => s.EstActif).ToList();

        // Création d’alertes (dépassement)
        foreach (var seuil in seuilsActifs)
        {
            bool depassement = seuil.TypeSeuil switch
            {
                TypeSeuil.Minimum => valeur < seuil.Valeur,
                TypeSeuil.Maximum => valeur > seuil.Valeur,
                _ => false
            };

            if (!depassement)
                continue;

            // Pas de doublon
            var alerteExistante = await _alerteService
                .GetActiveBySondeAndSeuilAsync(releve.SondeId, seuil.Id);

            if (alerteExistante != null)
                continue;

            // Créer l’alerte via le service
            await _alerteService.CreerAlerteAsync(
                releve.SondeId,
                seuil.Id,
                seuil.TypeSeuil,
                seuil.TypeAlerte,
                valeur,
                seuil.Valeur,
                dateReleve);
        }

        // 4️⃣ Résolution automatique des alertes
        await _alerteService.ResoudreAlertesSiNecessaireAsync(
            releve.SondeId,
            valeur,
            dateReleve);
    }


    public async Task<IEnumerable<ReleveDto>> GetBySondeDateRangeAync(Guid sondeId, DateTime startDate, DateTime endDate)
    {
        var releves = await _repository.GetBySondeDateRangeAsync(sondeId, startDate, endDate);
        return _mapper.ToDtoList(releves);
    }

    Task IReleveService.VerifierEtGererAlertes(Guid releveId)
    {
        return VerifierEtGererAlertes(releveId);
    }
}
