using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

/// <summary>
/// Application database context for managing weather data.
/// </summary>
/// <param name="options">Options for configuring the database context</param>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Represents the 'WeatherData' table in the database.
    /// </summary>
    public DbSet<WeatherData> WeatherData { get; init; } = default!;
}
