using System;
using System.Threading.Tasks;
using dotnet_script.Entities;

public class Program
{   
    private static async Task Main(string[] args)
    {
        while (true) 
        {
            await CsScriptRunner.Create().ExecuteAsync<int, WeatherForecast>("ScriptRunnerSample", 10);
            Console.WriteLine("continue?(Y/N)");
            var yn = Console.ReadLine();
            if ((string.IsNullOrEmpty(yn) ? "Y" : yn) == "N")
            {
                break;
            }
        }
    }
}
