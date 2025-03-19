using Domain;

namespace DAL;

public interface IWeatherRepository
{
    Task<WeatherData?> GetLatestWeatherByCityAsync(string city);
    Task AddWeatherDataAsync(List<WeatherData> weatherData);
}