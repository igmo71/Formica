using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.Persistence;

public sealed class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
{
    public DbSet<WarehouseEntity> Warehouses => Set<WarehouseEntity>();

    public DbSet<LocationAddressRules> LocationAddressRules => Set<LocationAddressRules>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WarehouseDbContext).Assembly,
            type => type.Namespace?.Contains(".Warehouse.Persistence.Configurations.", StringComparison.Ordinal) == true);
    }
}
