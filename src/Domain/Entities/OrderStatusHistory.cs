using System;

namespace OrderTracking.Domain;

public class OrderStatusHistory
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? Note { get; private set; }
    public DateTime ChangedAt { get; private set; } = DateTime.UtcNow;



    public OrderStatusHistory(Guid id, Guid orderId, OrderStatus status, string? note)
    {
        Id = id;
        OrderId = orderId;
        Status = status;
        Note = note;
    }
}