using Microsoft.EntityFrameworkCore;
using OrderTracking.Api.ApiEndpoints;
using OrderTracking.Infrastructure;
using OrderTracking.Worker;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.AI;
using System.Net;
using Azure;
using Azure.Search.Documents;

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
 #pragma warning disable SKEXP0010
builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["AzureOpenAI:ChatDeployment"]!,
        endpoint: builder.Configuration["AzureOpenAI:Endpoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!)
   
    .AddAzureOpenAIEmbeddingGenerator(
        builder.Configuration["AzureOpenAI:EmbeddingDeployment"]!,
          builder.Configuration["AzureOpenAI:Endpoint"]!,
          builder.Configuration["AzureOpenAI:ApiKey"]!);
    #pragma warning restore SKEXP0010
    
//registerring client list
  builder.Services.AddSingleton(new SearchClient(
      new Uri(builder.Configuration["AzureSearch:Endpoint"]!),
      "policy-docs-index",
      new AzureKeyCredential(builder.Configuration["AzureSearch:AdminKey"]!)));

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
