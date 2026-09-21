using Microsoft.EntityFrameworkCore;
using OrderTracking.Api.Dtos.Request;
using OrderTracking.Api.Dtos.Response;
using OrderTracking.Domain;
using OrderTracking.Infrastructure;

namespace OrderTracking.Api.ApiEndpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoint(this WebApplication app)
    {

        app.MapPost("/customers/{customerId}/cart/items", async (Guid customerId, AddCartItemsRequest request, AppDbContext dbContext) =>
        {
            var customer = await dbContext.Customers.FindAsync(customerId);
            if(customer is null) return Results.NotFound();
            
            var product = await dbContext.Products.FindAsync(request.ProductId);
            if(product is null) return Results.NotFound();

            var cart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Status == CartStatus.Active);
                
            if(cart is null)
            {
                cart  = new Cart(Guid.NewGuid(), customerId);
                _ = dbContext.Carts.Add(cart);
            }

            try
            {
                cart.AddItem(product, request.Quantity);
            }
            catch(InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            
            await dbContext.SaveChangesAsync();
            var itemResponse = cart.Items.Select(x =>
                new CartItemResponse(x.ProductId, x.Quantity))
                .ToList();

            return Results.Ok(new CartResponse(cart.Id, cart.Status.ToString(), itemResponse));
        });
    }
}