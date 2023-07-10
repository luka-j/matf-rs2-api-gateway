using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Processing;
using ApiGatewayApi.Services;
using Serilog;

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

Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var app = builder.Build();

app.MapGrpcReflectionService();
app.MapGrpcService<HttpRequesterService>();
app.MapGrpcService<ConfigManagementService>();
app.MapControllers();

app.Run();