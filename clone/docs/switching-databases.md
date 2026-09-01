# Switching from SQLite to SQL Server

The demo ships on SQLite for zero-install portability. To point the same
code at SQL Server instead:

1. In `Innovation.Api`'s `appsettings.json`, replace the `ConnectionStrings:Silo`
   value with a SQL Server connection string.
2. In `Program.cs`, change `options.UseSqlite(connectionString)` to
   `options.UseSqlServer(connectionString)` (requires adding the
   `Microsoft.EntityFrameworkCore.SqlServer` package).
3. Regenerate migrations for the SQL Server provider:
   `dotnet ef migrations add InitialCreate -o Migrations` (EF Core migrations
   are provider-specific; the SQLite migration in this repo will not apply).
4. No application code changes are needed - `IUnitOfWorkFactory`,
   `SiloUnitOfWork`, and every repository work against `DbContext`
   generically and know nothing about the underlying provider.

This was verified manually during Phase 1 development, not in CI (CI has no
SQL Server instance available).
