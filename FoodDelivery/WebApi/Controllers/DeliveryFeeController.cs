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
    /// <param name="time">Optional date and time for which to calculate the fee. If not provided, current time is used.</param>
    /// <returns>
    /// Returns an HTTP 200 response with the calculated delivery fee if successful.
    /// Returns an HTTP 400 response if the request parameters are invalid or if the selected vehicle type is forbidden.
    /// Returns an HTTP 500 response in case of an internal server error.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetDeliveryFee([FromQuery] string city, [FromQuery] string vehicleType, [FromQuery] string? time)
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
            DateTime? dateTime = null;

            if (!string.IsNullOrEmpty(time))
            {
                if (DateTime.TryParse(time, out var parsedDateTime))
                {
                    dateTime = parsedDateTime;
                }
                else
                {
                    return BadRequest(new { Error = "Invalid time format." });
                }
            }
            else
            {
                dateTime = DateTime.UtcNow;
            }
            var fee = await deliveryFeeService.CalculateFee(cityEnum, vehicleEnum, dateTime);
            return fee switch
            {
                -1 => BadRequest(new { Error = "Usage of selected vehicle type is forbidden" }),
                -2 => BadRequest(new { Error = "No weather information provided on selected time" }),
                _ => Ok(new { TotalFee = fee })
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }
}
