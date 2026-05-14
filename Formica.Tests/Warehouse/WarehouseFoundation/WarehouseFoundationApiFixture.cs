using Aspire.Hosting;
using Formica.ApiService.Warehouse.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Formica.Tests.Warehouse.WarehouseFoundation;

public sealed class WarehouseFoundationApiFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
    private DistributedApplication? _app;
    private string? _connectionString;

    public HttpClient ApiClient { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Formica_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        await _app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        await _app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        ApiClient = _app.CreateHttpClient("apiservice");

        await _app.ResourceNotifications.WaitForResourceHealthyAsync("warehouse", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        _connectionString = await _app.GetConnectionStringAsync("warehouse", cancellationToken)
            .AsTask()
            .WaitAsync(DefaultTimeout, cancellationToken);

        await using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    public WarehouseDbContext CreateDbContext()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Warehouse persistence fixture has not been initialized.");
        }

        var options = new DbContextOptionsBuilder<WarehouseDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new WarehouseDbContext(options);
    }

    public async ValueTask DisposeAsync()
    {
        ApiClient?.Dispose();

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}
