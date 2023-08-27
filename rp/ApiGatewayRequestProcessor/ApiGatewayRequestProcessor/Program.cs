using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<ConfigRepository>();
builder.Services.AddSingleton<ApiGateway>();
builder.Services.AddSingleton<CcoGateway>();

Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {ThreadName}: {CorrelationId} - {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var app = builder.Build();

app.MapGrpcReflectionService();
app.MapGrpcService<ConfigManagementService>();
app.MapGrpcService<RequestProcessorService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();