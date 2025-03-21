using System.ComponentModel.DataAnnotations;

namespace Domain;

public class WeatherData
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string StationName { get; set; } = default!;
    
    [MaxLength(128)]
    public string WMOCode { get; set; } = default!;
    
    public double Temperature { get; set; }
    
    public double WindSpeed { get; set; }
    
    [MaxLength(128)]
    public string Phenomenon { get; set; } = default!;

    public DateTime Time { get; set; }
}
