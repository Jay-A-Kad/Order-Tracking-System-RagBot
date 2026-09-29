namespace OrderTracking.Api.ApiEndpoints;
using Microsoft.SemanticKernel;
using OrderTracking.Infrastructure;
using OrderTracking.Api.Dtos.Request;
using OrderTracking.AI;
using ChatResponse = OrderTracking.Api.Dtos.Response.ChatResponse;
using Azure.Search.Documents;
using Microsoft.Extensions.AI;

public static class ChatEndpoints
{
    public static void MapToChatEndpoint(this WebApplication app)
    {
        app.MapPost("/customers/{customerId}/chat", 
            async(Guid customerId, ChatRequest request, AppDbContext dbContext, Kernel kernel, SearchClient searchClient) =>
        {
            //1-- find customer if missing 404
            var getCustomer = await dbContext.Customers.FindAsync(customerId);
            if(getCustomer is null) return Results.NotFound();



            var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            
            //inject plugins to the kernerl
            kernel.Plugins.AddFromObject(new KnowledgeBasePlugin(searchClient, embeddingGenerator));
            kernel.Plugins.AddFromObject(new OrderPlugin(dbContext,customerId));
            kernel.Plugins.AddFromObject(new InventoryPlugin(dbContext));

            //invoking the prompt
            var settings = new PromptExecutionSettings{FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()};
            var result = await kernel.InvokePromptAsync(request.Message, new(settings));

            return Results.Ok(new ChatResponse(result.ToString()));

        });
    }
}