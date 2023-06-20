using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using dotnet_script;
using dotnet_script.Entities;
using dotnet_script.ScriptRunner.Sql;
using Microsoft.Data.SqlClient;

public class Program
{   
    private static async Task Main(string[] args)
    {
        await SqlKataScript();

        await SqlBuild();

        await ProducerNConsumer();

        await Transaction();
    }

    private static async Task SqlKataScript()
    {
        #region [Sql Kata Scriptzation]
        while (true)
        {
            SqlConnection con = new SqlConnection(
                "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Etomars;Integrated Security=True;MultipleActiveResultSets=True;");
        
            await con.OpenAsync();
            var result = await SqlBuildScriptRunner.Create()
                .ExecuteAsync<string, IEnumerable<WeatherForecast>>(con, "WeatherForecastListRunner", "seoul");
            
            Console.WriteLine(JsonSerializer.Serialize(result));
            await con.CloseAsync();
            Console.ReadLine();
        }
        #endregion
    }

    private static async Task SqlBuild()
    {
        #region [SqlBuilder]

        SqlConnection con = new SqlConnection(
            "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Etomars;Integrated Security=True;MultipleActiveResultSets=True;");
        var id = 0;
        var city = "seoul";
        var result = await SqlBuilder.Create().AddQuery(m =>
            {
                var builder = m.From("Example.WeatherForecasts")
                        .Select(
                            nameof(WeatherForecast.Id),
                            nameof(WeatherForecast.TemperatureC))
                    ;
                if (id > 0)
                {
                    builder.Where(nameof(WeatherForecast.Id), id);
                }
        
                if (!string.IsNullOrWhiteSpace(city))
                {
                    builder.Where(nameof(WeatherForecast.City), city);
                }
                return builder;
            })
            .AddQuery(m => m.From("Example.WeatherForecasts")
                .Select(
                    nameof(WeatherForecast.Id),
                    nameof(WeatherForecast.TemperatureC))
                .Where(nameof(WeatherForecast.Id), id))
            .Build()
            .ListAsync<WeatherForecast>(con);
        
        if (result != null)
        {
            Console.WriteLine(JsonSerializer.Serialize(result));
        }

        #endregion
    }

    private static async Task ProducerNConsumer()
    {
        #region [Producer, Consumer]

        while (true) 
        {
            await ProducerScriptRunner.Create().ExecuteAsync<int, WeatherForecast>("ScriptRunnerSample", 10);
            Console.WriteLine("continue?(Y/N)");
            var yn = Console.ReadLine();
            if ((string.IsNullOrEmpty(yn) ? "Y" : yn) == "N")
            {
                break;
            }
        }        

        #endregion
    }

    private static async Task Transaction()
    {
        #region [Transaction]

        SqlConnection con = new SqlConnection(
            "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Etomars;Integrated Security=True;MultipleActiveResultSets=True;");
        try
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await con.OpenAsync();
                using (var scope4 = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    var item = await con.QueryFirstAsync<WeatherForecast>(
                        "SELECT * FROM [EXAMPLE].[WEATHERFORECASTS] WHERE ID = @ID", new { ID = 10 });
        
                    using (var scope2 = new TransactionScope(TransactionScopeOption.Required))
                    {
                        await con.ExecuteAsync("UPDATE [EXAMPLE].[WEATHERFORECASTS] SET Summary = @SUMMARY WHERE ID = @ID",
                            new { ID = 10, SUMMARY = "t26W" });
        
                        using (var scope3 = new TransactionScope(TransactionScopeOption.Required))
                        {
                            await con.ExecuteAsync(
                                "UPDATE [EXAMPLE].[WEATHERFORECASTS] SET Summary = @SUMMARY WHERE ID = @ID",
                                new { ID = 11, SUMMARY = "t28" });
                    
                            scope3.Complete();
                        }
                
                    
                        scope2.Complete();
                        throw new Exception("test");
                               
                    }
                    scope4.Complete();
                }
                scope.Complete();
            }
        }
        catch (Exception e)
        {
            
        }
        finally
        {
            await con.CloseAsync();
            await con.DisposeAsync();
        }

        #endregion
    }
}
