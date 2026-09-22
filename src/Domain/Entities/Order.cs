using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderTracking.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public OrderStatus Status { get; private set; } = OrderStatus.Placed;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items;

    private readonly List<OrderStatusHistory> _history = new();
    public IReadOnlyList<OrderStatusHistory> History => _history;

    private Order(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    public static Order CreateFromCart(Cart cart, IEnumerable<Product> products)
    {
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, cart.CustomerId);

        foreach (var cartItem in cart.Items)
        {
            var product = products.First(p => p.Id == cartItem.ProductId);

            var orderItem = new OrderItem(
                Guid.NewGuid(),
                orderId,
                product.Id,
                cartItem.Quantity,
                product.Price
            );

            order._items.Add(orderItem);

            order.TotalAmount += orderItem.Quantity * orderItem.UnitPrice;


            
        }
        order._history.Add(new OrderStatusHistory(
                Guid.NewGuid(),
                orderId,
                order.Status,
                "Your order is placed"
            ));

        return order;
    }


    public void Cancel()
    {
        //if status is placed or processed allow cancel
        //reject for any other status

        if(Status != OrderStatus.Placed && Status != OrderStatus.Processing)
        {
            throw new ArgumentException("Cannot cancel order after it's been shipped", nameof(Status));
        }

        Status = OrderStatus.Cancelled;

         _history.Add(new OrderStatusHistory(
                Guid.NewGuid(),
                Id,
                Status,
                "Your order is cancelled"
        ));

        
    }
}
