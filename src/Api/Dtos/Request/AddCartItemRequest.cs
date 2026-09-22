using System.ComponentModel;

namespace OrderTracking.Api.Dtos.Request;

public record AddCartItemsRequest(Guid ProductId, int Quantity);