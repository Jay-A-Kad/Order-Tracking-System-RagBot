namespace OrderTracking.Domain;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "Guest";
    public string? Email { get; private set; }

    private Customer()
    {
        
    }

    public static Customer CreateGuest()
{
    return new Customer
    {
        Id = Guid.NewGuid(),
    };
}
}




