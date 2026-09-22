using OrderTracking.Domain;
using Xunit;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;

namespace OrderTracking.UnitTests;

public class ProductTests
{

    [Theory]
    [InlineData(10, 10, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(10, 5, 5)]
    public void Product_ShouldDescreaseStock_WhenBuyingExactStock( int stockQuantity ,int quantity ,int expected)
    {
    //arrange
        var product = new Product(Guid.NewGuid(), "SKU-123", "Test-Product", 9.99m, stockQuantity);
    //act 
        product.DecreaseStock(quantity);
    //asert
        Assert.Equal(expected, product.StockQuantity);
    

    }

    //DecreaseStock throws InvalidOperationException when quantity > StockQuantity
    [Theory]
    [InlineData(10,100)]
    [InlineData(0,1)]
    public void Product_ShouldThrowInvalidOperation_WhenQuantityGreaterThanStockQuantity(int stockQuantity, int quantity)
    {
    
       var Product =  new Product(Guid.NewGuid(), "SKU-123", "Test-Product", 9.99m, stockQuantity);

        var exception = Assert.Throws<InvalidOperationException>(
            () => Product.DecreaseStock(quantity)
        );

        Assert.Equal("Insufficient stock available.", exception.Message);
    }

    //Constructor throws ArgumentException for negative Price
    [Theory]
    [InlineData(-1.0)]
    [InlineData(-0.01)]
    [InlineData(-0.000001)]
    [InlineData(-99)]
    public void Products_ShouldThrowArgumentException_WhenNegativePrice(decimal price)
    {
    
    

        var exception = Assert.Throws<ArgumentException>(
            () => new Product(Guid.NewGuid(), "SKU-123", "Test-Product", price, 10)
        );

        Assert.Equal("price", exception.ParamName);
    }



    //Constructor throws ArgumentException for empty Sku

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Products_ShouldThrowArgumentException_WhenEmptySku(string sku)
    {
    
     
        var exception = Assert.Throws<ArgumentException>(
            () => new Product(Guid.NewGuid(), sku, "Test-Product", 10.0m, 10)
        );

        Assert.Equal("sku", exception.ParamName);
    }

    //Constructor throws ArgumentException for negative StockQuantity


    [Theory]
    [InlineData(-1)]
    [InlineData(-9999)]
    [InlineData(int.MinValue)]
    public void Products_ShouldThrowArgumentException_WhenNegativeStockQuantity(int stockQuantity)
    {
    

        var exception = Assert.Throws<ArgumentException>(
            () => new Product(Guid.NewGuid(), "sku-123", "Test-Product", 10.0m, stockQuantity)
        );

        Assert.Equal("stockQuantity", exception.ParamName);
    }


    //Constructor succeeds and correctly sets all fields for valid input

    [Fact]
     public void Products_ShouldSucceed_WhenAllCorrectFeildsSet()
    {
    
        var id = Guid.NewGuid();
        var product = new Product(id, "sku-123", "Test Product", 19.99m, 10);

        Assert.Equal(id,product.Id);
        Assert.Equal("sku-123",product.Sku);
        Assert.Equal("Test Product",product.Name);
        Assert.Equal(19.99m,product.Price);
        Assert.Equal(10,product.StockQuantity);
    }

}
