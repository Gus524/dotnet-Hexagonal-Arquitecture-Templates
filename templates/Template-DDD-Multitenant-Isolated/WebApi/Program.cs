using IAM.Assembly;
using SharedKernel.Mediator;
using WebApi.Extensions.Bootstrap;
using WebApi.Extensions.DependencyInjection;
using WebApi.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

var assemblies = new[]
{
    typeof(IamApplicationAssembly).Assembly,
    typeof(IRequest<>).Assembly
};

builder.Services.AddApplicationLayer(assemblies);
builder.Services.AddPresentationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.ConfigureOptions(builder.Configuration);

var app = builder.Build();

await app.InitializeDatabasesAsync();

app.ConfigureRequestPipeline();

await app.RunAsync();