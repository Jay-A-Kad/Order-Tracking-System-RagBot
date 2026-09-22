using System.ComponentModel.DataAnnotations;


namespace OrderTracking.Domain;

public class Product
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }

    [Timestamp]
    public byte[] RowVersion { get; private set; } = default!;


    public Product(Guid id, string sku, string name, decimal price, int stockQuantity)
    {
        if(string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("sku cannot be empty", nameof(sku));
        }
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("name cannot be empty",nameof(name));
        }
        if(price < 0)
        {
            throw new ArgumentException("price cannot be negative", nameof(price));
        }
        if(stockQuantity < 0)
        {
            throw new ArgumentException("stock quantity cannot be negative", nameof(stockQuantity));
        }


        Id = id;
        Sku = sku;
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void DecreaseStock(int quantity)
    {
        if(quantity > StockQuantity)
        {
            throw new InvalidOperationException("Insufficient stock available.");
        }
        StockQuantity -= quantity;
    }

}

