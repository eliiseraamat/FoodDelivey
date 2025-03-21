using Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services;
using WebApi.Controllers;

namespace FoodDeliveyTests;

/// <summary>
/// Unit tests for the DeliveryFeeController class.
/// </summary>
public class DeliveryFeeControllerTests
{
    /// <summary>
    /// Test to verify that if an invalid city or vehicle type is provided, a BadRequest result is returned with correct message.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_InvalidCityOrVehicle()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        var controller = new DeliveryFeeController(mockService.Object);

        var result = await controller.GetDeliveryFee("InvalidCity", "Car", null);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorProperty = badRequestResult.Value?.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        var errorMessage = errorProperty.GetValue(badRequestResult.Value)?.ToString();

        Assert.Equal("City and vehicleType must be provided.", errorMessage);
    }
    
    /// <summary>
    /// Test to verify that if an forbidden vehicle type is provided, a BadRequest result is returned with correct message.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_ForbiddenVehicle()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        mockService
            .Setup(service => service.CalculateFeeAsync(It.IsAny<Cities>(), It.IsAny<VehicleTypes>(), null))
            .ReturnsAsync(-1); 
        
        var controller = new DeliveryFeeController(mockService.Object);
        
        var result = await controller.GetDeliveryFee("Tallinn", "Bike", null);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorProperty = badRequestResult.Value?.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        var errorMessage = errorProperty.GetValue(badRequestResult.Value)?.ToString();

        Assert.Equal("Usage of selected vehicle type is forbidden", errorMessage);
    }
    
    /// <summary>
    /// Test to check that a valid request returns an OK result with the correct delivery fee.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_ValidRequest()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        mockService
            .Setup(service => service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Car, null))
            .ReturnsAsync(4.0m);

        var controller = new DeliveryFeeController(mockService.Object);
        
        var result = await controller.GetDeliveryFee("Tallinn", "Car", null);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        
        var totalFeeProperty = okResult.Value?.GetType().GetProperty("TotalFee");
        Assert.NotNull(totalFeeProperty);
        var totalFee = totalFeeProperty.GetValue(okResult.Value);

        Assert.Equal(4.0m, totalFee);
    }
    
    /// <summary>
    /// Test to verify that if an internal server error occurs, a 500 status code and error message are returned.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_InternalServerError()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        mockService
            .Setup(service => service.CalculateFeeAsync(It.IsAny<Cities>(), It.IsAny<VehicleTypes>(), null))
            .ThrowsAsync(new Exception("Database error"));

        var controller = new DeliveryFeeController(mockService.Object);
        
        var result = await controller.GetDeliveryFee("Tallinn", "Car", null);
        
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        
        var errorProperty = objectResult.Value?.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        var errorMessage = errorProperty.GetValue(objectResult.Value);

        Assert.Equal("Internal server error", errorMessage);
    }
    
    /// <summary>
    /// Test to check that a valid request with a DateTime parameter returns an OK result with the correct delivery fee.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_ValidRequestWithDateTime()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        var requestTime = new DateTime(2024, 03, 20, 14, 30, 0); // Simulating the time the request is made

        // Setting up the service to calculate the fee based on a valid request and DateTime
        mockService
            .Setup(service => service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Car, requestTime))
            .ReturnsAsync(4.0m);

        var controller = new DeliveryFeeController(mockService.Object);
    
        var result = await controller.GetDeliveryFee("Tallinn", "Car", "2024-03-20T14:30:00");
    
        var okResult = Assert.IsType<OkObjectResult>(result);
    
        var totalFeeProperty = okResult.Value?.GetType().GetProperty("TotalFee");
        Assert.NotNull(totalFeeProperty);
        var totalFee = totalFeeProperty.GetValue(okResult.Value);

        Assert.Equal(4.0m, totalFee);
    }
    
    /// <summary>
    /// Test to check that if an invalid DateTime is provided, the controller returns a BadRequest with correct message.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_InvalidDateTime()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        
        mockService
            .Setup(service => service.CalculateFeeAsync(It.IsAny<Cities>(), It.IsAny<VehicleTypes>(), It.IsAny<DateTime>()))
            .ThrowsAsync(new ArgumentException("Invalid date time"));

        var controller = new DeliveryFeeController(mockService.Object);
    
        var result = await controller.GetDeliveryFee("Tallinn", "Car", "invalid-datetime");
    
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorProperty = badRequestResult.Value?.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        var errorMessage = errorProperty.GetValue(badRequestResult.Value)?.ToString();

        Assert.Equal("Invalid time format.", errorMessage);
    }
}
