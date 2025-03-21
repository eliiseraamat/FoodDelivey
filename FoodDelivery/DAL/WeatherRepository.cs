using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

/// <summary>
/// Repository for handling weather data operations in the database.
/// </summary>
/// <param name="context">The application database context.</param>
public class WeatherRepository(AppDbContext context) : IWeatherRepository
{
    /// <summary>
    /// Retrieves the latest weather data for a specified city.
    /// </summary>
    /// <param name="city">The name of the city to fetch weather data for.</param>
    /// <returns>
    /// The latest <see cref="WeatherData"/> entry for the given city, or null if no data is found.
    /// </returns>
    public async Task<WeatherData?> GetLatestWeatherByCityAsync(string city)
    {
        return await context.WeatherData
            .Where(w => w.StationName.ToLower().Contains(city.ToLower()))
            .OrderByDescending(w => w.Time)
            .FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Adds list of weather data entries to the database.
    /// </summary>
    /// <param name="weatherData">A list of weather data entries to be saved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddWeatherDataAsync(List<WeatherData> weatherData)
    {
        await context.WeatherData.AddRangeAsync(weatherData);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves the latest weather data for a specified city and date-time.
    /// </summary>
    /// <param name="city">The name of the city for which to fetch the weather data.</param>
    /// <param name="time">The specific date and time to match against weather data entries.</param>
    /// <returns>
    /// The latest <see cref="WeatherData"/> entry for the given city, or null if no data is found.
    /// </returns>
    public async Task<WeatherData?> GetLatestWeatherByCityAndTimeAsync(string city, DateTime time)
    {
        var weatherData = await context.WeatherData
            .Where(w => w.StationName.ToLower().Contains(city.ToLower()) && w.Time.Date <= time.Date && w.Time <= time)
            .ToListAsync();
        
        if (weatherData.Count != 0)
        {
            return weatherData
                .OrderBy(w => Math.Abs((w.Time - time).TotalMinutes))
                .FirstOrDefault();
        }
        return null; 
    }
}
