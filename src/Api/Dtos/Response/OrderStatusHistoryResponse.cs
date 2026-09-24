using System;

namespace OrderTracking.Api.Dtos.Response;

public record OrderStatusHistoryResponse(string Status, string? Note, DateTime ChangedAt);