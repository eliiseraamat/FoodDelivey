using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveyTests;

/// <summary>
/// Unit tests for the WeatherRepository class.
/// </summary>
public class WeatherRepositoryTests
{
    private async Task<AppDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var databaseContext = new AppDbContext(options);
        await databaseContext.Database.EnsureCreatedAsync();
        return databaseContext;
    }

    /// <summary>
    /// Tests that when a city exists, the latest weather data is returned.
    /// </summary>
    [Fact]
    public async Task GetLatestWeatherByCity_CityExists_ReturnsLatestWeatherData()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new WeatherRepository(dbContext);

        var weatherDataList = new List<WeatherData>
        {
            new() { StationName = "Tallinn-Harku", WMOCode = "1244", Temperature = 5, WindSpeed = 3, Phenomenon = "Clear", Time = DateTime.UtcNow.AddHours(-2) },
            new() { StationName = "Tallinn-Harku", WMOCode = "1345", Temperature = 7, WindSpeed = 5, Phenomenon = "Rain", Time = DateTime.UtcNow } // Uuem andmerida
        };

        await dbContext.WeatherData.AddRangeAsync(weatherDataList);
        await dbContext.SaveChangesAsync();
        
        var result = await repository.GetLatestWeatherByCity("Tallinn");
        
        Assert.NotNull(result);
        Assert.Equal("1345", result.WMOCode);
    }

    /// <summary>
    /// Tests that when a city does not exist, null is returned.
    /// </summary>
    [Fact]
    public async Task GetLatestWeatherByCity_CityDoesNotExist_ReturnsNull()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new WeatherRepository(dbContext);
        
        var result = await repository.GetLatestWeatherByCity("Tartu");
        
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that when a city does not exist, null is returned.
    /// </summary>
    [Fact]
    public async Task AddWeatherData_ValidData_SavesToDatabase()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new WeatherRepository(dbContext);

        var weatherData = new List<WeatherData>
        {
            new() { StationName = "Tartu-Tõravere", WMOCode = "1244", Temperature = 10, WindSpeed = 2, Phenomenon = "Cloudy", Time = DateTime.UtcNow }
        };
        
        await repository.AddWeatherData(weatherData);
        
        var result = await dbContext.WeatherData.FirstOrDefaultAsync(w => w.StationName == "Tartu-Tõravere");
        Assert.NotNull(result);
        Assert.Equal("1244", result.WMOCode);
    }
    
    /// <summary>
    /// Tests that when a city exists and there is weather data available for the specified time, then the right data is returned.
    /// </summary>
    [Fact]
    public async Task GetLatestWeatherByCityAndTime_ReturnsCorrectWeatherData()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new WeatherRepository(dbContext);

        var weatherData = new List<WeatherData>
        {
            new() { StationName = "Tartu-Tõravere", WMOCode = "1244", Temperature = 10, WindSpeed = 2, Phenomenon = "Cloudy", Time = new DateTime(2024, 03, 20, 14, 15, 0) },
            new() { StationName = "Tartu-Tõravere", WMOCode = "1244", Temperature = 10, WindSpeed = 2, Phenomenon = "Cloudy", Time = new DateTime(2024, 03, 20, 14, 45, 0) }
        };
        
        await repository.AddWeatherData(weatherData);
        
        var time = new DateTime(2024, 03, 20, 14, 30, 0);
        var result = await repository.GetLatestWeatherByCityAndTime("tartu", time);
        Assert.NotNull(result);
        Assert.Equal(new DateTime(2024, 03, 20, 14, 15, 0), result.Time);
    }
    
    /// <summary>
    /// Tests that when a city exists and there is no weather data available for the specified time, then null is returned.
    /// </summary>
    [Fact]
    public async Task GetLatestWeatherByCityAndTime_ReturnsNullIfNoData()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new WeatherRepository(dbContext);
        
        var time = new DateTime(2024, 03, 20, 14, 30, 0);
        var result = await repository.GetLatestWeatherByCityAndTime("tallinn", time);
        
        Assert.Null(result);
    }
}