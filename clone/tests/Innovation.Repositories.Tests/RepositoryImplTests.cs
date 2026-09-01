using FluentAssertions;
using Innovation.Core.Entities;
using Xunit;

namespace Innovation.Repositories.Tests;

// RepositoryImpl<T> is generic and identical for every entity (Backend
// ROADMAP §4.1), so its contract only needs proving once. KbTogether is used
// here because it carries the Number field the business rules branch on.
public class RepositoryImplTests : IClassFixture<SqliteTestDbContextFactory>
{
    private readonly SqliteTestDbContextFactory _factory;

    public RepositoryImplTests(SqliteTestDbContextFactory factory) => _factory = factory;

    [Fact]
    public void Add_ThenGetById_ReturnsSameEntity()
    {
        using var context = _factory.CreateContext();
        var repo = new RepositoryImpl<KbTogether>(context);
        var kanban = new KbTogether { Barcode = "KB-ADD", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1 };

        repo.Add(kanban);
        context.SaveChanges();

        var fetched = repo.Get(kanban.Id);
        fetched.Should().NotBeNull();
        fetched!.Barcode.Should().Be("KB-ADD");
    }

    [Fact]
    public void GetWhere_FiltersCorrectly()
    {
        using var context = _factory.CreateContext();
        var repo = new RepositoryImpl<KbTogether>(context);
        repo.Add(new KbTogether { Barcode = "KB-FILTER-A", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1 });
        repo.Add(new KbTogether { Barcode = "KB-FILTER-B", PlanId = 2, FormulationId = 1, LineId = 1, Number = 1 });
        context.SaveChanges();

        var results = repo.GetWhere(x => x.PlanId == 2).ToList();

        results.Should().ContainSingle(x => x.Barcode == "KB-FILTER-B");
    }

    [Fact]
    public void Update_PersistsChanges_AfterSave()
    {
        using var writeContext = _factory.CreateContext();
        var writeRepo = new RepositoryImpl<KbTogether>(writeContext);
        var kanban = new KbTogether { Barcode = "KB-UPDATE", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1, Status = "Pending" };
        writeRepo.Add(kanban);
        writeContext.SaveChanges();

        kanban.Status = "Completed";
        writeRepo.Update(kanban);
        writeContext.SaveChanges();

        using var readContext = _factory.CreateContext();
        var readRepo = new RepositoryImpl<KbTogether>(readContext);
        readRepo.Get(kanban.Id)!.Status.Should().Be("Completed");
    }

    [Fact]
    public void Delete_RemovesEntity_AfterSave()
    {
        using var context = _factory.CreateContext();
        var repo = new RepositoryImpl<KbTogether>(context);
        var kanban = new KbTogether { Barcode = "KB-DELETE", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1 };
        repo.Add(kanban);
        context.SaveChanges();

        repo.Delete(kanban);
        context.SaveChanges();

        repo.Get(kanban.Id).Should().BeNull();
    }

    [Fact]
    public void GetAll_ReturnsNoTrackingResults()
    {
        using var context = _factory.CreateContext();
        var repo = new RepositoryImpl<KbTogether>(context);
        repo.Add(new KbTogether { Barcode = "KB-NOTRACK", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1 });
        context.SaveChanges();

        var all = repo.GetAll().ToList();

        all.Should().NotBeEmpty();
        context.Entry(all.First()).State.Should().Be(Microsoft.EntityFrameworkCore.EntityState.Detached);
    }

    [Fact]
    public void Add_WeighingWithDecimalWeight_PreservesPrecisionAfterRoundTrip()
    {
        using var context = _factory.CreateContext();
        var repo = new RepositoryImpl<Weighting>(context);
        var weighing = new Weighting { KbTogetherId = 1, StepNo = 1, RawMaterialCode = "RM001", TargetWeight = 12.3456m };

        repo.Add(weighing);
        context.SaveChanges();

        repo.Get(weighing.Id)!.TargetWeight.Should().Be(12.3456m);
    }
}

// Proves every one of the 16 DbSets is correctly mapped and queryable -
// a schema-level smoke test complementing the full CRUD contract above.
public class AllEntitiesMappingTests : IClassFixture<SqliteTestDbContextFactory>
{
    private readonly SqliteTestDbContextFactory _factory;

    public AllEntitiesMappingTests(SqliteTestDbContextFactory factory) => _factory = factory;

    [Fact]
    public void AllSixteenDbSets_CanBeQueried_WithoutThrowing()
    {
        using var context = _factory.CreateContext();

        var act = () =>
        {
            _ = context.KbTogether.ToList();
            _ = context.Weighting.ToList();
            _ = context.TotalWeight.ToList();
            _ = context.TwAcceptWeightHis.ToList();
            _ = context.SendStepParameter.ToList();
            _ = context.Station.ToList();
            _ = context.UsrWt.ToList();
            _ = context.TrayPlan.ToList();
            _ = context.TrayWeight.ToList();
            _ = context.TrayBarcode.ToList();
            _ = context.TypeTray.ToList();
            _ = context.RmBal.ToList();
            _ = context.SiloApprove.ToList();
            _ = context.OnHand.ToList();
            _ = context.ProdstdMixtemp.ToList();
            _ = context.ApplicationSetting.ToList();
        };

        act.Should().NotThrow();
    }
}
