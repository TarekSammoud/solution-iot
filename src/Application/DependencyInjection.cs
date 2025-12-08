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
        services.AddSingleton<LocalisationMapper>();
        services.AddSingleton<SystemePartenaireMapper>();
        services.AddSingleton<UniteMesureMapper>();
        services.AddSingleton<UserMapper>();
        services.AddSingleton<ReleveMapper>();

        services.AddScoped<ILocalisationService, LocalisationService>();
        services.AddScoped<ISystemePartenaireService, SystemePartenaireService>();
        services.AddScoped<IUniteMesureService, UniteMesureService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISondeService, SondeService>();
        services.AddScoped<IReleveService, ReleveService>();

        return services;
    }
}
