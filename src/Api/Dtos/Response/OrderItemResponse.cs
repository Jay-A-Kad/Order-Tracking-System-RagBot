using System;

namespace OrderTracking.Api.Dtos.Response;


public record OrderItemResponse(Guid ProductId, int Quantity, decimal UnitPrice);
