using System;

namespace OrderTracking.Api.Dtos.Response;

public record ShipmentResponse(string Carrier, string TrackingNumber, DateTime EstimatedDelivery);