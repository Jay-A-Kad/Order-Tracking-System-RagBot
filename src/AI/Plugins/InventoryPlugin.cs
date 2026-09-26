using System.ComponentModel;
using System.Threading.Tasks;
using OrderTracking.Infrastructure;
using Microsoft.SemanticKernel;
using Microsoft.EntityFrameworkCore;

namespace OrderTracking.AI;

public class InventoryPlugin
{
    private readonly AppDbContext _dbContext;

    public InventoryPlugin(AppDbContext dbContext)
    {
        _dbContext = dbContext;    
    }


    //check inventory data
    [KernelFunction("check_stock")]
    [Description("Checks current stock quantity for a product SKU")]
    public async Task<string> CheckInventoryStock([Description("This is a products SKU")] string sku)
    {
        var checkStockSku = await _dbContext.Products.Where(x => x.Sku == sku)
            .ToListAsync();

        var skuSummaryString = string.Empty;

        //loop for all the the inventory sku
        foreach(var stock in checkStockSku)
        {
            skuSummaryString += $"Products: {stock.Id} : {stock.Name} : {stock.Sku} : {stock.StockQuantity} \n";
        }

        return skuSummaryString.Length == 0 ? "The inventory has no SKU for the products" : skuSummaryString;
    }
}