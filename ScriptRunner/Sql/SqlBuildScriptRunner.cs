using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace dotnet_script.ScriptRunner.Sql;

public class SqlBuildScriptRunner
{
    private static readonly Lazy<SqlBuildScriptRunner> _instance = new(() => new SqlBuildScriptRunner());

    private readonly SqlBuildScriptRunnerCore _scriptRunnerCore;

    private SqlBuildScriptRunner()
    {
        _scriptRunnerCore = new();
        _scriptRunnerCore.Initialize();
    }

    public async Task<TResult> ExecuteAsync<TRequest, TResult>(DbConnection connection, string filename, TRequest request)
    {
        return await _scriptRunnerCore.ExecuteAsync<TRequest, TResult>(connection, filename, request);
    }

    public static SqlBuildScriptRunner Create()
    {
        return _instance.Value;
    }

    public void Dispose()
    {
        _scriptRunnerCore.Dispose();
    }
}