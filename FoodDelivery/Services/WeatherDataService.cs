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
    /// Fetches weather data from Ilmateenistus and updates the database with the latest observations.
    /// </summary>
    public async Task FetchAndStoreWeatherData()
    {
        try
        {
            var response = await httpClient.GetStringAsync(WeatherApiUrl);

            var xml = XDocument.Parse(response);
            var observations = xml.Descendants("station")
                .Where(st => Stations.Contains(st.Element("name")!.Value))
                .Select(st => new Domain.WeatherData
                {
                    StationName = st.Element("name")!.Value,
                    WMOCode = st.Element("wmocode")!.Value,
                    Temperature = double.TryParse(st.Element("airtemperature")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var temp) ? temp : 1,
                    WindSpeed = double.TryParse(st.Element("windspeed")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var wind) ? wind : 0,
                    Phenomenon = st.Element("phenomenon")?.Value ?? "None",
                    Time = DateTime.UtcNow,
                })
                .ToList();
            await weatherRepository.AddWeatherData(observations);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather data: {ex.Message}");
        }
    }
}
