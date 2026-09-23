using System;

namespace OrderTracking.Domain;


public class Shipment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string Carrier { get; private set; }
    public string TrackingNumber { get; private set; }
    public DateTime EstimatedDelivery { get; private set; } = DateTime.UtcNow;


    public Shipment(Guid id, Guid orderId, string carrier, string trackingNumber, DateTime estimatedDelivery)
    {
        if (string.IsNullOrWhiteSpace(carrier))
        {
            throw new ArgumentException("Carrier cannot be null", nameof(carrier));

        }

        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            throw new ArgumentException("Tracking number cannot be null", nameof(trackingNumber));
        }

        if(estimatedDelivery <= DateTime.UtcNow)
        {
            throw new ArgumentException("Date cannot be less than current date", nameof(estimatedDelivery));
        }

        Id = id;
        OrderId = orderId;
        Carrier = carrier;
        TrackingNumber = trackingNumber;
        EstimatedDelivery = estimatedDelivery;
    }
}