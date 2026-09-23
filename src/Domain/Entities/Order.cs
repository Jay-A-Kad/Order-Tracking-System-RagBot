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

    //order items
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items;


    //order history
    private readonly List<OrderStatusHistory> _history = new();
    public IReadOnlyList<OrderStatusHistory> History => _history;


    //mark for shipement 
    public Shipment? Shipment { get; private set; }


    private Order(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    //build order from cart
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
                "Your order is Placed"
            ));

        //once placed order sent to processing
        order.MarkAsProcessing();

        return order;
    }

    //cancel order
    public void Cancel()
    {
        //if status is placed or processed allow cancel
        //reject for any other status

        if(Status != OrderStatus.Placed && Status != OrderStatus.Processing)
        {
            throw new ArgumentException("Order cannot be cancelled after it's been shipped", nameof(Status));
        }

        Status = OrderStatus.Cancelled;

         _history.Add(new OrderStatusHistory(
                Guid.NewGuid(),
                Id,
                Status,
                "Your order is Cancelled"
        ));
    
        
    }


    //mark order for processing

    public void MarkAsProcessing()
    {
        if(Status != OrderStatus.Placed)
        {
            throw new ArgumentException("Your order has not been placed yet", nameof(Status));
        }

        Status = OrderStatus.Processing;
        _history.Add(new OrderStatusHistory(
                Guid.NewGuid(),
                Id,
                Status,
                "Your order is Processing"
        ));
    }


    //mark order for shipement
    public void MarkAsShipped(string carrier, string trackingNumber, DateTime estimatedDelivery)
    {
        if(Status != OrderStatus.Processing)
        {
            throw new ArgumentException("You order is still processing", nameof(Status));
        }

        Status = OrderStatus.Shipped;

        //fresh shipement construcuted

        Shipment = new Shipment
        (
            Guid.NewGuid(),
            Id,
            carrier,
            trackingNumber,
            estimatedDelivery
        );


        _history.Add(new OrderStatusHistory(
                Guid.NewGuid(),
                Id,
                Status,
                "Your order has Shipped"
        ));
    }


    //mark shipement out of delivery

    public void MarkAsOutForDelivery()
    {
        if(Status != OrderStatus.Shipped)
        {
            throw new ArgumentException("Your order has not been shipped yet", nameof(Status));
        }

        Status = OrderStatus.OutForDelivery;
        _history.Add(new OrderStatusHistory(
                Guid.NewGuid(),
                Id,
                Status,
                "Your order is out for delivery"
        ));
    }

    //mark out of delivery order to delivered

    public void MarkAsDelivered()
    {
        if(Status != OrderStatus.OutForDelivery)
        {
            throw new ArgumentException("You order is out for delivery", nameof(Status));
        }

        Status = OrderStatus.Delivered;
        _history.Add(new OrderStatusHistory(
                Guid.NewGuid(),
                Id,
                Status,
                "Your order has been delivered"
        ));
    }
}


