using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Services;

/// <summary>
/// A background service that periodically fetches and stores weather data.
/// </summary>
/// <param name="serviceProvider">The application's service provider for dependency injection.</param>
/// <param name="getCurrentTime">A function that provides the current time</param>
public class WeatherDataCronJob(IServiceProvider serviceProvider, Func<DateTime> getCurrentTime) : BackgroundService
{
    /// <summary>
    /// Defines how often the job runs (in minutes).
    /// </summary>
    public static int UpdateIntervalMinutes { get; set; } = 60; 
    
    /// <summary>
    /// Executes the background job that fetches weather data every hour at 15 minutes past the hour.
    /// </summary>
    /// <param name="stoppingToken">A token to signal cancellation of the background task.</param>
    /// <returns>A task representing the execution of the cron job.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = getCurrentTime();

            if (UpdateIntervalMinutes == 60)
            {
                if (now.Minute == 15)
                {
                    await FetchWeatherData();
                }
            }
            else
            {
                if (now.Minute % UpdateIntervalMinutes == 0)
                {
                    await FetchWeatherData();
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
    
    private async Task FetchWeatherData()
    {
        using var scope = serviceProvider.CreateScope();
        var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherDataService>();
        await weatherService.FetchAndStoreWeatherData();
    }
}
