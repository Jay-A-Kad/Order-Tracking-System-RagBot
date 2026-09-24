using System;
using System.Collections.Generic;

namespace OrderTracking.Api.Dtos.Response;

public record OrderDetailResponse(Guid Id, string Status,decimal TotalAmount, DateTime CreatedAt, List<OrderItemResponse> Items, List<OrderStatusHistoryResponse> History, ShipmentResponse? Shipment);