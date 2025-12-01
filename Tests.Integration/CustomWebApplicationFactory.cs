using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration;

/// <summary>
/// Factory personnalisée pour créer une instance de l'application web pour les tests d'intégration.
/// Configure une base de données InMemory isolée pour chaque session de tests.
/// </summary>
/// <typeparam name="TProgram">Le type Program de l'application à tester.</typeparam>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Définir l'environnement Testing en premier pour que Program.cs saute la configuration SQLite
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Utiliser InMemoryDatabase pour les tests avec un nom unique par instance de factory
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
}
