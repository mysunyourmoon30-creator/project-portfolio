using FluentAssertions;
using Innovation.Core.Domain;
using Xunit;

namespace Innovation.Core.Tests;

public class WeightToleranceCalculatorTests
{
    [Fact]
    public void Calculate_NormalStep_UsesApplicationSettingOffsets()
    {
        var (min, max) = WeightToleranceCalculator.Calculate(
            stepNo: 1, kbTogetherNumber: 1, target: 10m, minToleranceSetting: 0.5m, maxToleranceSetting: 0.5m);

        min.Should().Be(9.5m);
        max.Should().Be(10.5m);
    }

    [Fact]
    public void Calculate_Step2WithNumberNotOne_UsesFixed002Tolerance()
    {
        var (min, max) = WeightToleranceCalculator.Calculate(
            stepNo: 2, kbTogetherNumber: 3, target: 10m, minToleranceSetting: 0.5m, maxToleranceSetting: 0.5m);

        min.Should().Be(9.98m);
        max.Should().Be(10.02m);
    }

    [Fact]
    public void Calculate_Step2WithNumberEqualsOne_UsesApplicationSettingOffsets()
    {
        var (min, max) = WeightToleranceCalculator.Calculate(
            stepNo: 2, kbTogetherNumber: 1, target: 10m, minToleranceSetting: 0.5m, maxToleranceSetting: 0.5m);

        min.Should().Be(9.5m);
        max.Should().Be(10.5m);
    }

    [Fact]
    public void Calculate_Step3WithNumberEqualsOne_UsesFixed002Tolerance()
    {
        var (min, max) = WeightToleranceCalculator.Calculate(
            stepNo: 3, kbTogetherNumber: 1, target: 10m, minToleranceSetting: 0.5m, maxToleranceSetting: 0.5m);

        min.Should().Be(9.98m);
        max.Should().Be(10.02m);
    }

    [Fact]
    public void Calculate_Step3WithNumberNotOne_UsesApplicationSettingOffsets()
    {
        var (min, max) = WeightToleranceCalculator.Calculate(
            stepNo: 3, kbTogetherNumber: 2, target: 10m, minToleranceSetting: 0.5m, maxToleranceSetting: 0.5m);

        min.Should().Be(9.5m);
        max.Should().Be(10.5m);
    }

    [Fact]
    public void Calculate_OtherSteps_AlwaysUseApplicationSettingOffsets_RegardlessOfNumber()
    {
        var (min, max) = WeightToleranceCalculator.Calculate(
            stepNo: 4, kbTogetherNumber: 1, target: 5m, minToleranceSetting: 0.3m, maxToleranceSetting: 0.4m);

        min.Should().Be(4.7m);
        max.Should().Be(5.4m);
    }
}
