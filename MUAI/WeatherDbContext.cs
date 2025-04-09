using Microsoft.EntityFrameworkCore;
using MUAI.Models;

namespace MUAI;

public class WeatherDbContext : DbContext
{
    public DbSet<WeatherData> WeatherRecords { get; set; }
    public DbSet<WindData> WindRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=weather.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WindData>()
            .HasOne(w => w.WeatherData)
            .WithOne(w => w.Wind)
            .HasForeignKey<WindData>(w => w.WeatherDataId);
    }
}
