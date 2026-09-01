using FluentAssertions;
using Innovation.Hardware;
using Xunit;

namespace Innovation.Hardware.Tests;

public class PlcWeightConverterTests
{
    [Fact]
    public void ToPlcValue_Truncates_NotRounds()
    {
        PlcWeightConverter.ToPlcValue(12.349m).Should().Be(1234);
    }

    [Fact]
    public void ToPlcValue_ExactTwoDecimals_NoPrecisionLoss()
    {
        PlcWeightConverter.ToPlcValue(10.05m).Should().Be(1005);
    }

    [Fact]
    public void FromPlcValue_RoundTrips_ExactTwoDecimalInput()
    {
        PlcWeightConverter.FromPlcValue(1234).Should().Be(12.34m);
    }
}
