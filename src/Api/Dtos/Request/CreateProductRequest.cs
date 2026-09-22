namespace OrderTracking.Api.Dtos.Request;

public record CreateProductRequest(string Sku, string Name, decimal Price, int StockQuantity);