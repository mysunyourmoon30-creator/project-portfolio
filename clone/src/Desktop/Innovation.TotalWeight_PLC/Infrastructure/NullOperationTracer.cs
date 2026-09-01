using Innovation.Mvp.Core.Async;

namespace Innovation.TotalWeight_PLC.Infrastructure;

// Placeholder until Phase 5 replaces this with an ActivitySource-backed
// OperationTracer (see README §8.4 Phase 5 / CallTracer replacement).
public sealed class NullOperationTracer : IOperationTracer
{
    public void StartCase(string context)
    {
    }

    public void EndCase(bool success, string? errorMessage = null)
    {
    }
}
