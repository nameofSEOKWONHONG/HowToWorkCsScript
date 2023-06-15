using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CSScriptLib;

public class CsScriptRunner : IDisposable {
    private static Lazy<CsScriptRunner> _instance = new(() => new CsScriptRunner());
    public static CsScriptRunner Instance {
        get {
            return _instance.Value;
        }
    }

    private ConcurrentDictionary<string, string> _maps = new();
    private FileSystemWatcher _fileSystemWatcher;

    private CsScriptRunner()
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

    DateTime _lastRead = DateTime.MinValue;
    private void WatcherHandler(object sender, FileSystemEventArgs e)
    {
        DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
        if (lastWriteTime == _lastRead) return;
        _lastRead = lastWriteTime;
        Console.WriteLine("pass");
        if(e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Changed) {            
            Interval.Set(() => {
                var code = File.ReadAllText(e.FullPath);
                _maps.AddOrUpdate(Path.GetFileNameWithoutExtension(e.Name), code, (key ,oldValue) => code);
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

    public async Task Execute<TRequest, TResult>(string filename, TRequest request) {
        if(_maps.TryGetValue(filename, out var code)) {
            IScriptRunner<TRequest, TResult> runner = CSScript.Evaluator
                .ReferenceAssembliesFromCode(code)
                .ReferenceAssembly(Assembly.GetExecutingAssembly())
                .ReferenceAssembly(Assembly.GetExecutingAssembly().Location)     
                .ReferenceDomainAssemblies()
                .LoadCode<IScriptRunner<TRequest, TResult>>(code);
            var results = await runner.OnProducer(request);
            if(results != null) {
                foreach (var item in results)
                {
                    await runner.OnConsumer(item);
                }
            }

            return;
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
        if(disposing) {
            _fileSystemWatcher?.Dispose();
            _fileSystemWatcher = null;
        }
    }
}

public static class Interval
{
    public static System.Timers.Timer Set(System.Action action, int interval)
    {
        var timer = new System.Timers.Timer(interval);
        timer.Elapsed += (s, e) => {
            timer.Enabled = false;
            action();
            timer.Enabled = true;
        };
        timer.Enabled = true;
        return timer;
    }

    public static void Stop(System.Timers.Timer timer)
    {
        timer.Stop();
        timer.Dispose();
    }
}