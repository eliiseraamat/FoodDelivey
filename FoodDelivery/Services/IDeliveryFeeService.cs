using Domain;

namespace Services;

/// <summary>
/// Defines a service responsible for calculating the delivery fee based on the city, vehicle type and weather conditions.
/// </summary>
public interface IDeliveryFeeService
{
    /// <summary>
    /// Calculates the delivery fee based on the given city, vehicle type and weather conditions.
    /// </summary>
    /// <param name="city">The city for which the delivery fee is calculated.</param>
    /// <param name="vehicleType">The type of vehicle used for the delivery.</param>
    /// <param name="dateTime">Optional date and time for which to calculate the fee.</param>
    /// <returns>A task that represents the asynchronous operation, containing the calculated delivery fee.</returns>
    Task<decimal> CalculateFeeAsync(Cities city, VehicleTypes vehicleType, DateTime? dateTime);
}