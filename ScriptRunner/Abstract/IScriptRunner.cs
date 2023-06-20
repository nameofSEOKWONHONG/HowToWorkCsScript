using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IScriptRunner<TRequest, TResult>
{
    Task<IEnumerable<TResult>> OnProducer(TRequest request);
    Task OnConsumer(TResult item);
}