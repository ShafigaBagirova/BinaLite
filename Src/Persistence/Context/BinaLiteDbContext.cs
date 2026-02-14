using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;

namespace Persistence.Context;

public class BinaLiteDbContext: IdentityDbContext<User>
{
    public BinaLiteDbContext(DbContextOptions<BinaLiteDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BinaLiteDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<PropertyAd> PropertyAds { get; set; }
    public DbSet<PropertyMedia> PropertyMedias { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<AppFile> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

}
