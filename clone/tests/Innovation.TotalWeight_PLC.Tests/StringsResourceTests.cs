using FluentAssertions;
using Innovation.TotalWeight_PLC.Resources;
using Xunit;

namespace Innovation.TotalWeight_PLC.Tests;

// Guards against the resx manifest name / ResourceManager base name silently
// mismatching - GetString returns null (Strings.Get falls back to the key
// itself) rather than throwing, so a broken wiring would pass a build but
// show raw resource keys like "SaveSuccess" to operators instead of Thai text.
public class StringsResourceTests
{
    [Fact]
    public void SaveSuccess_ResolvesToThaiText_NotTheResourceKey()
    {
        Strings.SaveSuccess.Should().Be("บันทึกน้ำหนักเรียบร้อยแล้ว");
    }

    [Fact]
    public void BarcodeNotFound_FormatsArgumentIntoThaiText()
    {
        Strings.BarcodeNotFound("KB999").Should().Be("ไม่พบคัมบัง: KB999");
    }
}
