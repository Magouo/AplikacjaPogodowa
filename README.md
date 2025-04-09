# Aplikacja pogodowa z integracją API i bazą danych  

---

## Wstęp  

Projekt został stworzony na platformie **.NET** (wersja 8.0) i implementuje aplikację pogodową, która:  
- Pobiera dane pogodowe z API OpenWeatherMap i zapisuje je w lokalnej bazie danych SQLite.  
- Umożliwia ręczne dodawanie danych pogodowych.  
- Obsługuje relacje między danymi pogodowymi a danymi o wietrze.  
- Udostępnia funkcjonalności takie jak wyświetlanie, sortowanie i usuwanie danych z bazy.  

Projekt składa się z dwóch aplikacji:  
- **Aplikacja konsolowa**: interaktywne zarządzanie danymi pogodowymi.  
- **Aplikacja okienkowa (MAUI)**: graficzny interfejs użytkownika umożliwiający pobieranie danych z API oraz ręczne dodawanie danych.  

---

## Opis funkcjonalności  

### Aplikacja konsolowa  

1. **Pobieranie danych pogodowych z API**:  
   - Użytkownik podaje nazwę miasta, a aplikacja pobiera dane pogodowe z API OpenWeatherMap.  
   - Dane są zapisywane w lokalnej bazie SQLite.  

2. **Wyświetlanie danych z bazy**:  
   - Aplikacja wyświetla wszystkie zapisane dane pogodowe.  

3. **Sortowanie danych według temperatury**:  
   - Dane pogodowe w bazie mogą być posortowane według temperatury.  

4. **Usuwanie bazy danych**:  
   - Użytkownik może usunąć całą bazę danych.  

---

### Aplikacja MAUI  

1. **Pobieranie danych z API**:  
   - Użytkownik podaje nazwę miasta, a aplikacja pobiera dane pogodowe z API OpenWeatherMap i zapisuje je w bazie danych.  

2. **Ręczne dodawanie danych**:  
   - Użytkownik może ręcznie wprowadzić dane pogodowe (miasto, temperatura, wilgotność, ciśnienie, prędkość wiatru, kierunek wiatru).  

3. **Wyświetlanie danych z bazy**:  
   - Dane pogodowe zapisane w bazie są wyświetlane w interfejsie graficznym.  

---

## Pliki programu oraz ich przeznaczenie  

### Aplikacja konsolowa  

#### **Program.cs**  
Zawiera cały kod aplikacji konsolowej.  
W pliku znajdują się:  

- **Klasa `WeatherData`**:  
  - Reprezentuje dane pogodowe.  
  - Zawiera właściwości:  
    - `CityName` - nazwa miasta.  
    - `Temp` - temperatura.  
    - `Humidity` - wilgotność.  
    - `Pressure` - ciśnienie.  
    - `Wind` - nawigacja do danych o wietrze.  

- **Klasa `WindData`**:  
  - Reprezentuje dane o wietrze.  
  - Zawiera właściwości:  
    - `WindSpeed` - prędkość wiatru.  
    - `WindDeg` - kierunek wiatru.  
    - `WeatherData` - nawigacja do danych pogodowych.  

- **Klasa `WeatherDbContext`**:  
  - Reprezentuje bazę danych SQLite.  
  - Zawiera konfigurację relacji między tabelami `WeatherData` i `WindData`.  

- **Klasa `OpenWeatherMapService`**:  
  - Obsługuje komunikację z API OpenWeatherMap.  
  - Pobiera dane pogodowe dla podanego miasta i zapisuje je w bazie danych.  

- **Kod główny**:  
  - Umożliwia użytkownikowi:  
    - Pobieranie danych z API.  
    - Wyświetlanie danych z bazy.  
    - Sortowanie danych według temperatury.  
    - Usuwanie bazy danych.  

---

### Aplikacja MAUI  

#### **Models/WeatherData.cs**  
- Reprezentuje dane pogodowe.  
- Zawiera właściwości:  
  - `CityName` - nazwa miasta.  
  - `Temp` - temperatura.  
  - `Humidity` - wilgotność.  
  - `Pressure` - ciśnienie.  
  - `Wind` - nawigacja do danych o wietrze.  

#### **Models/WindData.cs**  
- Reprezentuje dane o wietrze.  
- Zawiera właściwości:  
  - `WindSpeed` - prędkość wiatru.  
  - `WindDeg` - kierunek wiatru.  
  - `WeatherData` - nawigacja do danych pogodowych.  

#### **Services/OpenWeatherMapService.cs**  
- Obsługuje komunikację z API OpenWeatherMap.  
- Pobiera dane pogodowe dla podanego miasta i zapisuje je w bazie danych.  

#### **WeatherDbContext.cs**  
- Reprezentuje bazę danych SQLite.  
- Zawiera konfigurację relacji między tabelami `WeatherData` i `WindData`.  

#### **MainPage.xaml**  
- Definiuje interfejs graficzny aplikacji MAUI.  
- Zawiera:  
  - Pole do wprowadzenia nazwy miasta i przycisk do pobierania danych z API.  
  - Pola do ręcznego wprowadzania danych pogodowych.  
  - Listę wyświetlającą dane pogodowe zapisane w bazie.  

#### **MainPage.xaml.cs**  
- Obsługuje logikę interfejsu graficznego.  
- Zawiera:  
  - Metodę do pobierania danych z API.  
  - Metodę do ręcznego dodawania danych.  
  - Metodę do ładowania danych z bazy.  

---

## Działanie programu  

### Aplikacja konsolowa  

1. Użytkownik uruchamia aplikację i wybiera jedną z opcji:  
   - Pobranie danych pogodowych dla miasta.  
   - Wyświetlenie wszystkich danych z bazy.  
   - Posortowanie danych według temperatury.  
   - Usunięcie bazy danych.  
2. Aplikacja wykonuje odpowiednią operację i wyświetla wyniki w konsoli.  

### Aplikacja MAUI  

1. Użytkownik uruchamia aplikację i korzysta z interfejsu graficznego:  
   - Wprowadza nazwę miasta i pobiera dane z API.  
   - Ręcznie dodaje dane pogodowe.  
   - Przegląda dane zapisane w bazie.  

---

## Wykorzystane technologie  

- **.NET (wersja 8.0)**  
- **C#**  
- **Entity Framework Core** (do obsługi bazy danych SQLite)  
- **MAUI** (do aplikacji okienkowej)  
- **OpenWeatherMap API** (do pobierania danych pogodowych)  
