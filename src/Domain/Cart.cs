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

    public void AddItem(Product product, int quantity)
    {
        if (Status != CartStatus.Active)
        {
            throw new InvalidOperationException("Cannot add items to a cart that is not active.");
        }

        // if (quantity <= 0)
        // {
        //     throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        // }

        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var newItem = new CartItem(Guid.NewGuid(), Id, product.Id, quantity);
            _items.Add(newItem);
        }
    }
  
}       
        
    
  
