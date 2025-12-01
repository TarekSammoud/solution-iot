using Application.Mappers;
using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Classe d'extension pour l'enregistrement des services de la couche Application.
/// Utilisée pour configurer l'injection de dépendances dans le conteneur IoC.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Enregistre tous les services de la couche Application dans le conteneur de services.
    /// </summary>
    /// <param name="services">La collection de services.</param>
    /// <returns>La collection de services pour permettre le chaînage.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Enregistrer le mapper comme Singleton (pas d'état, thread-safe, code généré)
        services.AddSingleton<LocalisationMapper>();
        services.AddSingleton<UserMapper>();

        // Enregistrer les services comme Scoped (une instance par requête HTTP)
        services.AddScoped<ILocalisationService, LocalisationService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
