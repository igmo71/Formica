using Microsoft.EntityFrameworkCore;

namespace Formica.ApiService.Warehouse.Persistence;

public sealed class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WarehouseDbContext).Assembly,
            type => type.Namespace?.Contains(".Warehouse.Persistence.Configurations.", StringComparison.Ordinal) == true);
    }
}
