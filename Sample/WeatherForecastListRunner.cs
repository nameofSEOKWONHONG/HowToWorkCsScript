using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using dotnet_script;
using dotnet_script.Entities;

public class WeatherForecastListRunner : ISqlBuilderScriptRunner<string, IEnumerable<WeatherForecast>>
{
    public DbConnection DbConnection { get; set; }
    public async Task<IEnumerable<WeatherForecast>> ExecuteAsync(string request)
    {
        var result = await SqlBuilder.Create().AddQuery(m =>
            {
                var builder = m.From("Example.WeatherForecasts")
                        .Select(
                            nameof(WeatherForecast.Id),
                            nameof(WeatherForecast.TemperatureC))
                    ;

                if (!string.IsNullOrWhiteSpace(request))
                {
                    builder.Where(nameof(WeatherForecast.City), request);
                }
                return builder;
            })
            .AddQuery(m => m.From("Example.WeatherForecasts")
                .Select(
                    nameof(WeatherForecast.Id),
                    nameof(WeatherForecast.TemperatureC))
                .Where(nameof(WeatherForecast.City), request))
            .Build()
            .ListAsync<WeatherForecast>(DbConnection);

        return result;
    }
}