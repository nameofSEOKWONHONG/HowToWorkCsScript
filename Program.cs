using System;
using System.Threading.Tasks;

public class Program
{
    
    private static async Task Main(string[] args)
    {
        CsScriptRunner.Instance.Initialize();
        for(var i = 0; i < 20; i++) {
            await Task.Delay(1000);
            await CsScriptRunner.Instance.Execute<int, string>("ScriptRunnerSample", 10);
        }
    }
}
