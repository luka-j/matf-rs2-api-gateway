using Configurator.GrpcServices;
using Configurator.Repositories;
using Configurator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

if (bool.Parse(builder.Configuration["GitHubSettings:UseGitHub"]))
{
    builder.Services.AddSingleton<IConfigRepository, GitHubConfigRepository>();
} else
{
    builder.Services.AddSingleton<IConfigRepository, DirectoryConfigRepository>();
}

builder.Services.AddGrpcClient<ApiGatewayApi.ConfigManagement.ConfigManagementClient>(
                options => options.Address = new Uri(builder.Configuration["GrpcSettings:APIURL"]));
builder.Services.AddSingleton<APIGrpcService>();
builder.Services.AddSingleton<ConfiguratorService>();

var app = builder.Build();

app.Services.GetService<ConfiguratorService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();