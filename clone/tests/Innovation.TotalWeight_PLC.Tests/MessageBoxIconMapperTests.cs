using FluentAssertions;
using Innovation.TotalWeight_PLC.Infrastructure;
using Innovation.TotalWeight_PLC.Interfaces.Views;
using Xunit;

namespace Innovation.TotalWeight_PLC.Tests;

// Guards against the icon bug caught by manually running the app: the
// original inline ternary (`type == Error ? Error : Warning`) silently
// mapped Information messages to a Warning (triangle) icon instead of the
// correct Information (circle-i) icon.
public class MessageBoxIconMapperTests
{
    [Theory]
    [InlineData(AppMessageType.Information, MessageBoxIcon.Information)]
    [InlineData(AppMessageType.Warning, MessageBoxIcon.Warning)]
    [InlineData(AppMessageType.Error, MessageBoxIcon.Error)]
    public void ToIcon_MapsEachMessageTypeToItsOwnIcon(AppMessageType type, MessageBoxIcon expected)
    {
        MessageBoxIconMapper.ToIcon(type).Should().Be(expected);
    }
}
