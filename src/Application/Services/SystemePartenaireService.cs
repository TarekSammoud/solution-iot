using Application.DTOs;
using Application.Mappers;
using Application.Services.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

/// <summary>
/// Service de gestion des SystemePartenaires.
/// Orchestre la logique métier CRUD en utilisant le repository et le mapper.
/// </summary>
public class SystemePartenaireService : ISystemePartenaireService
{
    private readonly ISystemePartenaireRepository _repository;
    private readonly SystemePartenaireMapper _mapper;

    /// <summary>
    /// Initialise une nouvelle instance du service de SystemePartenaire.
    /// </summary>
    /// <param name="repository">Le repository pour accéder aux données.</param>
    /// <param name="mapper">Le mapper pour convertir entre entités et DTOs.</param>
    public SystemePartenaireService(ISystemePartenaireRepository repository, SystemePartenaireMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Récupère une SystemePartenaire par son identifiant.
    /// </summary>
    public async Task<SystemePartenaireDto?> GetByIdAsync(Guid id)
    {
        var SystemePartenaire = await _repository.GetByIdAsync(id);

        if (SystemePartenaire == null)
        {
            return null;
        }

        return _mapper.ToDto(SystemePartenaire);
    }

    /// <summary>
    /// Récupère toutes les SystemePartenaires.
    /// </summary>
    public async Task<IEnumerable<SystemePartenaireDto>> GetAllAsync()
    {
        var SystemePartenaires = await _repository.GetAllAsync();
        return _mapper.ToDtoList(SystemePartenaires);
    }

    /// <summary>
    /// Crée une nouvelle SystemePartenaire.
    /// Id et DateCreation sont générés automatiquement par le repository.
    /// </summary>
    public async Task<SystemePartenaireDto> CreateAsync(CreateSystemePartenaireDto dto)
    {
        // Mapper le DTO vers l'entité
        var SystemePartenaire = _mapper.ToEntity(dto);

        // Créer l'entité (Id et DateCreation générés par le repository)
        var created = await _repository.AddAsync(SystemePartenaire);

        // Retourner le DTO de l'entité créée
        return _mapper.ToDto(created);
    }

    /// <summary>
    /// Met à jour une SystemePartenaire existante.
    /// Préserve Id et DateCreation (propriétés immutables).
    /// </summary>
    public async Task<SystemePartenaireDto?> UpdateAsync(UpdateSystemePartenaireDto dto)
    {
        // Vérifier que la SystemePartenaire existe
        var SystemePartenaire = await _repository.GetByIdAsync(dto.Id);

        if (SystemePartenaire == null)
        {
            return null;
        }

        // Mettre à jour l'entité existante (préserve Id et DateCreation)
        _mapper.UpdateEntity(dto, SystemePartenaire);

        // Sauvegarder les modifications
        await _repository.UpdateAsync(SystemePartenaire);

        // Retourner le DTO de l'entité mise à jour
        return _mapper.ToDto(SystemePartenaire);
    }

    /// <summary>
    /// Supprime une SystemePartenaire.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Vérifier que la SystemePartenaire existe
        var exists = await _repository.ExistsAsync(id);

        if (!exists)
        {
            return false;
        }

        // Supprimer la SystemePartenaire
        await _repository.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// Vérifie si une SystemePartenaire existe.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }
}
