using System.Diagnostics;
using Innovation.Mvp.Core.Async;

namespace Innovation.TotalWeight_PLC.Infrastructure;

// Phase 5 replacement for the original's static CallTracer class. Built on
// System.Diagnostics.ActivitySource instead of ad-hoc static mutable state -
// each case becomes a standard .NET Activity that any listener (console,
// OpenTelemetry, a debugger) can observe, and nothing here is a static
// field that could leak between concurrent operations the way the
// original's tracer implicitly assumed a single station/thread.
public sealed class OperationTracer : IOperationTracer
{
    private static readonly ActivitySource Source = new("Innovation.TotalWeight_PLC");
    private readonly AsyncLocal<Activity?> _currentActivity = new();

    public void StartCase(string context)
    {
        _currentActivity.Value = Source.StartActivity(context);
    }

    public void EndCase(bool success, string? errorMessage = null)
    {
        var activity = _currentActivity.Value;
        if (activity is null)
        {
            return;
        }

        activity.SetStatus(success ? ActivityStatusCode.Ok : ActivityStatusCode.Error, errorMessage);
        activity.Dispose();
        _currentActivity.Value = null;
    }
}
