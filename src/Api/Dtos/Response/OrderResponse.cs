using System;
using System.Collections.Generic;

namespace OrderTracking.Api.Dtos.Response;

public record OrderResponse(Guid Id, string Status, decimal TotalAmount, List<OrderItemResponse> Items);