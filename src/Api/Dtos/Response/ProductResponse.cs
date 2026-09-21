namespace OrderTracking.Api.Dtos.Response;

public record ProductResponse(Guid id, string Sku, string Name, decimal Price, int StockQuantity);