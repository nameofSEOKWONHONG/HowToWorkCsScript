using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CSScriptLib;
using dotnet_script.Utils;

namespace dotnet_script.ScriptRunner.Sql;

public class SqlBuildScriptRunnerCore: IDisposable
{
    private readonly ConcurrentDictionary<string, string> _maps = new();
    private FileSystemWatcher _fileSystemWatcher;
    private DateTime _lastRead = DateTime.MinValue;

    public SqlBuildScriptRunnerCore()
    {
        _fileSystemWatcher = new FileSystemWatcher(AppContext.BaseDirectory)
        {
            NotifyFilter = NotifyFilters.LastWrite | 
                           NotifyFilters.FileName,
            Filter= "*.cs",
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };
        _fileSystemWatcher.Changed += WatcherHandler;
        _fileSystemWatcher.Created += WatcherHandler;
    }

    private void WatcherHandler(object sender, FileSystemEventArgs e)
    {
        DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
        if (lastWriteTime == _lastRead) return;
        _lastRead = lastWriteTime;
        Console.WriteLine("pass");
        if(e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Changed)
        {
            Interval.Set(() =>
            {
                var code = File.ReadAllText(e.FullPath);
                _maps.AddOrUpdate(Path.GetFileNameWithoutExtension(e.Name), code, (key, oldValue) => code);
            }, 1000);
        }
    }

    public void Initialize() {
        var files = Directory.GetFiles(AppContext.BaseDirectory, "*.cs");
        if(_maps.IsEmpty) {
            foreach(var file in files) {
                _maps.TryAdd(Path.GetFileNameWithoutExtension(file), File.ReadAllText(file));
            }
        }
    }

    public async Task<TResult> ExecuteAsync<TRequest, TResult>(DbConnection connection, string filename, TRequest request) {
        if(_maps.TryGetValue(filename, out var code)) {
            ISqlBuilderScriptRunner<TRequest, TResult> runner = CSScript.Evaluator
                .ReferenceAssembliesFromCode(code)
                .ReferenceAssembly(Assembly.GetExecutingAssembly())
                .ReferenceAssembly(Assembly.GetExecutingAssembly().Location)     
                .ReferenceDomainAssemblies()
                .LoadCode<ISqlBuilderScriptRunner<TRequest, TResult>>(code);
            runner.DbConnection = connection;
            var results = await runner.ExecuteAsync(request);
            return results;
        }
        throw new NotImplementedException($"{filename} not exist");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if(disposing)
        {
            _fileSystemWatcher.Changed -= WatcherHandler;
            _fileSystemWatcher.Created -= WatcherHandler;
            _fileSystemWatcher?.Dispose();
            _fileSystemWatcher = null;
        }
    }
}