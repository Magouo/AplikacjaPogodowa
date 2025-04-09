using MUAI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace MUAI;

public partial class MainPage : ContentPage
{
    private ObservableCollection<WeatherData> weatherList = new();
    private readonly OpenWeatherMapService weatherService;

    public MainPage()
    {
        InitializeComponent();
        WeatherList.ItemsSource = weatherList;
        weatherService = new OpenWeatherMapService("b067ffeb856145cdcd8eb54d51c7b9d7");

        using (var db = new WeatherDbContext())
        {
            db.Database.EnsureCreated();
        }

        LoadDataFromDb();

    }

    private void LoadDataFromDb()
    {
        using var db = new WeatherDbContext();
        var data = db.WeatherRecords.Include(w => w.Wind).ToList();
        foreach (var item in data)
            weatherList.Add(item);
    }

    private async void OnApiClick(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(CityEntry.Text))
        {
            var data = await weatherService.GetWeatherByCityAsync(CityEntry.Text);
            if (data != null && !weatherList.Any(w => w.CityName == data.CityName))
                weatherList.Add(data);
        }
    }

    private void OnManualAddClick(object sender, EventArgs e)
    {
        try
        {
            var weather = new WeatherData
            {
                CityName = ManualCity.Text.ToLower(),
                Temp = double.Parse(ManualTemp.Text),
                Humidity = int.Parse(ManualHumidity.Text),
                Pressure = int.Parse(ManualPressure.Text),
            };

            var wind = new WindData
            {
                WindSpeed = double.Parse(ManualWindSpeed.Text),
                WindDeg = int.Parse(ManualWindDeg.Text),
                WeatherData = weather
            };

            using var db = new WeatherDbContext();
            db.WeatherRecords.Add(weather);
            db.WindRecords.Add(wind);
            db.SaveChanges();

            weatherList.Add(weather);
        }
        catch (Exception ex)
        {
            DisplayAlert("Błąd", "Niepoprawne dane wejściowe: " + ex.Message, "OK");
        }
    }
}
