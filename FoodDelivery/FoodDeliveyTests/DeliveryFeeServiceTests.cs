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
            .Setup(repo => repo.GetLatestWeatherByCity("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tallinn, VehicleTypes.Bike, null);
        
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
            .Setup(repo => repo.GetLatestWeatherByCity("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tartu, VehicleTypes.Car, null);
        
        Assert.Equal(3.5m, result);
    }

    /// <summary>
    /// Test to check that snowy weather adds an extra fee to the delivery cost.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_SnowyWeather()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 1, WindSpeed = 5, Phenomenon = "Snow" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tartu, VehicleTypes.Bike, null);
        
        Assert.Equal(3.5m, result);
    }

    /// <summary>
    /// Test to check that rainy weather adds an extra fee to the delivery cost.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_RainyWeather()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Pärnu"))
            .ReturnsAsync(new WeatherData { Temperature = 5, WindSpeed = 5, Phenomenon = "Rain" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Pärnu, VehicleTypes.Scooter, null);
        
        Assert.Equal(3, result);
    }

    /// <summary>
    /// Test to verify that severe weather results in a -1 fee indicating invalid conditions.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_SevereWeather()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 5, Phenomenon = "Thunder" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tallinn, VehicleTypes.Bike, null);
        
        Assert.Equal(-1, result);
    }
    
    /// <summary>
    /// Test to check that for cars, weather phenomenon does not add any extra fee.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_SevereWeatherCar()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 5, Phenomenon = "Thunder" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tallinn, VehicleTypes.Car, null);
        
        Assert.Equal(4, result);
    }
    
    /// <summary>
    /// Test to verify that an average wind speed increases the delivery fee for bikes.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_AverageWindSpeed()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 15, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tallinn, VehicleTypes.Bike, null);
        
        Assert.Equal(3.5m, result);
    }
    
    /// <summary>
    /// Test to verify that an wind speed does not affect the fee for scooters and cars.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_AverageWindSpeedScooter()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Tallinn"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 15, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tallinn, VehicleTypes.Scooter, null);
        
        Assert.Equal(3.5m, result);
    }
    
    /// <summary>
    /// Test to check that a high wind speed for bikes results in a negative fee (invalid conditions).
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_HighWindSpeedBike()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = 3, WindSpeed = 21, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tartu, VehicleTypes.Bike, null);
        
        Assert.Equal(-1, result);
    }
    
    /// <summary>
    /// Test to verify that a air temperature does not add extra fees for cars.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_AirTemperatureCar()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = -5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tartu, VehicleTypes.Car, null);
        
        Assert.Equal(3.5m, result);
    }
    
    /// <summary>
    /// Test to verify that a low air temperature adds an extra fee for bikes and scooters.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_LowAirTemperature()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Pärnu"))
            .ReturnsAsync(new WeatherData { Temperature = -5, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Pärnu, VehicleTypes.Bike, null);
        
        Assert.Equal(2.5m, result);
    }
    
    /// <summary>
    /// Test to check that extremely low air temperatures results in an invalid fee for bikes and scooters.
    /// </summary>
    [Fact]
    public async Task CalculateFeeAsync_ExtraLowAirTemperature()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCity("Pärnu"))
            .ReturnsAsync(new WeatherData { Temperature = -11, WindSpeed = 5, Phenomenon = "Clear" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Pärnu, VehicleTypes.Scooter, null);
        
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
            .Setup(repo => repo.GetLatestWeatherByCity("Tartu"))
            .ReturnsAsync(new WeatherData { Temperature = -2.1, WindSpeed = 4.7, Phenomenon = "Light snow shower" });

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tartu, VehicleTypes.Bike, null);
        
        Assert.Equal(4, result);
    }
    
    /// <summary>
    /// Test to check that no weather data for selected time results a negative fee.
    /// </summary>
    [Fact]
    public async Task CalculateFee_NoWeatherDataForSelectedTime()
    {
        var mockWeatherRepo = new Mock<IWeatherRepository>();
        
        var testTime = new DateTime(2024, 01, 01, 12, 00, 00);
        
        mockWeatherRepo
            .Setup(repo => repo.GetLatestWeatherByCityAndTime(Cities.Tallinn.ToString(), testTime))
            .ReturnsAsync((WeatherData?)null);

        var service = new DeliveryFeeService(mockWeatherRepo.Object);
        
        var result = await service.CalculateFee(Cities.Tallinn, VehicleTypes.Bike, testTime);
        
        Assert.Equal(-2, result);
    }
}
