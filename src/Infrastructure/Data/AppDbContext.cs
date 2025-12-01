using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Localisation> Localisations { get; set; } = null!;
    public DbSet<SystemePartenaire> SystemesPartenaires { get; set; } = null!;
    public DbSet<UniteMesure> UnitesMesures { get; set; } = null!;
    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<Sonde> Sondes { get; set; } = null!;
    public DbSet<Actionneur> Actionneurs { get; set; } = null!;
    public DbSet<Releve> Releves { get; set; } = null!;
    public DbSet<SeuilAlerte> SeuilsAlerte { get; set; } = null!;
    public DbSet<Alerte> Alertes { get; set; } = null!;
    public DbSet<EtatActionneur> EtatsActionneur { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Application des configurations via classes dédiées
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new LocalisationConfiguration());
        modelBuilder.ApplyConfiguration(new SystemePartenaireConfiguration());
        modelBuilder.ApplyConfiguration(new UniteMesureConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceConfiguration());
        modelBuilder.ApplyConfiguration(new SondeConfiguration());
        modelBuilder.ApplyConfiguration(new ActionneurConfiguration());
        modelBuilder.ApplyConfiguration(new ReleveConfiguration());
        modelBuilder.ApplyConfiguration(new SeuilAlerteConfiguration());
        modelBuilder.ApplyConfiguration(new AlerteConfiguration());
        modelBuilder.ApplyConfiguration(new EtatActionneurConfiguration());
    }
}
