using Configurator.GrpcServices;
using Configurator.Handlers;
using Configurator.Repositories;
using Configurator.Services;
using k8s;
using k8s.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

if (bool.Parse(builder.Configuration["GitHubSettings:UseGitHub"]))
{
    builder.Services.AddScoped<IConfigRepository, GitHubConfigRepository>();
}
else
{
    builder.Services.AddScoped<IConfigRepository, DirectoryConfigRepository>();
}

if (bool.Parse(builder.Configuration["UseKubernetes"]))
{
    var config = KubernetesClientConfiguration.InClusterConfig();
    var client = new Kubernetes(config);
    builder.Services.AddScoped<IClientNameService, KubernetesClientNameService>();
    var APIPort = builder.Configuration["APIPort"];

    string namespaceName = "api-gateway";

    V1PodList podList = client.ListNamespacedPod(namespaceName, labelSelector: "app:api");
    foreach (V1Pod pod in podList.Items)
    {
        var name = pod.Metadata.Name;
        var URI = pod.Status.PodIP + ":" + APIPort;
        builder.Services.AddGrpcClient<ApiGatewayApi.ConfigManagement.ConfigManagementClient>(name, op => op.Address = new Uri(URI));
    }
}
else
{
    builder.Services.AddScoped<IClientNameService, DefaultClientNameService>();
    builder.Services.AddGrpcClient<ApiGatewayApi.ConfigManagement.ConfigManagementClient>("API",
                options => options.Address = new Uri(builder.Configuration["GrpcSettings:APIURL"]));
}

builder.Services.AddScoped<APIGrpcService>();
builder.Services.AddScoped<ConfiguratorService>();
builder.Services.AddSingleton<SchedulerService>();

var app = builder.Build();

app.Services.GetService<SchedulerService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));
}

app.UseMiddleware<AuthenticationHandler>("test", builder.Configuration["AuthSettings:AuthUrl"]);

app.MapControllers();

app.Run();