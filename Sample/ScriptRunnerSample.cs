using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using dotnet_script.Entities;
using Microsoft.Data.SqlClient;

public class ScriptRunnerSample : IScriptRunner<int, WeatherForecast>
{
    private readonly SqlConnection _connection;
    public ScriptRunnerSample()
    {
        _connection = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Etomars;Integrated Security=True;MultipleActiveResultSets=True;");
    }
    public Task OnConsumer(WeatherForecast item)
    {
        Console.WriteLine(JsonSerializer.Serialize(item));
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<WeatherForecast>> OnProducer(int request)
    {
        var result = await _connection.QueryAsync<WeatherForecast>("SELECT * FROM [EXAMPLE].[WEATHERFORECASTS] WHERE ID = @ID", new{ID = request});
        return result;
    }
}