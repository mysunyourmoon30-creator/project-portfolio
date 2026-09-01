using FluentAssertions;
using Innovation.Core.Entities;
using Xunit;

namespace Innovation.Repositories.Tests;

public class SiloUnitOfWorkTests : IClassFixture<SqliteTestDbContextFactory>
{
    private readonly SqliteTestDbContextFactory _factory;

    public SiloUnitOfWorkTests(SqliteTestDbContextFactory factory) => _factory = factory;

    [Fact]
    public void Save_PersistsAcrossMultipleRepositories_InSingleTransaction()
    {
        using var uow = new SiloUnitOfWork(_factory.CreateContext());
        var kanban = new KbTogether { Barcode = "KB-UOW", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1 };
        uow.KbTogetherRepository.Add(kanban);
        uow.Save();

        uow.RmBalRepository.Add(new RmBal { RawMaterialBarcode = "RM-UOW", Balance = 100m, UpdatedAt = DateTime.UtcNow });
        uow.Save();

        uow.KbTogetherRepository.Find(x => x.Barcode == "KB-UOW").Should().NotBeNull();
        uow.RmBalRepository.Find(x => x.RawMaterialBarcode == "RM-UOW").Should().NotBeNull();
    }

    [Fact]
    public void BeginTransaction_ThenRollback_DiscardsChanges()
    {
        using var uow = new SiloUnitOfWork(_factory.CreateContext());
        uow.BeginTransaction();
        uow.KbTogetherRepository.Add(new KbTogether { Barcode = "KB-ROLLBACK", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1 });
        uow.Save();

        uow.RollbackTransaction();

        uow.KbTogetherRepository.Find(x => x.Barcode == "KB-ROLLBACK").Should().BeNull();
    }

    [Fact]
    public void CheckConnection_ReturnsTrueForValidSqliteConnection()
    {
        using var uow = new SiloUnitOfWork(_factory.CreateContext());

        uow.CheckConnection().Should().BeTrue();
    }
}
