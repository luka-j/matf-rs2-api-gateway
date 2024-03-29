using Configurator.GrpcServices;
using Configurator.Handlers;
using Configurator.Repositories;
using Configurator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

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
    builder.Services.AddScoped<IClientGenerator, KubernetesClientGenerator>();
}
else
{
    builder.Services.AddScoped<IClientGenerator, ClientGenerator>();
}

builder.Services.AddScoped<APIGrpcService>();
builder.Services.AddScoped<RPGrpcService>();
builder.Services.AddScoped<CCOGrpcService>();
builder.Services.AddScoped<CCOService>();
builder.Services.AddScoped<ConfiguratorService>();
builder.Services.AddSingleton<SchedulerService>();

var app = builder.Build();

app.Services.GetService<SchedulerService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

if (bool.Parse(builder.Configuration["AuthSettings:UseAuth"]))
{
    app.UseMiddleware<AuthenticationHandler>("test", builder.Configuration["AuthSettings:AuthUrl"]);
}

app.MapGrpcReflectionService();
app.MapGrpcService<ConfigRetrieverService>();

app.MapControllers();

app.Run();