using System.Globalization;
using System.Xml.Linq;
using DAL;

namespace Services;

/// <summary>
/// Service responsible for fetching weather data from Ilmateenistus and storing it in the database.
/// </summary>
/// <param name="weatherRepository">Repository to store weather data.</param>
/// <param name="httpClient">HTTP client for making API requests.</param>
public class WeatherDataService(IWeatherRepository weatherRepository, HttpClient httpClient) : IWeatherDataService
{
    private const string WeatherApiUrl = "https://www.ilmateenistus.ee/ilma_andmed/xml/observations.php";

    private static readonly string[] Stations = ["Tallinn-Harku", "Tartu-Tõravere", "Pärnu"];

    /// <summary>
    /// Fetches data from the weather portal of the Estonian Environment Agency and updates the database with the latest observations.
    /// </summary>
    public async Task FetchAndStoreWeatherData()
    {
        try
        {
            var response = await httpClient.GetStringAsync(WeatherApiUrl);

            var xml = XDocument.Parse(response);
            var observations = xml.Descendants("station")
                .Where(s => Stations.Contains(s.Element("name")!.Value))
                .Select(s => new Domain.WeatherData
                {
                    StationName = s.Element("name")!.Value,
                    WMOCode = s.Element("wmocode")!.Value,
                    Temperature = double.TryParse(s.Element("airtemperature")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var temp) ? temp : 1,
                    WindSpeed = double.TryParse(s.Element("windspeed")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var wind) ? wind : 0,
                    Phenomenon = s.Element("phenomenon")?.Value ?? "None",
                    Time = DateTime.UtcNow,
                })
                .ToList();
            await weatherRepository.AddWeatherDataAsync(observations);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather data: {ex.Message}");
        }
    }
}
