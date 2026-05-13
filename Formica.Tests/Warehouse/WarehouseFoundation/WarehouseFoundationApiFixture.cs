using Aspire.Hosting;
using Microsoft.Extensions.Logging;

namespace Formica.Tests.Warehouse.WarehouseFoundation;

public sealed class WarehouseFoundationApiFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    private DistributedApplication? _app;

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
