using System;
using System.Dynamic;
using System.IO;
using System.Threading;
using Jint;

namespace dotnet_script.ScriptRunner.Js;

public class JsSqlRunnerCore
{
    private readonly Engine _engine;
    public JsSqlRunnerCore(CancellationToken cancellationToken = new())
    {
        _engine = new Engine(options =>
        {
            // Limit memory allocations to MB
            options.LimitMemory(4_000_000);

            // Set a timeout to 4 seconds.
            options.TimeoutInterval(TimeSpan.FromSeconds(4));

            // Set limit of 1000 executed statements.
            options.MaxStatements(1000);

            // Use a cancellation token.
            options.CancellationToken(cancellationToken);

            var path = Directory.GetCurrentDirectory();
            options.EnableModules(path);
        });
    }

    public string Execute<TResult>(string filename, object obj)
    {
        var filepath = Path.Combine(Directory.GetCurrentDirectory(), $"{filename}.ejs");
        var code = File.ReadAllText(filepath);

        var value = new Jint.Engine()  // Create the Jint engine
            .Execute(code) // Define a function car() that accesses the Type field of the incoming obj and returns it.
            .Invoke("generate", obj);

        return value.AsString();
    }
}