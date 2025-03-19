using Domain;

namespace Services;

public interface IDeliveryFeeService
{
    Task<decimal> CalculateFeeAsync(Cities city, VehicleTypes vehicleType);
}