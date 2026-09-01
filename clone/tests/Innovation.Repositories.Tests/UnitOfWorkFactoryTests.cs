using FluentAssertions;
using Innovation.Core.Entities;
using Innovation.Core.UnitOfWork;
using Innovation.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Innovation.Repositories.Tests;

public class UnitOfWorkFactoryTests
{
    [Fact]
    public void CreateSiloUnitOfWork_ReturnsWorkingUnitOfWork()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var services = new ServiceCollection();
        services.AddDbContextFactory<SiloDbContext>(o => o.UseSqlite(connection));
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        var provider = services.BuildServiceProvider();

        using (var context = provider.GetRequiredService<IDbContextFactory<SiloDbContext>>().CreateDbContext())
        {
            context.Database.EnsureCreated();
        }

        var factory = provider.GetRequiredService<IUnitOfWorkFactory>();
        using var uow = factory.CreateSiloUnitOfWork();
        uow.KbTogetherRepository.Add(new KbTogether { Barcode = "KB-FACTORY", PlanId = 1, FormulationId = 1, LineId = 1, Number = 1 });
        uow.Save();

        uow.KbTogetherRepository.Find(x => x.Barcode == "KB-FACTORY").Should().NotBeNull();
    }

    // This is the concrete rebuttal to Backend ROADMAP §6.3/§7b's finding that
    // services calling a static UnitOfWorkFactory from their own constructor
    // cannot be unit tested. SampleConsumer below is constructed with a
    // substituted IUnitOfWorkFactory and never touches SQLite, a real file,
    // or any I/O at all.
    public sealed class SampleConsumer
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SampleConsumer(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public int CountPendingKanbans()
        {
            using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();
            return uow.KbTogetherRepository.GetWhere(x => x.Status == "Pending").Count();
        }
    }

    [Fact]
    public void Consumer_WithInjectedFactory_CanBeConstructedAndTested_WithoutTouchingRealDatabase()
    {
        var fakeUnitOfWork = Substitute.For<ISiloUnitOfWork>();
        var fakeKbTogetherRepository = Substitute.For<Innovation.Core.Repository.IKbTogetherRepository>();
        fakeKbTogetherRepository
            .GetWhere(Arg.Any<System.Linq.Expressions.Expression<Func<KbTogether, bool>>>())
            .Returns(new List<KbTogether>
            {
                new() { Barcode = "KB-1", Status = "Pending" },
                new() { Barcode = "KB-2", Status = "Pending" },
            }.AsQueryable());
        fakeUnitOfWork.KbTogetherRepository.Returns(fakeKbTogetherRepository);

        var fakeFactory = Substitute.For<IUnitOfWorkFactory>();
        fakeFactory.CreateSiloUnitOfWork().Returns(fakeUnitOfWork);

        var consumer = new SampleConsumer(fakeFactory);

        consumer.CountPendingKanbans().Should().Be(2);
        fakeFactory.Received(1).CreateSiloUnitOfWork();
    }
}
