using FluentAssertions;
using Innovation.Hardware;
using Xunit;

namespace Innovation.Hardware.Tests.Scenarios;

// Scenario 3 of 7: PLC not connected / timeout.
public class PlcUnreachableScenarioTests
{
    [Fact]
    public async Task OpenAsync_ScriptedToFail_ThrowsPlcConnectionException()
    {
        var plc = new SimulatedPlcDevice(new PlcSimulationScript { FailToConnect = true });

        var act = () => plc.OpenAsync();

        await act.Should().ThrowAsync<PlcConnectionException>();
        plc.IsConnected.Should().BeFalse();
    }

    [Fact]
    public async Task ReadDevice_ResponseSlowerThanTimeout_ThrowsPlcTimeoutException()
    {
        var plc = new SimulatedPlcDevice(new PlcSimulationScript
        {
            ResponseDelay = TimeSpan.FromMilliseconds(200),
            Timeout = TimeSpan.FromMilliseconds(50),
        });
        await plc.OpenAsync();

        var act = () => plc.ReadDeviceAsync("D70");

        await act.Should().ThrowAsync<PlcTimeoutException>();
    }

    [Fact]
    public async Task WriteDevice_WhenNotOpen_ThrowsPlcConnectionException()
    {
        var plc = new SimulatedPlcDevice();

        var act = () => plc.WriteDeviceAsync("D70", 100);

        await act.Should().ThrowAsync<PlcConnectionException>();
    }
}
