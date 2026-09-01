namespace Innovation.Mvp.Core.Async;

// Minimal replacement for the original CallTracer static class. Consumers
// (e.g. Phase 5's OperationTracer built on ActivitySource) implement this.
public interface IOperationTracer
{
    void StartCase(string context);

    void EndCase(bool success, string? errorMessage = null);
}
