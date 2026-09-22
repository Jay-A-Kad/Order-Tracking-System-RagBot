using OrderTracking.Domain;

namespace OrderTracking.UnitTests;

public class CartTests
{

    private static Product CreateProduct(int stockQuantity = 100) =>
        new(Guid.NewGuid(), "sku-123", "Test Product", 9.99m, stockQuantity);

    [Fact]
    public void Constructor_ShouldDefaultToActiveStatus()
    {
        var cart = new Cart(Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(CartStatus.Active, cart.Status);
    }




    [Fact]
    public void AddItem_ShouldCreateNewCartItem_WhenProductNotAlreadyInCart()
    {
        var cart = new Cart(Guid.NewGuid(), Guid.NewGuid());
        var product = CreateProduct();

        cart.AddItem(product, 2);

        var item = Assert.Single(cart.Items);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(2, item.Quantity);
    }




    [Fact]
    public void AddItem_ShouldMergeQuantity_WhenSameProductAddedTwice()
    {
        var cart = new Cart(Guid.NewGuid(), Guid.NewGuid());
        var product = CreateProduct();

        cart.AddItem(product, 2);
        cart.AddItem(product, 3);

        var item = Assert.Single(cart.Items);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void AddItem_ShouldCreateSeparateCartItems_WhenDifferentProductsAdded()
    {
        var cart = new Cart(Guid.NewGuid(), Guid.NewGuid());
        var productA = CreateProduct();
        var productB = CreateProduct();

        cart.AddItem(productA, 1);
        cart.AddItem(productB, 1);

        Assert.Equal(2, cart.Items.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_ShouldThrowArgumentException_WhenQuantityIsNotPositive(int quantity)
    {
        var cart = new Cart(Guid.NewGuid(), Guid.NewGuid());
        var product = CreateProduct();

        Assert.Throws<ArgumentException>(
            () => cart.AddItem(product, quantity)
        );
    }

   
}
