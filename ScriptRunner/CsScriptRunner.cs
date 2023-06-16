using System;
using System.Threading.Tasks;
using dotnet_script.ScriptRunner;

public class CsScriptRunner : IDisposable {
    private static readonly Lazy<CsScriptRunner> _instance = new(() => new CsScriptRunner());

    private readonly ScriptRunnerCore _scriptRunnerCore;

    private CsScriptRunner()
    {
        _scriptRunnerCore = new();
        _scriptRunnerCore.Initialize();
    }

    public async Task ExecuteAsync<TRequest, TResult>(string filename, TRequest request)
    {
        await _scriptRunnerCore.ExecuteAsync<TRequest, TResult>(filename, request);
    }

    public static CsScriptRunner Create()
    {
        return _instance.Value;
    }

    public void Dispose()
    {
        _scriptRunnerCore.Dispose();
    }
}

