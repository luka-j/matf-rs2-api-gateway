using ApiGatewayApi;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Controllers;
using ApiGatewayApi.Processing;
using ApiGatewayApi.Services;
using Prometheus;
using Serilog;
using HttpRequester = ApiGatewayApi.Processing.HttpRequester;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ApiRepository>();
builder.Services.AddSingleton<RequestResponseFilter>();
builder.Services.AddSingleton<HttpRequester>();
builder.Services.AddSingleton<EntityMapper>();
builder.Services.AddSingleton<RequestExecutor>();
builder.Services.AddSingleton<RequestProcessorGateway>();
builder.Services.AddSingleton<ControllerUtils>();
builder.Services.AddSingleton<MetricsService>();
builder.Services.AddSingleton<ConfGateway>();
builder.Services.AddSingleton<Initializer>();

builder.Services.AddHealthChecks();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {ThreadName}: {CorrelationId} - {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var app = builder.Build();

app.Services.GetService<Initializer>(); // run config initialization

app.MapGrpcReflectionService();
app.MapGrpcService<HttpRequesterService>();
app.MapGrpcService<ConfigManagementService>();
app.MapControllers();

app.UseHttpMetrics();
app.MapMetrics();
app.MapHealthChecks("/healthz");

app.Run();