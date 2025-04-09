namespace MUAI.Models;

public class WeatherData
{
    public int Id { get; set; }
    public string CityName { get; set; }
    public double Temp { get; set; }
    public int Humidity { get; set; }
    public int Pressure { get; set; }

    public WindData Wind { get; set; }
}
