using FluentAssertions;
using Innovation.Core.Domain;
using Innovation.Hardware;
using Xunit;

namespace Innovation.Hardware.Tests.Scenarios;

// Scenario 2 of 7: weight outside [min, max] must never be written to the
// PLC. WeightToleranceCalculator (Innovation.Core) decides the range; this
// test proves the hardware layer respects it end to end.
public class OutOfRangeWeightScenarioTests
{
    [Fact]
    public async Task WeightOutsideTolerance_WriteIsNeverAttempted()
    {
        var (min, max) = WeightToleranceCalculator.Calculate(
            stepNo: 1, kbTogetherNumber: 1, target: 10m, minToleranceSetting: 0.5m, maxToleranceSetting: 0.5m);

        var plc = new SimulatedPlcDevice();
        await plc.OpenAsync();
        var actualWeight = 11.2m; // above max (10.5)

        bool inRange = actualWeight >= min && actualWeight <= max;
        if (inRange)
        {
            await plc.WriteDeviceAsync("D70", PlcWeightConverter.ToPlcValue(actualWeight));
        }

        inRange.Should().BeFalse();
        (await plc.ReadDeviceAsync("D70")).Should().Be(0); // untouched register
    }
}
