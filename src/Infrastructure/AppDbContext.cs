using Microsoft.EntityFrameworkCore;
using OrderTracking.Domain;

namespace OrderTracking.Infrastructure;

public class AppDbContext : DbContext
{
    //add db context
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    //added customer products and carts db set
    public DbSet<Customer> Customers{get;set;}
    public DbSet<Product> Products{get;set;}
    public DbSet<Cart> Carts{get;set;}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //cart items is one directional
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(ci => ci.CartId);

        //cart items to product no nav
        modelBuilder.Entity<CartItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cart>()
            .Navigation(c => c.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        //fixed: no store type for price in products whic would have turnicated if no precision
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        //fixed: cartItem.id is not db generated
        modelBuilder.Entity<CartItem>()
            .Property(x => x.Id)
            .ValueGeneratedNever();

    }


}