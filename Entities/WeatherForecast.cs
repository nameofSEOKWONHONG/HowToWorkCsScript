using System;

namespace dotnet_script.Entities;

public class WeatherForecast
{
    public int Id { get; set; }
    public string City { get; set; }

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

    /// <summary>
    /// allow null로 설정되어 있으나 Validator에서 필수로 구현함.
    /// WeatherForecastValidator 참조
    /// </summary>
    public string Summary { get; set; }
}