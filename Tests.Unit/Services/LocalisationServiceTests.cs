using Application.DTOs;
using Application.Mappers;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour LocalisationService.
/// Utilise Moq pour mocker le repository, et une instance réelle du mapper.
/// </summary>
public class LocalisationServiceTests
{
    private readonly Mock<ILocalisationRepository> _mockRepository;
    private readonly LocalisationMapper _mapper;
    private readonly LocalisationService _service;

    public LocalisationServiceTests()
    {
        _mockRepository = new Mock<ILocalisationRepository>();
        _mapper = new LocalisationMapper();
        _service = new LocalisationService(_mockRepository.Object, _mapper);
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerDto_QuandLocalisationExiste()
    {
        // Arrange
        var id = Guid.NewGuid();
        var localisation = new Localisation
        {
            Id = id,
            Nom = "Salon",
            Description = "Pièce principale",
            DateCreation = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(localisation);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Salon", result.Nom);
        Assert.Equal("Pièce principale", result.Description);
        Assert.Equal(localisation.DateCreation, result.DateCreation);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitRetournerNull_QuandLocalisationNExistePas()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Localisation?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_DevraitAppelerRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Localisation?)null);

        // Act
        await _service.GetByIdAsync(id);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_DevraitRetournerListeDtos_QuandLocalisationsExistent()
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

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(localisations);

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Salon", result[0].Nom);
        Assert.Equal("Cuisine", result[1].Nom);
        Assert.Equal("Chambre", result[2].Nom);
        Assert.Null(result[2].Description);
    }

    [Fact]
    public async Task GetAllAsync_DevraitRetournerListeVide_QuandAucuneLocalisation()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Localisation>());

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_DevraitAppelerRepository()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Localisation>());

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_DevraitCreerLocalisation_EtRetournerDto()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "Bureau",
            Description = "Espace de travail"
        };

        var createdId = Guid.NewGuid();
        var createdDate = DateTime.UtcNow;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Localisation>()))
            .ReturnsAsync((Localisation l) =>
            {
                l.Id = createdId;
                l.DateCreation = createdDate;
                return l;
            });

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdId, result.Id);
        Assert.Equal("Bureau", result.Nom);
        Assert.Equal("Espace de travail", result.Description);
        Assert.Equal(createdDate, result.DateCreation);
    }

    [Fact]
    public async Task CreateAsync_DevraitAppelerRepositoryAddAsync()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "Garage",
            Description = "Parking"
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Localisation>()))
            .ReturnsAsync((Localisation l) =>
            {
                l.Id = Guid.NewGuid();
                l.DateCreation = DateTime.UtcNow;
                return l;
            });

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.Is<Localisation>(l =>
            l.Nom == "Garage" &&
            l.Description == "Parking"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DevraitMapperDtoVersEntity()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "Terrasse",
            Description = "Extérieur"
        };

        Localisation? capturedEntity = null;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Localisation>()))
            .Callback<Localisation>(l => capturedEntity = l)
            .ReturnsAsync((Localisation l) =>
            {
                l.Id = Guid.NewGuid();
                l.DateCreation = DateTime.UtcNow;
                return l;
            });

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(capturedEntity);
        Assert.Equal("Terrasse", capturedEntity.Nom);
        Assert.Equal("Extérieur", capturedEntity.Description);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_DevraitMettreAJourLocalisation_QuandExiste()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingLocalisation = new Localisation
        {
            Id = id,
            Nom = "Ancien Nom",
            Description = "Ancienne Description",
            DateCreation = DateTime.UtcNow.AddDays(-10)
        };

        var updateDto = new UpdateLocalisationDto
        {
            Id = id,
            Nom = "Nouveau Nom",
            Description = "Nouvelle Description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingLocalisation);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Localisation>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Nouveau Nom", result.Nom);
        Assert.Equal("Nouvelle Description", result.Description);
    }

    [Fact]
    public async Task UpdateAsync_DevraitRetournerNull_QuandNExistePas()
    {
        // Arrange
        var updateDto = new UpdateLocalisationDto
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync((Localisation?)null);

        // Act
        var result = await _service.UpdateAsync(updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_DevraitAppelerRepositoryUpdateAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingLocalisation = new Localisation
        {
            Id = id,
            Nom = "Ancien",
            Description = "Ancien",
            DateCreation = DateTime.UtcNow
        };

        var updateDto = new UpdateLocalisationDto
        {
            Id = id,
            Nom = "Nouveau",
            Description = "Nouveau"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingLocalisation);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Localisation>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Localisation>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_DevraitUtiliserMapperUpdateEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var originalDateCreation = DateTime.UtcNow.AddDays(-5);
        var existingLocalisation = new Localisation
        {
            Id = id,
            Nom = "Ancien",
            Description = "Ancien",
            DateCreation = originalDateCreation
        };

        var updateDto = new UpdateLocalisationDto
        {
            Id = id,
            Nom = "Modifié",
            Description = "Modifié"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingLocalisation);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Localisation>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(updateDto);

        // Assert
        Assert.Equal("Modifié", existingLocalisation.Nom);
        Assert.Equal("Modifié", existingLocalisation.Description);
    }

    [Fact]
    public async Task UpdateAsync_DevraitPreserverIdEtDateCreation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var originalDateCreation = DateTime.UtcNow.AddDays(-5);
        var existingLocalisation = new Localisation
        {
            Id = id,
            Nom = "Ancien",
            Description = "Ancien",
            DateCreation = originalDateCreation
        };

        var updateDto = new UpdateLocalisationDto
        {
            Id = id,
            Nom = "Modifié",
            Description = "Modifié"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(existingLocalisation);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Localisation>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id); // Id préservé
        Assert.Equal(originalDateCreation, result.DateCreation); // DateCreation préservée
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_DevraitRetournerTrue_QuandLocalisationExiste()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(id))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.DeleteAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_DevraitRetournerFalse_QuandNExistePas()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(id))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_DevraitAppelerRepositoryDeleteAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(id))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.DeleteAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_DevraitRetournerTrue_QuandLocalisationExiste()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ExistsAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_DevraitRetournerFalse_QuandNExistePas()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.ExistsAsync(id))
            .ReturnsAsync(false);

        // Act
        var result = await _service.ExistsAsync(id);

        // Assert
        Assert.False(result);
    }

    #endregion
}
