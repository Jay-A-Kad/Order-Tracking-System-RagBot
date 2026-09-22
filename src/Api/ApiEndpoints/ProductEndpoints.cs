using OrderTracking.Api.Dtos.Request;
using OrderTracking.Api.Dtos.Response;
using OrderTracking.Domain;
using OrderTracking.Infrastructure;

namespace OrderTracking.Api.ApiEndpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var products = app.MapGroup("/products");

        //create product request
        products.MapPost("/", async (AppDbContext dbContext, CreateProductRequest request) =>
        {
            try
            {
                var createProduct = new Product
                (
                    Guid.NewGuid(),
                    request.Sku,
                    request.Name,
                    request.Price,
                    request.StockQuantity

                );

                await dbContext.Products.AddAsync(createProduct);

                    await dbContext.SaveChangesAsync();
                    return Results.Created($"/products/{createProduct.Id}", new ProductResponse (createProduct.Id, createProduct.Sku, createProduct.Name,createProduct.Price,createProduct.StockQuantity));
            }
            catch(ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
       
        });
    }
}