using Microsoft.EntityFrameworkCore;
using OrderTracking.Api.ApiEndpoints;
using OrderTracking.Infrastructure;
using OrderTracking.Worker;
using Microsoft.SemanticKernel;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//app db context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb")));

//worker services
builder.Services.AddHostedService<ShipmentSimulatorService>();

//AI services
builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["AzureOpenAI:ChatDeployment"]!,
        endpoint: builder.Configuration["AzureOpenAI:Endpoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapCustomerEndpoints();
app.MapProductEndpoints();
app.MapCartEndpoint();
app.MapOrderEndpoints();
app.MapToChatEndpoint();

app.Run();
