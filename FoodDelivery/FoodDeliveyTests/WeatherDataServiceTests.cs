using System.Net;
using DAL;
using Domain;
using Moq;
using Moq.Protected;
using Services;

namespace FoodDeliveyTests;

public class WeatherDataServiceTests
{
    private readonly Mock<IWeatherRepository> _mockWeatherRepo = new();

    /// <summary>
    /// Tests that valid weather data is correctly fetched and saved to the repository.
    /// </summary>
    [Fact]
    public async Task FetchAndStoreWeatherData_ValidResponse_SavesDataToRepository()
    {
        const string xmlResponse = @"
            <observations>
                <station>
                    <name>Tallinn-Harku</name>
                    <wmocode>26038</wmocode>
                    <airtemperature>5</airtemperature>
                    <windspeed>3.2</windspeed>
                    <phenomenon>Clear</phenomenon>
                </station>
                <station>
                    <name>Tartu-Tõravere</name>
                    <wmocode>26242</wmocode>
                    <airtemperature>3.1</airtemperature>
                    <windspeed>2.8</windspeed>
                    <phenomenon>Cloudy</phenomenon>
                </station>
            </observations>";

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xmlResponse)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new WeatherDataService(_mockWeatherRepo.Object, httpClient);
        
        await service.FetchAndStoreWeatherData();
        
        _mockWeatherRepo.Verify(repo => repo.AddWeatherDataAsync(It.Is<List<WeatherData>>(list =>
            list.Count == 2 &&
            list.Any(d => d.StationName == "Tallinn-Harku" && d.Temperature == 5 && d.WindSpeed == 3.2) &&
            list.Any(d => d.StationName == "Tartu-Tõravere" && d.Temperature == 3.1 && d.WindSpeed == 2.8)
        )), Times.Once);
    }

    /// <summary>
    /// Tests that if an empty response is received, no data is saved to the repository.
    /// </summary>
    [Fact]
    public async Task FetchAndStoreWeatherData_EmptyResponse_DoesNotSaveData()
    {
        var xmlResponse = "<response><station></station></response>";

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xmlResponse)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new WeatherDataService(_mockWeatherRepo.Object, httpClient);
        
        await service.FetchAndStoreWeatherData();
        
        _mockWeatherRepo.Verify(repo => repo.AddWeatherDataAsync(It.IsAny<List<WeatherData>>()), Times.Never);
    }

    /// <summary>
    /// Tests that an invalid XML response does not crash the service.
    /// </summary>
    [Fact]
    public async Task FetchAndStoreWeatherData_InvalidXml_DoesNotCrash()
    {
        var invalidXml = "<invalid><data></data>";

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(invalidXml)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new WeatherDataService(_mockWeatherRepo.Object, httpClient);
        
        await service.FetchAndStoreWeatherData();
        _mockWeatherRepo.Verify(repo => repo.AddWeatherDataAsync(It.IsAny<List<WeatherData>>()), Times.Never);
    }
}