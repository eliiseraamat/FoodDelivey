using Domain;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebApi.Controllers;

/// <summary>
/// Handles delivery fee calculations based on city and vehicle type.
/// Validates API requests and returns either the calculated delivery fee or an error message.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DeliveryFeeController(IDeliveryFeeService deliveryFeeService) : ControllerBase
{
    /// <summary>
    /// Calculates the delivery fee based on the given city and vehicle type.
    /// </summary>
    /// <param name="city">The city where delivery is requested.</param>
    /// <param name="vehicleType">The type of vehicle used for delivery.</param>
    /// <returns>
    /// Returns an HTTP 200 response with the calculated delivery fee if successful.
    /// Returns an HTTP 400 response if the request parameters are invalid or if the selected vehicle type is forbidden.
    /// Returns an HTTP 500 response in case of an internal server error.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetDeliveryFee([FromQuery] string city, [FromQuery] string vehicleType)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(vehicleType) || 
            !Enum.GetNames<Cities>().Contains(city, StringComparer.OrdinalIgnoreCase) || !Enum.GetNames<VehicleTypes>().Contains(vehicleType, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new { Error = "City and vehicleType must be provided." });
        }
        
        try
        {
            Enum.TryParse(city, true, out Cities cityEnum);
            Enum.TryParse(vehicleType, true, out VehicleTypes vehicleEnum);
            var fee = await deliveryFeeService.CalculateFeeAsync(cityEnum, vehicleEnum);
            if (fee == -1)
            {
                return BadRequest(new { Error = "Usage of selected vehicle type is forbidden" });
            }
            return Ok(new { TotalFee = fee });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }
}
