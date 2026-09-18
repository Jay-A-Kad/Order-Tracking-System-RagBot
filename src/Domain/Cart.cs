namespace OrderTracking.Domain;


public enum CartStatus
{
    Active,
    CheckedOut,
    Abandoned
}

public class Cart
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public CartStatus Status { get; private set; } = CartStatus.Active;

    private readonly List<CartItem> _items = new();
    public IReadOnlyList<CartItem> Items => _items;
    
    public Cart(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    public void AddItem(CartItem item)
    {
        if (Status != CartStatus.Active)
        {
            throw new InvalidOperationException("Cannot add items to a cart that is not active.");
        }
        
        if (item.CartId != Id)
        {
            throw new InvalidOperationException("Item does not belong to this cart.");
        }

        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(item.Quantity);
        }
        else
        {
            _items.Add(item);
        }
    }
  
}