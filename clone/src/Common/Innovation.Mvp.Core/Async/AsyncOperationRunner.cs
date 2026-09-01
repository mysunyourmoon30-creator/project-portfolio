namespace Innovation.Mvp.Core.Async;

// Replaces the original BaseForm.RunSafeAsync + `private static bool _caseActive`.
// The original static flag was only safe because the real app runs one
// station per process on one UI thread; a static field shared across every
// form instance is a latent bug waiting for a second concurrent operation.
// AsyncLocal<T> scopes "case ownership" to the logical async call chain
// instead, so concurrent/nested operations never stomp on each other -
// regardless of how many forms or threads are involved.
//
// Unlike the original (which swallowed exceptions after showing a message
// box), this always rethrows via `throw;` after reporting to the tracer, so
// a caller further up the chain (or a test) can observe the failure. Nothing
// here decides to show a message box - that is the presenter/view's job.
public sealed class AsyncOperationRunner : IAsyncOperationRunner
{
    private static readonly AsyncLocal<bool> CaseActive = new();
    private readonly IOperationTracer _tracer;

    public AsyncOperationRunner(IOperationTracer tracer)
    {
        _tracer = tracer;
    }

    public async Task RunAsync(string context, Func<Task> action, Action? onFinally = null)
    {
        bool ownsCase = !CaseActive.Value;
        if (ownsCase)
        {
            CaseActive.Value = true;
            _tracer.StartCase(context);
        }

        try
        {
            await action().ConfigureAwait(false);
            if (ownsCase)
            {
                _tracer.EndCase(success: true);
            }
        }
        catch (Exception ex)
        {
            if (ownsCase)
            {
                _tracer.EndCase(success: false, errorMessage: ex.Message);
            }

            throw;
        }
        finally
        {
            if (ownsCase)
            {
                CaseActive.Value = false;
            }

            onFinally?.Invoke();
        }
    }
}
