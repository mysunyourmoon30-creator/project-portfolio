using FluentAssertions;
using Innovation.Hardware;
using Xunit;

namespace Innovation.Hardware.Tests.Scenarios;

// Scenario 1 of 7 from RUNTIME_TEST_CHECKLIST.md (referenced by both
// ROADMAP docs): normal weighing to completion.
public class NormalWeighingScenarioTests
{
    [Fact]
    public async Task WeighToTarget_ThenWritePlc_StoresTruncatedValue()
    {
        var plc = new SimulatedPlcDevice();
        var scale = new SimulatedScaleReader();
        await plc.OpenAsync();
        await scale.OpenAsync();

        decimal? weightAtStable = null;
        scale.WeightChanged += (_, w) => weightAtStable = w;
        scale.PushWeight(12.349m); // operator lands on a weight with 3 decimals

        await plc.WriteDeviceAsync("D70", PlcWeightConverter.ToPlcValue(weightAtStable!.Value));
        var writtenRaw = await plc.ReadDeviceAsync("D70");

        writtenRaw.Should().Be(1234); // truncated, not rounded to 1235
        PlcWeightConverter.FromPlcValue(writtenRaw).Should().Be(12.34m);
    }
}
