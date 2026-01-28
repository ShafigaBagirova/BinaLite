using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;

namespace Persistence.Context;

public class BinaLiteDbContext: DbContext
{
    public BinaLiteDbContext(DbContextOptions<BinaLiteDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PropertyAdConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyMediaConfiguration());
    }
    DbSet<PropertyAd> PropertyAds { get; set; }
    DbSet<PropertyMedia> PropertyMedias { get; set; }

    
}
