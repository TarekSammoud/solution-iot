using Application;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données SQLite (sauf en mode Testing où CustomWebApplicationFactory le configure)
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Enregistrement des repositories (Infrastructure)
builder.Services.AddScoped<ILocalisationRepository, LocalisationRepository>();
builder.Services.AddScoped<IActionneurRepository, ActionneurRepository>();
builder.Services.AddScoped<IEtatActionneurRepository, EtatActionneurRepository>();
builder.Services.AddScoped<ISystemePartenaireRepository, SystemePartenaireRepository>();
builder.Services.AddScoped<IUniteMesureRepository, UniteMesureRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISondeRepository, SondeRepository>();

// Enregistrement des services (Application)
builder.Services.AddApplicationServices();

// Configuration CORS pour Blazor Client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7002", "http://localhost:5002")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuration des contrôleurs et Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IoT API", Version = "v1" });
});

var app = builder.Build();

// Initialisation de la base de données (sauf en mode Testing pour les tests d'intégration)
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            await DbInitializer.InitializeAsync(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Une erreur est survenue lors de l'initialisation de la base de données.");
        }
    }
}

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthorization();

app.MapControllers();

app.Run();

// Rendre Program accessible pour les tests d'intégration
public partial class Program { }
