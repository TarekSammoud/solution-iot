using Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Tests.Integration.API;

/// <summary>
/// Tests d'intégration pour LocalisationsController.
/// Teste les endpoints API REST complets avec WebApplicationFactory et InMemory database.
/// </summary>
public class LocalisationsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public LocalisationsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region GET /api/localisations

    [Fact]
    public async Task GetAll_DevraitRetourner200_AvecListeLocalisations()
    {
        // Arrange - Créer des localisations de test
        var localisation1 = new CreateLocalisationDto
        {
            Nom = "Salon Test",
            Description = "Pièce de test 1"
        };
        var localisation2 = new CreateLocalisationDto
        {
            Nom = "Cuisine Test",
            Description = "Pièce de test 2"
        };

        await _client.PostAsJsonAsync("/api/localisations", localisation1);
        await _client.PostAsJsonAsync("/api/localisations", localisation2);

        // Act
        var response = await _client.GetAsync("/api/localisations");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var localisations = await response.Content.ReadFromJsonAsync<List<LocalisationDto>>();
        Assert.NotNull(localisations);
        Assert.True(localisations.Count >= 2); // Au moins les 2 qu'on a créées
    }

    [Fact]
    public async Task GetAll_DevraitRetournerListeVide_QuandAucuneLocalisation()
    {
        // Arrange - Utiliser une nouvelle factory avec DB vide
        using var factory = new CustomWebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/localisations");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var localisations = await response.Content.ReadFromJsonAsync<List<LocalisationDto>>();
        Assert.NotNull(localisations);
        Assert.Empty(localisations);
    }

    #endregion

    #region GET /api/localisations/{id}

    [Fact]
    public async Task GetById_DevraitRetourner200_QuandLocalisationExiste()
    {
        // Arrange - Créer une localisation
        var createDto = new CreateLocalisationDto
        {
            Nom = "Bureau Test",
            Description = "Espace de travail"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/localisations", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LocalisationDto>();

        // Act
        var response = await _client.GetAsync($"/api/localisations/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var localisation = await response.Content.ReadFromJsonAsync<LocalisationDto>();
        Assert.NotNull(localisation);
        Assert.Equal(created.Id, localisation.Id);
        Assert.Equal("Bureau Test", localisation.Nom);
        Assert.Equal("Espace de travail", localisation.Description);
    }

    [Fact]
    public async Task GetById_DevraitRetourner404_QuandLocalisationNExistePas()
    {
        // Arrange
        var idInexistant = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/localisations/{idInexistant}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region POST /api/localisations

    [Fact]
    public async Task Create_DevraitRetourner201_AvecLocalisationCreee()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "Garage Test",
            Description = "Parking"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/localisations", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<LocalisationDto>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Garage Test", created.Nom);
        Assert.Equal("Parking", created.Description);
        Assert.NotEqual(default(DateTime), created.DateCreation);

        // Vérifier le header Location
        Assert.NotNull(response.Headers.Location);
        Assert.Contains(created.Id.ToString(), response.Headers.Location.ToString());
    }

    [Fact]
    public async Task Create_DevraitRetourner400_QuandNomManquant()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "", // Nom vide (Required)
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/localisations", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_DevraitRetourner400_QuandNomTropLong()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = new string('A', 201), // Plus de 200 caractères
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/localisations", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_DevraitGenererIdEtDateCreation()
    {
        // Arrange
        var createDto = new CreateLocalisationDto
        {
            Nom = "Terrasse Test",
            Description = "Extérieur"
        };

        var avant = DateTime.UtcNow;

        // Act
        var response = await _client.PostAsJsonAsync("/api/localisations", createDto);

        var apres = DateTime.UtcNow;

        // Assert
        var created = await response.Content.ReadFromJsonAsync<LocalisationDto>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id); // Id généré
        Assert.True(created.DateCreation >= avant && created.DateCreation <= apres); // DateCreation générée
    }

    #endregion

    #region PUT /api/localisations/{id}

    [Fact]
    public async Task Update_DevraitRetourner200_QuandLocalisationMiseAJour()
    {
        // Arrange - Créer une localisation
        var createDto = new CreateLocalisationDto
        {
            Nom = "Chambre Originale",
            Description = "Description originale"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/localisations", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LocalisationDto>();

        var updateDto = new UpdateLocalisationDto
        {
            Id = created!.Id,
            Nom = "Chambre Modifiée",
            Description = "Description modifiée"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/localisations/{created.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<LocalisationDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Chambre Modifiée", updated.Nom);
        Assert.Equal("Description modifiée", updated.Description);
        Assert.Equal(created.DateCreation, updated.DateCreation); // DateCreation préservée
    }

    [Fact]
    public async Task Update_DevraitRetourner404_QuandLocalisationNExistePas()
    {
        // Arrange
        var updateDto = new UpdateLocalisationDto
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/localisations/{updateDto.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_DevraitRetourner400_QuandIdUrlDifferentIdBody()
    {
        // Arrange
        var idUrl = Guid.NewGuid();
        var updateDto = new UpdateLocalisationDto
        {
            Id = Guid.NewGuid(), // ID différent
            Nom = "Test",
            Description = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/localisations/{idUrl}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_DevraitRetourner400_QuandNomManquant()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateLocalisationDto
        {
            Id = id,
            Nom = "", // Nom vide (Required)
            Description = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/localisations/{id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region DELETE /api/localisations/{id}

    [Fact]
    public async Task Delete_DevraitRetourner204_QuandLocalisationSupprimee()
    {
        // Arrange - Créer une localisation
        var createDto = new CreateLocalisationDto
        {
            Nom = "À Supprimer",
            Description = "Test suppression"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/localisations", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LocalisationDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/localisations/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_DevraitRetourner404_QuandLocalisationNExistePas()
    {
        // Arrange
        var idInexistant = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/localisations/{idInexistant}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_DevraitSupprimerEffectivement_LaLocalisation()
    {
        // Arrange - Créer une localisation
        var createDto = new CreateLocalisationDto
        {
            Nom = "À Supprimer Vraiment",
            Description = "Test suppression effective"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/localisations", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LocalisationDto>();

        // Act - Supprimer
        await _client.DeleteAsync($"/api/localisations/{created!.Id}");

        // Assert - Vérifier que GET retourne 404
        var getResponse = await _client.GetAsync($"/api/localisations/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    #endregion
}
