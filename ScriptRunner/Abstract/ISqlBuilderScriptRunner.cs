using System.Data.Common;
using System.Threading.Tasks;

public interface ISqlBuilderScriptRunner<TRequest, TResult>
{
    DbConnection DbConnection { get; set; }
    Task<TResult> ExecuteAsync(TRequest request);
}