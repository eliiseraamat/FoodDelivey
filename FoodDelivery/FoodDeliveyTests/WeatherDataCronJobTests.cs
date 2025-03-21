using Microsoft.Extensions.DependencyInjection;
using Moq;
using Services;

namespace FoodDeliveyTests;

/// <summary>
/// Unit tests for the WeatherDataCronJob background service.
/// </summary>
public class WeatherDataCronJobTests
{
    /// <summary>
    /// Tests that the cron job calls FetchAndStoreWeatherData when the time is exactly 15 minutes past the hour.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_CallsFetchAndStoreWeatherData()
    {
        var mockWeatherService = new Mock<IWeatherDataService>();
        mockWeatherService
            .Setup(service => service.FetchAndStoreWeatherData())
            .Returns(Task.CompletedTask);

        var serviceProvider = new ServiceCollection()
            .AddSingleton(mockWeatherService.Object) 
            .BuildServiceProvider();
        
        var fakeTime = () => DateTime.UtcNow.Date.AddHours(12).AddMinutes(15);
        
        var cronJob = new WeatherDataCronJob(serviceProvider, fakeTime);
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        
        await cronJob.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
        
        await cts.CancelAsync();
        
        mockWeatherService.Verify(service => service.FetchAndStoreWeatherData(), Times.AtLeastOnce());
    }
    
    /// <summary>
    /// Tests that the cron job doesn't call FetchAndStoreWeatherData when the time is not 15 minutes past the hour.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_DoesNotCallFetchAndStoreWeatherData_WhenTimeIsNot15MinutesPastHour()
    {
        var mockWeatherService = new Mock<IWeatherDataService>();
        mockWeatherService
            .Setup(service => service.FetchAndStoreWeatherData())
            .Returns(Task.CompletedTask);
        
        var serviceProvider = new ServiceCollection()
            .AddSingleton(mockWeatherService.Object)
            .BuildServiceProvider();

        var fakeTime = () => DateTime.UtcNow.Date.AddHours(12).AddMinutes(10);
        
        var cronJob = new WeatherDataCronJob(serviceProvider, fakeTime);
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        
        await cronJob.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
        
        await cts.CancelAsync();
        
        mockWeatherService.Verify(service => service.FetchAndStoreWeatherData(), Times.Never());
    }
    
    /// <summary>
    /// Tests that changing the update interval makes the cron job fetch more frequently.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_IntervalChanged()
    {
        var mockWeatherService = new Mock<IWeatherDataService>();
        mockWeatherService
            .Setup(service => service.FetchAndStoreWeatherData())
            .Returns(Task.CompletedTask);

        var serviceProvider = new ServiceCollection()
            .AddSingleton(mockWeatherService.Object)
            .BuildServiceProvider();
        
        WeatherDataCronJob.UpdateIntervalMinutes = 1;

        var cronJob = new WeatherDataCronJob(serviceProvider, () => DateTime.UtcNow);
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(130));
        
        await cronJob.StartAsync(cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(125), cts.Token);
        
        await cts.CancelAsync();
        
        mockWeatherService.Verify(service => service.FetchAndStoreWeatherData(), Times.AtLeast(2));
    }
}
