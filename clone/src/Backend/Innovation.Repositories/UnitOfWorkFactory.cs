using Innovation.Core.UnitOfWork;
using Innovation.Data;
using Microsoft.EntityFrameworkCore;

namespace Innovation.Repositories;

// The Phase 1 inversion: an ordinary injectable class (not a static one),
// registered as services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>().
// Uses IDbContextFactory<SiloDbContext> rather than holding a single
// DbContext directly, since a factory-of-factories composes cleanly with
// both ASP.NET Core's per-request DbContext lifetime and desktop code that
// may need to create contexts outside any request scope.
public sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory<SiloDbContext> _dbContextFactory;

    public UnitOfWorkFactory(IDbContextFactory<SiloDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public ISiloUnitOfWork CreateSiloUnitOfWork() =>
        new SiloUnitOfWork(_dbContextFactory.CreateDbContext());
}
