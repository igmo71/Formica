using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Formica.ApiService.Warehouse.Persistence;

public sealed class WarehouseDbContextFactory : IDesignTimeDbContextFactory<WarehouseDbContext>
{
    private const string ConnectionStringName = "warehouse";

    public WarehouseDbContext CreateDbContext(string[] args)
    {
        string? connectionString =
            Environment.GetEnvironmentVariable($"ConnectionStrings__{ConnectionStringName}")
            ?? Environment.GetEnvironmentVariable("FORMICA_WAREHOUSE_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Design-time warehouse connection string is not configured. " +
                "Set ConnectionStrings__warehouse or FORMICA_WAREHOUSE_CONNECTION before running dotnet ef.");
        }

        var options = new DbContextOptionsBuilder<WarehouseDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new WarehouseDbContext(options);
    }
}
