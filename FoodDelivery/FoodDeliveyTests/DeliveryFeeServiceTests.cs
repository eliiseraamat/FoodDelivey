using DAL;
using Domain;
using Moq;
using Services;

namespace FoodDeliveyTests;

/// <summary>
/// Unit tests for the DeliveryFeeService class.
/// </summary>
public class DeliveryFeeServiceTests
{
    /// <summary>
    /// Test to verify that if no weather data is available, the service returns -1 as an invalid fee.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_NoWeatherData()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Bike);
        
        Assert.Equal(-1, result);
    }
    
    /// <summary>
    /// Test to check that a valid city and vehicle type result in a correct fee with no extra charges.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_ValidCityAndVehicle_NoExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tartu, VehicleTypes.Car);
        
        Assert.Equal(3.5m, result);
    }

    /// <summary>
    /// Test to check that snowy weather adds an extra fee to the delivery cost.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_SnowyWeather_AddsExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 1, WindSpeed = 5, Phenomenon = "Snow" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tartu, VehicleTypes.Bike);
        
        Assert.Equal(3.5m, result);
    }

    /// <summary>
    /// Test to check that rainy weather adds an extra fee to the delivery cost.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_RainyWeather_AddsExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Pärnu"))
            .ReturnsAsync(new WeatherData { Temperature = 5, WindSpeed = 5, Phenomenon = "Rain" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Pärnu, VehicleTypes.Scooter);
        
        Assert.Equal(4.5m, result);
    }

    /// <summary>
    /// Test to verify that severe weather results in a -1 fee indicating invalid conditions.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_SevereWeather_ReturnsNegativeOne()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 5, Phenomenon = "Thunder" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Bike);
        
        Assert.Equal(-1, result);
    }
    
    /// <summary>
    /// Test to check that for cars, weather phenomenon does not add any extra fee.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_SevereWeatherCar_NoExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 5, Phenomenon = "Thunder" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Car);
        
        Assert.Equal(4, result);
    }
    
    /// <summary>
    /// Test to verify that an average wind speed increases the delivery fee for bikes.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_AverageWindSpeed_AddsExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 15, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Bike);
        
        Assert.Equal(3.5m, result);
    }
    
    /// <summary>
    /// Test to verify that an wind speed does not affect the fee for scooters and cars.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_AverageWindSpeedScooter_NoExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 15, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Scooter);
        
        Assert.Equal(3.5m, result);
    }
    
    /// <summary>
    /// Test to check that a high wind speed for bikes results in a negative fee (invalid conditions).
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_HighWindSpeedScooter_ReturnsNegativeOne()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 21, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tartu, VehicleTypes.Bike);
        
        Assert.Equal(-1, result);
    }
    
    /// <summary>
    /// Test to verify that a air temperature does not add extra fees for cars.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_AirTemperatureCar_NoExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = -5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tartu, VehicleTypes.Car);
        
        Assert.Equal(3.5m, result);
    }
    
    /// <summary>
    /// Test to verify that a low air temperature adds an extra fee for bikes and scooters.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_LowAirTemperature_AddsExtraFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Pärnu"))
            .ReturnsAsync(new WeatherData { Temperature = -5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Pärnu, VehicleTypes.Bike);
        
        Assert.Equal(2.5m, result);
    }
    
    /// <summary>
    /// Test to check that extremely low air temperatures results in an invalid fee for cars and scooters.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_ExtraLowAirTemperature_ReturnsNegativeOne()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Pärnu"))
            .ReturnsAsync(new WeatherData { Temperature = -11, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Pärnu, VehicleTypes.Scooter);
        
        Assert.Equal(3.5m, result);
    }
    
    /// <summary>
    /// Test to verify the total fee calculation based on weather conditions.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_TotalFee()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAsync("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = -2.1, WindSpeed = 4.7, Phenomenon = "Light snow shower" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFeeAsync(Cities.Tartu, VehicleTypes.Bike);
        
        Assert.Equal(4, result);
    }
}