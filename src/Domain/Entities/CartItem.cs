namespace OrderTracking.Domain;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    public CartItem(Guid id, Guid cartId, Guid productId, int quantity)
    {
        Id = id;
        CartId = cartId;
        ProductId = productId;
        IncreaseQuantity(quantity);
    }
    
    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        }
        Quantity += amount;
        
    }
    
}

