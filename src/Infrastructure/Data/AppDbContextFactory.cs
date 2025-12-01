using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data;

/// <summary>
/// Factory pour créer une instance d'AppDbContext au moment du design (migrations EF Core).
/// Nécessaire pour que dotnet ef migrations fonctionne correctement.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Utilise SQLite avec une connexion par défaut pour les migrations
        optionsBuilder.UseSqlite("Data Source=iot.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
