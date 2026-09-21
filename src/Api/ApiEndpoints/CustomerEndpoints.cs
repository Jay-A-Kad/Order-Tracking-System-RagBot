using OrderTracking.Api.Dtos;
using OrderTracking.Domain;
using OrderTracking.Infrastructure;

namespace OrderTracking.Api.ApiEndpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {

        var customers  = app.MapGroup("/customers");
    
    //create guest customer
        customers.MapPost("/guest", async ( AppDbContext dbContext) =>
        {
            var createGuest = Customer.CreateGuest();
            await dbContext.Customers.AddAsync(createGuest);

            await dbContext.SaveChangesAsync();
            return Results.Created($"/customers/{createGuest.Id}", new CustomerResponse (createGuest.Id));
        });

    }
}


