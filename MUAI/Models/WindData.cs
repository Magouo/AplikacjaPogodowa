namespace MUAI.Models;

public class WindData
{
    public int Id { get; set; }
    public int WeatherDataId { get; set; }
    public WeatherData WeatherData { get; set; }
    public double WindSpeed { get; set; }
    public int WindDeg { get; set; }
}
