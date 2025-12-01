using Application.DTOs;
using Application.Mappers;
using Domain.Entities;
using Xunit;

namespace Tests.Unit.Mappers;

/// <summary>
/// Tests unitaires pour LocalisationMapper.
/// Vérifie tous les mappings entre l'entité Localisation et ses DTOs.
/// </summary>
public class LocalisationMapperTests
{
    private readonly LocalisationMapper _mapper;

    public LocalisationMapperTests()
    {
        _mapper = new LocalisationMapper();
    }

    [Fact]
    public void ToDto_DevraitMapperToutesLesProprietes()
    {
        // Arrange
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Salon",
            Description = "Pièce principale",
            DateCreation = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(localisation);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(localisation.Id, dto.Id);
        Assert.Equal(localisation.Nom, dto.Nom);
        Assert.Equal(localisation.Description, dto.Description);
        Assert.Equal(localisation.DateCreation, dto.DateCreation);
    }

    [Fact]
    public void ToDto_AvecDescriptionNull_DevraitMapperCorrectement()
    {
        // Arrange
        var localisation = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Cuisine",
            Description = null,
            DateCreation = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(localisation);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(localisation.Id, dto.Id);
        Assert.Equal(localisation.Nom, dto.Nom);
        Assert.Null(dto.Description);
        Assert.Equal(localisation.DateCreation, dto.DateCreation);
    }

    [Fact]
    public void ToDtoList_DevraitMapperListeLocalisations()
    {
        // Arrange
        var localisations = new List<Localisation>
        {
            new Localisation
            {
                Id = Guid.NewGuid(),
                Nom = "Salon",
                Description = "Pièce 1",
                DateCreation = DateTime.UtcNow
            },
            new Localisation
            {
                Id = Guid.NewGuid(),
                Nom = "Cuisine",
                Description = "Pièce 2",
                DateCreation = DateTime.UtcNow
            },
            new Localisation
            {
                Id = Guid.NewGuid(),
                Nom = "Chambre",
                Description = null,
                DateCreation = DateTime.UtcNow
            }
        };

        // Act
        var dtos = _mapper.ToDtoList(localisations).ToList();

        // Assert
        Assert.NotNull(dtos);
        Assert.Equal(3, dtos.Count);
        Assert.Equal(localisations[0].Nom, dtos[0].Nom);
        Assert.Equal(localisations[1].Nom, dtos[1].Nom);
        Assert.Equal(localisations[2].Nom, dtos[2].Nom);
        Assert.Null(dtos[2].Description);
    }

    [Fact]
    public void ToDtoList_ListeVide_DevraitRetournerListeVide()
    {
        // Arrange
        var localisations = new List<Localisation>();

        // Act
        var dtos = _mapper.ToDtoList(localisations).ToList();

        // Assert
        Assert.NotNull(dtos);
        Assert.Empty(dtos);
    }

    [Fact]
    public void ToEntity_AvecCreateDto_DevraitMapperNomEtDescription()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "Bureau",
            Description = "Espace de travail"
        };

        // Act
        var entity = _mapper.ToEntity(createDto);

        // Assert
        Assert.NotNull(entity);
        Assert.Equal(createDto.Nom, entity.Nom);
        Assert.Equal(createDto.Description, entity.Description);
        // Id et DateCreation ne sont pas mappés (valeurs par défaut)
    }

    [Fact]
    public void ToEntity_AvecUpdateDto_DevraitMapperIdNomDescription()
    {
        // Arrange
        var updateDto = new UpdateLocalisationDto
        {
            Id = Guid.NewGuid(),
            Nom = "Garage Modifié",
            Description = "Description mise à jour"
        };

        // Act
        var entity = _mapper.ToEntity(updateDto);

        // Assert
        Assert.NotNull(entity);
        Assert.Equal(updateDto.Id, entity.Id);
        Assert.Equal(updateDto.Nom, entity.Nom);
        Assert.Equal(updateDto.Description, entity.Description);
        // DateCreation n'est pas mappé (valeur par défaut)
    }

    [Fact]
    public void UpdateEntity_DevraitMettreAJourProprietesModifiables()
    {
        // Arrange
        var existingEntity = new Localisation
        {
            Id = Guid.NewGuid(),
            Nom = "Ancien Nom",
            Description = "Ancienne Description",
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var updateDto = new UpdateLocalisationDto
        {
            Id = existingEntity.Id,
            Nom = "Nouveau Nom",
            Description = "Nouvelle Description"
        };

        // Act
        _mapper.UpdateEntity(updateDto, existingEntity);

        // Assert
        Assert.Equal(updateDto.Nom, existingEntity.Nom);
        Assert.Equal(updateDto.Description, existingEntity.Description);
    }

    [Fact]
    public void UpdateEntity_DevraitPreserverIdEtDateCreation()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var originalDateCreation = DateTime.UtcNow.AddDays(-5);

        var existingEntity = new Localisation
        {
            Id = originalId,
            Nom = "Ancien Nom",
            Description = "Ancienne Description",
            DateCreation = originalDateCreation
        };

        var updateDto = new UpdateLocalisationDto
        {
            Id = Guid.NewGuid(), // Id différent dans le DTO
            Nom = "Nouveau Nom",
            Description = "Nouvelle Description"
        };

        // Act
        _mapper.UpdateEntity(updateDto, existingEntity);

        // Assert
        Assert.Equal(originalId, existingEntity.Id); // Id préservé
        Assert.Equal(originalDateCreation, existingEntity.DateCreation); // DateCreation préservée
        Assert.Equal(updateDto.Nom, existingEntity.Nom); // Nom mis à jour
        Assert.Equal(updateDto.Description, existingEntity.Description); // Description mise à jour
    }

    [Fact]
    public void ToEntity_AvecCreateDto_DevraitLaisserIdEtDateCreationParDefaut()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "Salle de Bain",
            Description = "Description"
        };

        // Act
        var entity = _mapper.ToEntity(createDto);

        // Assert
        Assert.NotNull(entity);
        Assert.Equal(Guid.Empty, entity.Id); // Id par défaut (sera généré par repository)
        Assert.Equal(default(DateTime), entity.DateCreation); // DateTime.MinValue (sera généré par repository)
        Assert.Equal(createDto.Nom, entity.Nom);
        Assert.Equal(createDto.Description, entity.Description);
    }

    [Fact]
    public void ToEntity_AvecUpdateDto_DevraitMapperIdMaisNePasMapperDateCreation()
    {
        // Arrange
        var updateDto = new UpdateLocalisationDto
        {
            Id = Guid.NewGuid(),
            Nom = "Terrasse",
            Description = "Espace extérieur"
        };

        // Act
        var entity = _mapper.ToEntity(updateDto);

        // Assert
        Assert.NotNull(entity);
        Assert.Equal(updateDto.Id, entity.Id); // Id mappé depuis UpdateDto
        Assert.Equal(default(DateTime), entity.DateCreation); // DateCreation non mappé (absent du UpdateDto)
        Assert.Equal(updateDto.Nom, entity.Nom);
        Assert.Equal(updateDto.Description, entity.Description);
    }
}
