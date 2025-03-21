using Domain;

namespace DAL;

/// <summary>
/// Defines methods for accessing and adding weather data in the repository.
/// </summary>
public interface IWeatherRepository
{
    /// <summary>
    /// Retrieves the latest weather data for a specified city.
    /// </summary>
    /// <param name="city">The name of the city for which to fetch the latest weather data.</param>
    /// <returns>
    /// The latest <see cref="WeatherData"/> entry for the given city, or null if no data is found.
    /// </returns>
    Task<WeatherData?> GetLatestWeatherByCityAsync(string city);
    
    /// <summary>
    /// Retrieves the weather data for a specified city and time.
    /// </summary>
    /// <param name="city">The name of the city for which to fetch the weather data.</param>
    /// <param name="time">The specific date and time to match against weather data entries.</param>
    /// <returns>
    /// The latest <see cref="WeatherData"/> entry for the given city, or null if no data is found.
    /// </returns>
    Task<WeatherData?> GetLatestWeatherByCityAndTimeAsync(string city, DateTime time);
    
    /// <summary>
    /// Adds list of weather data entries to the database.
    /// </summary>
    /// <param name="weatherData">A list of weather data entries to be saved.</param>
    /// <returns>A task representing the asynchronou    s operation.</returns>
    Task AddWeatherDataAsync(List<WeatherData> weatherData);
}