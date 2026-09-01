using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Innovation.Data;

// Lets `dotnet ef migrations add` run without a full DI host.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SiloDbContext>
{
    public SiloDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SiloDbContext>();
        optionsBuilder.UseSqlite("Data Source=design-time.db");
        return new SiloDbContext(optionsBuilder.Options);
    }
}
