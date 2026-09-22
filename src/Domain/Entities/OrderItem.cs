using System;

namespace OrderTracking.Domain;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice  {get; private set; }

    public OrderItem(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = ValidateQuantity(quantity);
        UnitPrice = SnapshotUnitPrice(unitPrice);
    }

    private int ValidateQuantity(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("quantity must be greater than zero.", nameof(amount));
        }
        return amount;
        
    }

    private decimal SnapshotUnitPrice(decimal unitPrice)
    {
            if(unitPrice < 0)
        {
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
        }
        return unitPrice;
    }
}