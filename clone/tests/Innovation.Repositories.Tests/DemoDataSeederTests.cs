using FluentAssertions;
using Innovation.Data.Seed;
using Xunit;

namespace Innovation.Repositories.Tests;

public class DemoDataSeederTests
{
    [Fact]
    public void Seed_ProducesOneWeighableDemoKanban_WithBackingRmBalAndSettings()
    {
        using var factory = new SqliteTestDbContextFactory();
        using var context = factory.CreateContext();

        DemoDataSeeder.Seed(context);

        var kanban = context.KbTogether.Single(x => x.Barcode == "KB0000001");
        context.Weighting.Should().Contain(x => x.KbTogetherId == kanban.Id);
        context.RmBal.Should().Contain(x => x.RawMaterialBarcode == "RM001" && x.Balance > 0);
        context.ApplicationSetting.Select(x => x.Id).Should().Contain(new[] { 4, 5, 23, 24 });
        context.UsrWt.Single().PasswordHash.Should().NotBe("Password123!"); // must be hashed, not plaintext
    }

    [Fact]
    public void Seed_CalledTwice_DoesNotDuplicateData()
    {
        using var factory = new SqliteTestDbContextFactory();
        using var context = factory.CreateContext();

        DemoDataSeeder.Seed(context);
        DemoDataSeeder.Seed(context);

        context.Station.Count().Should().Be(1);
    }
}
