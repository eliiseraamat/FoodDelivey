using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<WeatherData> WeatherData { get; init; } = default!;
}
