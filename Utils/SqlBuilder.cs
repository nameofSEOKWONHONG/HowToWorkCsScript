using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace dotnet_script;

public class SqlBuilder : IDisposable
{
    private readonly List<Func<Query, Query>> _queries;
    private SqlBuilder()
    {
        _queries = new List<Func<Query, Query>>();
    }

    public SqlBuilder AddQuery(Func<Query, Query> func)
    {
        _queries.Add(func);
        return this;
    }

    private readonly List<Query> _results = new List<Query>();
    public SqlBuilder Build()
    {
        foreach (var query in _queries)
        {
            var createdQuery = query.Invoke(new Query());
            _results.Add(createdQuery);
        }

        return this;
    }

    public async Task<TEntity> FirstAsync<TEntity>(IDbConnection connection)
    {
        var db = new QueryFactory(connection, new SqlServerCompiler());
        db.Logger = compiled =>
        {
            Console.WriteLine(compiled.ToString());
        };
        return await db.FirstOrDefaultAsync<TEntity>(_results.First());
    }

    public async Task<IEnumerable<TEntity>> ListAsync<TEntity>(DbConnection connection)
    {
        using var db = new QueryFactory(connection, new SqlServerCompiler());
        db.Logger = compiled =>
        {
            Console.WriteLine(compiled.ToString());
        };
        return await db.GetAsync<TEntity>(_results.First());
    }

    public static SqlBuilder Create()
    {
        return new SqlBuilder();
    }

    public void Dispose()
    {
    }
}

public class Sample
{
    public void Run()
    {
 
    }
}