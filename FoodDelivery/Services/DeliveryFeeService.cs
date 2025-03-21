using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Services;

/// <summary>
/// Service responsible for calculating delivery fees based on location, vehicle type, and weather conditions.
/// </summary>
/// <param name="weatherRepository">Repository to fetch weather data.</param>
public class DeliveryFeeService(IWeatherRepository weatherRepository) : IDeliveryFeeService
{
    /// <summary>
    /// Calculates the total delivery fee based on the selected city, vehicle type, and current weather conditions.
    /// </summary>
    /// <param name="city">The city where delivery is requested.</param>
    /// <param name="vehicleType">The type of vehicle used for delivery.</param>
    /// <param name="dateTime">Optional date and time for which to calculate the fee.</param>
    /// <returns>The total delivery fee or -1 if the delivery is not possible.</returns>
    public async Task<decimal> CalculateFeeAsync(Cities city, VehicleTypes vehicleType, DateTime? dateTime)
    {
        var cityString = Enum.GetName(city);
        WeatherData? weatherData;
        if (dateTime != null)
        {
            weatherData = await weatherRepository.GetLatestWeatherByCityAndTimeAsync(cityString!, dateTime.Value);
            if (weatherData == null) return -2;
        }
        else
        {
            weatherData = await weatherRepository.GetLatestWeatherByCityAsync(cityString!);
        }
        if (weatherData == null) return -1;
        var extraFee = CalculateExtraFee(vehicleType, weatherData);
        if (extraFee < 0) return -1;
        var baseFee = GetRegionalBaseFee(city, vehicleType);
        if (baseFee < 0) return -1;
        return baseFee + extraFee;
    }

    private static decimal GetRegionalBaseFee(Cities city, VehicleTypes vehicleType)
    {
        return city switch
        {
            Cities.Tallinn when vehicleType == VehicleTypes.Car => 4,
            Cities.Tallinn when vehicleType == VehicleTypes.Scooter => 3.5m,
            Cities.Tallinn => 3,
            Cities.Tartu when vehicleType == VehicleTypes.Car => 3.5m,
            Cities.Tartu when vehicleType == VehicleTypes.Scooter => 3,
            Cities.Tartu => 2.5m,
            Cities.Pärnu when vehicleType == VehicleTypes.Car => 3,
            Cities.Pärnu when vehicleType == VehicleTypes.Scooter => 2.5m,
            Cities.Pärnu => 2,
            _ => -1
        };
    }

    private static decimal CalculateExtraFee(VehicleTypes vehicleType, WeatherData weatherData)
    {
        var sum = 0m;
        var wsef = GetWindSpeedFee(vehicleType, weatherData);
        if (wsef == -1) return -1; 
        sum += wsef;
        var wpef = GetWeatherPhenomenonFee(vehicleType, weatherData);
        if (wpef == -1) return -1; 
        sum += wpef;
        return sum + GetAirTemperatureFee(vehicleType, weatherData);
    }

    private static decimal GetAirTemperatureFee(VehicleTypes vehicleType, WeatherData weatherData)
    {
        if (vehicleType == VehicleTypes.Car) return 0;
        var temperature = weatherData.Temperature;

        return temperature switch
        {
            < -10 => 1,
            >= -10 and <= 0 => 0.5m,
            _ => 0
        };
    }

    private static decimal GetWindSpeedFee(VehicleTypes vehicleType, WeatherData weatherData)
    {
        if (vehicleType == VehicleTypes.Car || vehicleType == VehicleTypes.Scooter) return 0;
        var windSpeed = weatherData.WindSpeed;
        return windSpeed switch
        {
            <= 20 and >= 10 => 0.5m,
            > 20 => -1,
            _ => 0
        };
    }

    private static decimal GetWeatherPhenomenonFee(VehicleTypes vehicleType, WeatherData weatherData)
    {
        if (vehicleType == VehicleTypes.Car) return 0;
        var weatherPhenomenon = weatherData.Phenomenon.ToLower();
        if (weatherPhenomenon.Contains("snow") || weatherPhenomenon.Contains("sleet"))
        {
            return 1;
        }

        if (weatherPhenomenon.Contains("rain"))
        {
            return 0.5m;
        }

        if (weatherPhenomenon == "glaze" || weatherPhenomenon == "hail" ||
            weatherPhenomenon == "thunder")
        {
            return -1;
        }

        return 0;
    }
}