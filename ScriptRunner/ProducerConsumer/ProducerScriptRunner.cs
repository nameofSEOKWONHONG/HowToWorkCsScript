using System;
using System.Threading.Tasks;
using dotnet_script.ScriptRunner;

public class ProducerScriptRunner : IDisposable {
    private static readonly Lazy<ProducerScriptRunner> _instance = new(() => new ProducerScriptRunner());

    private readonly ScriptRunnerCore _scriptRunnerCore;

    private ProducerScriptRunner()
    {
        _scriptRunnerCore = new();
        _scriptRunnerCore.Initialize();
    }

    public async Task ExecuteAsync<TRequest, TResult>(string filename, TRequest request)
    {
        await _scriptRunnerCore.ExecuteAsync<TRequest, TResult>(filename, request);
    }

    public static ProducerScriptRunner Create()
    {
        return _instance.Value;
    }

    public void Dispose()
    {
        _scriptRunnerCore.Dispose();
    }
}

