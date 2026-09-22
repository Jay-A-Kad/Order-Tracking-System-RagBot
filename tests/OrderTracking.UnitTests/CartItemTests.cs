using OrderTracking.Domain;

namespace OrderTracking.UnitTests;

public class CartItemTests
{
    [Fact]
    public void Constructor_ShouldSetAllFields_WhenGivenValidInput()
    {
        var id = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var item = new CartItem(id, cartId, productId, 3);

        Assert.Equal(id, item.Id);
        Assert.Equal(cartId, item.CartId);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(3, item.Quantity);
    }



    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-9999)]
    public void Constructor_ShouldThrowArgumentException_WhenQuantityIsNotPositive(int quantity)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new CartItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), quantity)
        );

        Assert.Equal("amount", exception.ParamName);
    }




    [Fact]
    public void IncreaseQuantity_ShouldAddToExistingQuantity_WhenAmountIsPositive()
    {
        var item = new CartItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3);

        item.IncreaseQuantity(2);

        Assert.Equal(5, item.Quantity);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseQuantity_ShouldThrowArgumentException_WhenAmountIsNotPositive(int amount)
    {
        var item = new CartItem(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3);

        var exception = Assert.Throws<ArgumentException>(
            () => item.IncreaseQuantity(amount)
        );

        Assert.Equal("amount", exception.ParamName);
    }
}
