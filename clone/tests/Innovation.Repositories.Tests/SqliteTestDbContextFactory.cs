using Innovation.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Innovation.Repositories.Tests;

// Each test gets an isolated in-memory SQLite database, kept alive for the
// test's lifetime by holding one open connection (SQLite's in-memory mode
// destroys the DB when the last connection closes).
public sealed class SqliteTestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    public DbContextOptions<SiloDbContext> Options { get; }

    public SqliteTestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<SiloDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new SiloDbContext(Options);
        context.Database.EnsureCreated();
    }

    public SiloDbContext CreateContext() => new(Options);

    public void Dispose() => _connection.Dispose();
}
