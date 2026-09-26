using System;
using OrderTracking.Infrastructure;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OrderTracking.AI;

public class OrderPlugin
{
    private readonly AppDbContext _dbContext;
    private readonly Guid _customerId;

    public OrderPlugin(AppDbContext dbContext, Guid customerId)
    {
        _dbContext = dbContext;
        _customerId = customerId;
    }

    //list user recent orders

    [KernelFunction("list_my_orders")]
    [Description("This method list the customer orders by their status and total amount")]
    public async Task<string> ListMyOrdersAsync()
    {
        var listOrders = await _dbContext.Orders.Where(x => x.CustomerId == _customerId)
            .OrderBy(o => o.CreatedAt)
            .Take(10)
            .ToListAsync();

        //string for summary
        var summaryString = string.Empty;

        //loop for all the orders and building a summary
        foreach(var order in listOrders)
        {
            summaryString += $"Order: {order.Id} : {order.Status} : {order.TotalAmount} \n";
        }

        return summaryString.Length == 0 ? "This customer has no order yet" : summaryString;
    }


    //get statusof the recent orders
    [KernelFunction("get_order_status")]
    [Description("Gets the current status and latest tracking update for a customer's order by order ID")]
     public async Task<string> GetMyOrderStatusAsync([Description("The order ID, e.g. 123e4567)")] Guid orderId)
    {
        var getOrderStatus = await _dbContext.Orders
            .Include(o => o.Shipment)
            .FirstOrDefaultAsync(x => x.Id == orderId && x.CustomerId == _customerId);
            
        if(getOrderStatus is null)
        {
            return "No Order Found for this OrderId";
        }
        else
        {
            return getOrderStatus.Shipment is null
            ? "The order has not been shipped yet"
            : $"Order : {orderId} : {getOrderStatus.Status} : {getOrderStatus.LastStatusChangedAt}, Shipment : {getOrderStatus.Shipment.Carrier} : {getOrderStatus.Shipment.TrackingNumber} : {getOrderStatus.Shipment.EstimatedDelivery}";
    }
        
        }


        
}