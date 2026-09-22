namespace OrderTracking.Domain;

public enum OrderStatus
{
    Placed,
    Processing,
    Shipped,
    OutForDelivery,
    Delivered,
    Cancelled
}
