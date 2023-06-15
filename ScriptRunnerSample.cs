using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ScriptRunnerSample : IScriptRunner<int, string>
{
    public Task OnConsumer(string item)
    {
        Console.WriteLine(item);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> OnProducer(int request)
    {
        var list = new List<string>();
        for(int i = 0; i < request; i++) {
            list.Add(i.ToString());
        }
        return Task.FromResult<IEnumerable<string>>(list);
    }
}