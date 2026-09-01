namespace Innovation.Mvp.Core.Async;

public interface IAsyncOperationRunner
{
    Task RunAsync(string context, Func<Task> action, Action? onFinally = null);
}
