using Microsoft.EntityFrameworkCore;

namespace Formica.ApiService.Warehouse.Persistence;

public static class WarehousePersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddWarehousePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WarehouseDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("warehouse");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Warehouse database connection string 'warehouse' is not configured.");
            }

            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
