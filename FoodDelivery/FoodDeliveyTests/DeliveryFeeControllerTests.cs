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
    /// Test to verify that if an invalid city or vehicle type is provided, a BadRequest result is returned.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_InvalidCityOrVehicle_ReturnsBadRequest()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        var controller = new DeliveryFeeController(mockService.Object);
        
        var result = await controller.GetDeliveryFee("InvalidCity", "Car");
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorProperty = badRequestResult.Value?.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        var errorMessage = errorProperty.GetValue(badRequestResult.Value)?.ToString();

        Assert.Equal("City and vehicleType must be provided.", errorMessage);
    }
    
    /// <summary>
    /// Test to verify that if an forbidden city or vehicle type is provided, a BadRequest result is returned.
    /// </summary>
    [Fact]
    public async Task GetDeliveryFee_ForbiddenCityOrVehicle_ReturnsBadRequest()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        mockService
            .Setup(service => service.CalculateFeeAsync(It.IsAny<Cities>(), It.IsAny<VehicleTypes>()))
            .ReturnsAsync(-1); 
        
        var controller = new DeliveryFeeController(mockService.Object);
        
        var result = await controller.GetDeliveryFee("Tallinn", "Bike");
        
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
    public async Task GetDeliveryFee_ValidRequest_ReturnsOkWithFee()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        mockService
            .Setup(service => service.CalculateFeeAsync(Cities.Tallinn, VehicleTypes.Car))
            .ReturnsAsync(4.0m);

        var controller = new DeliveryFeeController(mockService.Object);
        
        var result = await controller.GetDeliveryFee("Tallinn", "Car");
        
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
    public async Task GetDeliveryFee_InternalServerError_Returns500()
    {
        var mockService = new Mock<IDeliveryFeeService>();
        mockService
            .Setup(service => service.CalculateFeeAsync(It.IsAny<Cities>(), It.IsAny<VehicleTypes>()))
            .ThrowsAsync(new Exception("Database error"));

        var controller = new DeliveryFeeController(mockService.Object);
        
        var result = await controller.GetDeliveryFee("Tallinn", "Car");
        
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        
        var errorProperty = objectResult.Value?.GetType().GetProperty("Error");
        Assert.NotNull(errorProperty);
        var errorMessage = errorProperty.GetValue(objectResult.Value);

        Assert.Equal("Internal server error", errorMessage);
    }
}