using Application.DTOs.Localisation;
using Application.Mappers;
using Application.Services.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

/// <summary>
/// Service de gestion des localisations.
/// Orchestre la logique métier CRUD en utilisant le repository et le mapper.
/// </summary>
public class LocalisationService : ILocalisationService
{
    private readonly ILocalisationRepository _repository;
    private readonly LocalisationMapper _mapper;

    /// <summary>
    /// Initialise une nouvelle instance du service de localisation.
    /// </summary>
    /// <param name="repository">Le repository pour accéder aux données.</param>
    /// <param name="mapper">Le mapper pour convertir entre entités et DTOs.</param>
    public LocalisationService(ILocalisationRepository repository, LocalisationMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Récupère une localisation par son identifiant.
    /// </summary>
    public async Task<LocalisationDto?> GetByIdAsync(Guid id)
    {
        var localisation = await _repository.GetByIdAsync(id);

        if (localisation == null)
        {
            return null;
        }

        return _mapper.ToDto(localisation);
    }

    /// <summary>
    /// Récupère toutes les localisations.
    /// </summary>
    public async Task<IEnumerable<LocalisationDto>> GetAllAsync()
    {
        var localisations = await _repository.GetAllAsync();
        return _mapper.ToDtoList(localisations);
    }

    /// <summary>
    /// Crée une nouvelle localisation.
    /// Id et DateCreation sont générés automatiquement par le repository.
    /// </summary>
    public async Task<LocalisationDto> CreateAsync(CreateLocalisationDto dto)
    {
        // Mapper le DTO vers l'entité
        var localisation = _mapper.ToEntity(dto);

        // Créer l'entité (Id et DateCreation générés par le repository)
        var created = await _repository.AddAsync(localisation);

        // Retourner le DTO de l'entité créée
        return _mapper.ToDto(created);
    }

    /// <summary>
    /// Met à jour une localisation existante.
    /// Préserve Id et DateCreation (propriétés immutables).
    /// </summary>
    public async Task<LocalisationDto?> UpdateAsync(UpdateLocalisationDto dto)
    {
        // Vérifier que la localisation existe
        var localisation = await _repository.GetByIdAsync(dto.Id);

        if (localisation == null)
        {
            return null;
        }

        // Mettre à jour l'entité existante (préserve Id et DateCreation)
        _mapper.UpdateEntity(dto, localisation);

        // Sauvegarder les modifications
        await _repository.UpdateAsync(localisation);

        // Retourner le DTO de l'entité mise à jour
        return _mapper.ToDto(localisation);
    }

    /// <summary>
    /// Supprime une localisation.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Vérifier que la localisation existe
        var exists = await _repository.ExistsAsync(id);

        if (!exists)
        {
            return false;
        }

        // Supprimer la localisation
        await _repository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// Vérifie si une localisation existe.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }
}
