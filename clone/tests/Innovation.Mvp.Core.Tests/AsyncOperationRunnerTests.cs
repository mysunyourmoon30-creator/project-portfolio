using FluentAssertions;
using Innovation.Mvp.Core.Async;
using NSubstitute;
using Xunit;

namespace Innovation.Mvp.Core.Tests;

public class AsyncOperationRunnerTests
{
    [Fact]
    public async Task RunAsync_NestedCalls_OnlyOutermostOwnsTheCase()
    {
        var tracer = Substitute.For<IOperationTracer>();
        var runner = new AsyncOperationRunner(tracer);

        await runner.RunAsync("outer", async () =>
        {
            await runner.RunAsync("inner", async () => await Task.Delay(1));
        });

        tracer.Received(1).StartCase("outer");
        tracer.DidNotReceive().StartCase("inner");
        tracer.Received(1).EndCase(true, null);
    }

    [Fact]
    public async Task RunAsync_WhenActionThrows_ReportsFailureAndRethrows()
    {
        var tracer = Substitute.For<IOperationTracer>();
        var runner = new AsyncOperationRunner(tracer);

        var act = () => runner.RunAsync("failing", () => throw new InvalidOperationException("boom"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
        tracer.Received(1).EndCase(false, "boom");
    }

    [Fact]
    public async Task RunAsync_ConcurrentCallsOnDifferentThreads_DoNotInterfere()
    {
        var tracer = Substitute.For<IOperationTracer>();
        var runner = new AsyncOperationRunner(tracer);

        var taskA = Task.Run(() => runner.RunAsync("A", async () => await Task.Delay(20)));
        var taskB = Task.Run(() => runner.RunAsync("B", async () => await Task.Delay(20)));
        await Task.WhenAll(taskA, taskB);

        tracer.Received(1).StartCase("A");
        tracer.Received(1).StartCase("B");
        tracer.Received(2).EndCase(true, null);
    }
}
