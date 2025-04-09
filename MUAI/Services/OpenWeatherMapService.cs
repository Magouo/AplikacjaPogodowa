using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MUAI.Models;

namespace MUAI;

public class OpenWeatherMapService
{
    private readonly HttpClient client;
    private readonly string apiKey;

    public OpenWeatherMapService(string apiKeys)
    {
        client = new HttpClient();
        client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        apiKey = apiKeys;
    }

    public async Task<WeatherData> GetWeatherByCityAsync(string cityName)
    {
        using (var db = new WeatherDbContext())
        {
            string normalizedCityName = cityName.ToLower();

            var existingRecord = db.WeatherRecords.Include(w => w.Wind)
                .FirstOrDefault(w => w.CityName == normalizedCityName);

            if (existingRecord != null)
            {
                return existingRecord;
            }

            string requestUrl = $"weather?q={Uri.EscapeDataString(cityName)}&appid={apiKey}&units=metric";
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var root = JsonDocument.Parse(jsonResponse).RootElement;

                var weatherData = new WeatherData
                {
                    CityName = root.GetProperty("name").GetString().ToLower(),
                    Temp = root.GetProperty("main").GetProperty("temp").GetDouble(),
                    Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                    Pressure = root.GetProperty("main").GetProperty("pressure").GetInt32(),
                };

                var windData = new WindData
                {
                    WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                    WindDeg = root.GetProperty("wind").GetProperty("deg").GetInt32(),
                    WeatherData = weatherData
                };

                db.WeatherRecords.Add(weatherData);
                db.WindRecords.Add(windData);
                db.SaveChanges();

                return weatherData;
            }

            return null;
        }
    }
}
