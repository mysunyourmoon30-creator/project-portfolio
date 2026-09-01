using Innovation.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Innovation.Api.Tests;

// Boots the real API pipeline (auth, controllers, exception mapping) against
// an isolated in-memory SQLite database seeded with DemoDataSeeder, per test
// class instance.
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public CustomWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<SiloDbContext>>();
            services.RemoveAll<IDbContextFactory<SiloDbContext>>();
            services.AddDbContextFactory<SiloDbContext>(options => options.UseSqlite(_connection));

            // Program.cs's own startup block (Migrate() + DemoDataSeeder.Seed())
            // runs against this same overridden factory once the host builds -
            // no separate seeding needed here.
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
