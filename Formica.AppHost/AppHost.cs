var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var postgres = builder.AddPostgres("postgres");
var warehouseDatabase = postgres.AddDatabase("warehouse");

var apiService = builder.AddProject<Projects.Formica_ApiService>("apiservice")
    .WithReference(warehouseDatabase)
    .WaitFor(warehouseDatabase)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Formica_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
