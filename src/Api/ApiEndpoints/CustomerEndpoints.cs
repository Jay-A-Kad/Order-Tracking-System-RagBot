using System;
using System.ComponentModel;
using System.IO.Pipelines;
using Microsoft.VisualBasic;
using OrderTracking.Api.Dtos.Response;
using OrderTracking.Domain;
using OrderTracking.Infrastructure;
using Microsoft.EntityFrameworkCore;

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


        //customer checkout
        customers.MapPost("/{customerId}/checkout", async(Guid customerId, AppDbContext dbContext) =>
        {
            //1-- find customer if missing 404
            var getCustomer = await dbContext.Customers.FindAsync(customerId);
            if(getCustomer is null) return Results.NotFound();
        
        //2-- find customer active cart if missing 404
            var getCart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Status == CartStatus.Active);

            if(getCart is null)
            {
                return Results.NotFound();
            }

        //3--load product using query filtering where id is in carts product id
            var productIds = getCart.Items
                .Select(i => i.ProductId)
                .ToList();

            var loadProducts = await dbContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            //4-- for each item call product.decrease stock in try catch
            try
            {
                foreach(var item in getCart.Items)
                {
                    var product = loadProducts.First(p =>p.Id == item.ProductId);
                    product.DecreaseStock(item.Quantity);
                }
            }
            catch(InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            
        //5-- call createfrom cart to build the cart 

            var currentOrder = Order.CreateFromCart(getCart, loadProducts);

        //6-- call checkout
            getCart.CheckOut();

        //7--add new order to dbcontext.order
            dbContext.Orders.Add(currentOrder);
            //8--savechange async
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                return Results.Conflict(ex.Message);
            }
            
        //9-- result.conflict
            var orderItems = currentOrder.Items
                .Select(x => new OrderItemResponse(x.ProductId,x.Quantity,x.UnitPrice))
                .ToList();

            return Results.Created($"/customers/{customerId}/checkout", new OrderResponse( currentOrder.Id, currentOrder.Status.ToString(), currentOrder.TotalAmount, orderItems));
        });
        

    }
}


