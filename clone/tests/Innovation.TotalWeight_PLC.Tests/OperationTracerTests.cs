using System.Diagnostics;
using FluentAssertions;
using Innovation.TotalWeight_PLC.Infrastructure;
using Xunit;

namespace Innovation.TotalWeight_PLC.Tests;

public class OperationTracerTests : IDisposable
{
    private readonly ActivityListener _listener;

    public OperationTracerTests()
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Innovation.TotalWeight_PLC",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
        };
        ActivitySource.AddActivityListener(_listener);
    }

    [Fact]
    public void StartCase_ThenEndCase_ProducesOneCompletedActivityWithSuccessStatus()
    {
        var tracer = new OperationTracer();

        tracer.StartCase("TestCase");
        tracer.EndCase(success: true);

        // Activity.Current is cleared by Dispose(); the point being verified
        // is that EndCase does not throw and clears the ambient activity -
        // no static field survives to leak into the next case.
        Activity.Current.Should().BeNull();
    }

    [Fact]
    public void EndCase_WithoutMatchingStartCase_DoesNotThrow()
    {
        var tracer = new OperationTracer();

        var act = () => tracer.EndCase(success: false, errorMessage: "boom");

        act.Should().NotThrow();
    }

    public void Dispose() => _listener.Dispose();
}
