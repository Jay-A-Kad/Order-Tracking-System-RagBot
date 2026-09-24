using System;
using System.ComponentModel;
using System.IO.Pipelines;
using Microsoft.VisualBasic;
using OrderTracking.Api.Dtos.Response;
using OrderTracking.Domain;
using OrderTracking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;


namespace OrderTracking.Api.ApiEndpoints;


public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var order  = app.MapGroup("/customers");

        order.MapGet("/{customerId}/orders/{orderId}", 
            async (Guid customerId, Guid orderId, AppDbContext dbContext) => {

                var orders = await dbContext.Orders
                    .Include(i => i.Items)
                    .Include(i => i.History)
                    .Include(i => i.Shipment)
                    .FirstOrDefaultAsync(x => x.Id == orderId && x.CustomerId == customerId);

                if(orders is null){
                    return Results.NotFound();
                }



                var orderItemResponse = orders.Items
                    .Select(x => new OrderItemResponse(x.ProductId, x.Quantity, x.UnitPrice))
                    .ToList();

                  
                
                var orderStatusHistoryResponse = orders.History
                    .Select(x => new OrderStatusHistoryResponse(x.Status.ToString(), x.Note, x.ChangedAt))
                    .ToList();


                var orderShipmentResponse = orders.Shipment is not null 
                    ? new ShipmentResponse(orders.Shipment.Carrier, orders.Shipment.TrackingNumber, orders.Shipment.EstimatedDelivery)
                    :  null;


                var orderDetailedReponse =  new OrderDetailResponse(
                        orders.Id, orders.Status.ToString(), orders.TotalAmount, orders.CreatedAt, 
                        orderItemResponse,orderStatusHistoryResponse, orderShipmentResponse);
                

                return Results.Ok(orderDetailedReponse);
            });
    }
}