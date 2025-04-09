using System.Text.Json;
using Microsoft.EntityFrameworkCore;

// relacje 
// Klasa  dane pogodowe
public class WeatherData
{
    public int Id { get; set; } // Klucz główny
    public string CityName { get; set; } //  Miasta
    public double Temp { get; set; } // Temperatura
    public int Humidity { get; set; } // Wilgotność
    public int Pressure { get; set; } // Ciśnienie
    
    public WindData Wind { get; set; } // Nawigacja do bazy z wiatrem
}

public class WindData
{
    public int Id { get; set; } // Klucz główny
    public int WeatherDataId { get; set; } // Klucz obcy do WeatherData 
    public WeatherData WeatherData { get; set; } // Nawigacja do WeatherData
    public double WindSpeed { get; set; } // Prędkość wiatru
    public int WindDeg { get; set; } // Kierunek wiatru
}
// Klasa bazy danych
public class WeatherDbContext : DbContext
{
    public DbSet<WeatherData> WeatherRecords { get; set; }
    public DbSet<WindData> WindRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=weather.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.Entity<WindData>()
            .HasOne(w => w.WeatherData)
            .WithOne(w => w.Wind)
            .HasForeignKey<WindData>(w => w.WeatherDataId);
    }
}

// Klasa do API
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

            // Sprawdzenie czy już jest
            var existingRecord = db.WeatherRecords.Include(w  => w.Wind).FirstOrDefault(w => w.CityName == normalizedCityName);
            if (existingRecord != null)
            {
                Console.WriteLine($"Dane dla miasta {cityName} pobrano z bazy danych.");
                return existingRecord;
            }

            // Pobieranie  z API
            string requestUrl = $"weather?q={Uri.EscapeDataString(cityName)}&appid={apiKey}&units=metric";
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var root = jsonDocument.RootElement;

                WeatherData weatherData = new WeatherData
                {
                    CityName = root.GetProperty("name").GetString().ToLower(),
                    Temp = root.GetProperty("main").GetProperty("temp").GetDouble(),
                    Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                    Pressure = root.GetProperty("main").GetProperty("pressure").GetInt32(),
                };

                WindData windData = new WindData
                {
                    WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                    WindDeg = root.GetProperty("wind").GetProperty("deg").GetInt32(),
                    WeatherData = weatherData
                };
                // Zapis
                db.WeatherRecords.Add(weatherData);
                db.WindRecords.Add(windData);
                db.SaveChanges();

                Console.WriteLine($"Dane dla miasta {cityName} pobrano z API i zapisano do bazy.");
                return weatherData;
            }
            else
            {
                Console.WriteLine($"HTTP Error: {(int)response.StatusCode} ({response.ReasonPhrase})");
                return null;
            }
        }
    }
}

internal class Program
{
    static async Task Main(string[] args)
    {
        const string apiKey = "b067ffeb856145cdcd8eb54d51c7b9d7";

        using (var db = new WeatherDbContext())
        {
            db.Database.EnsureCreated();
        }

        OpenWeatherMapService weatherService = new OpenWeatherMapService(apiKey);

        while (true)
        {
            Console.WriteLine("\nWybierz opcję:");
            Console.WriteLine("1. Pobierz dane pogodowe dla miasta");
            Console.WriteLine("2. Wyświetl wszystkie dane z bazy");
            Console.WriteLine("3. Posortuj dane według temperatury");
            Console.WriteLine("4. Usuń bazę danych");
            Console.WriteLine("5. Wyjście");
            Console.Write("Twój wybór: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Podaj nazwę miasta: ");
                    string city = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        WeatherData data = await weatherService.GetWeatherByCityAsync(city);
                        if (data != null)
                        {
                            Console.WriteLine($"\n--- Pogoda dla {data.CityName} ---");
                            Console.WriteLine($"Temperatura: {data.Temp}°C");
                            Console.WriteLine($"Wilgotność: {data.Humidity}%");
                            Console.WriteLine($"Ciśnienie: {data.Pressure} hPa");
                            Console.WriteLine($"Wiatr: {data.Wind.WindSpeed} m/s, kierunek: {data.Wind.WindDeg}°");
                        }
                    }
                    break;

                case "2":
                    using (var db = new WeatherDbContext())
                    {
                        var records = db.WeatherRecords.ToList();

                        Console.WriteLine("\n--- Wszystkie dane w bazie ---");
                        foreach (var record in records)
                        {
                            Console.WriteLine($"Miasto: {record.CityName}, Temp: {record.Temp}°C");
                        }
                    }
                    break;

                case "3":
                    using (var db = new WeatherDbContext())
                    {
                        var sortedRecords = db.WeatherRecords.OrderBy(w => w.Temp).ToList();

                        Console.WriteLine("\n--- Dane posortowane według temperatury ---");
                        foreach (var record in sortedRecords)
                        {
                            Console.WriteLine($"Miasto: {record.CityName}, Temp: {record.Temp}°C");
                        }
                    }
                    break;

                case "4":
                    using (var db = new WeatherDbContext())
                    {
                        if (db.Database.EnsureDeleted())
                        {
                            Console.WriteLine("Baza danych została usunięta.");
                        }
                        else
                        {
                            Console.WriteLine("Nie udało się usunąć bazy danych lub baza nie istnieje.");
                        }
                    }
                    break;

                case "5":
                    Console.WriteLine("Zamykanie programu...");
                    return;

                default:
                    Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }
}
