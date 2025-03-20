namespace Services;

/// <summary>
/// Defines a service responsible for fetching and storing weather data.
/// </summary>
public interface IWeatherDataService
{
    /// <summary>
    /// Fetches the latest weather data from an external source and stores it in the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task FetchAndStoreWeatherData();
}