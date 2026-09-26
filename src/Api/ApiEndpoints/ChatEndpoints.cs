namespace OrderTracking.Api.ApiEndpoints;
using Microsoft.SemanticKernel;
using OrderTracking.Infrastructure;
using OrderTracking.Api.Dtos.Request;
using OrderTracking.AI;
using OrderTracking.Api.Dtos.Response;

public static class ChatEndpoints
{
    public static void MapToChatEndpoint(this WebApplication app)
    {
        app.MapPost("/customers/{customerId}/chat", 
            async(Guid customerId, ChatRequest request, AppDbContext dbContext, Kernel kernel) =>
        {
            //1-- find customer if missing 404
            var getCustomer = await dbContext.Customers.FindAsync(customerId);
            if(getCustomer is null) return Results.NotFound();

            //inject plugins to the kernerl
            kernel.Plugins.AddFromObject(new OrderPlugin(dbContext,customerId));
            kernel.Plugins.AddFromObject(new InventoryPlugin(dbContext));

            //invoking the prompt
            var settings = new PromptExecutionSettings{FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()};
            var result = await kernel.InvokePromptAsync(request.Message, new(settings));

            return Results.Ok(new ChatResponse(result.ToString()));

        });
    }
}