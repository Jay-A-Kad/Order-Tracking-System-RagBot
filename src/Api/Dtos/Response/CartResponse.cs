namespace OrderTracking.Api.Dtos.Response;

public record CartResponse(Guid Id, string Status, List<CartItemResponse> Items);