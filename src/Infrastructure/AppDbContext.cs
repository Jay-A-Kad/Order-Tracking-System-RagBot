using System.Security.Cryptography;
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
    public DbSet<Order> Orders{get;set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //BELOW ARE CART AND CART ITEMS RELATIONSHIPS

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

        

        //BELOW ARE ORDER SHIPMENT RELATIONSHIPS

        //order config and relationship
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId);


         //order status history config and relationship
        modelBuilder.Entity<Order>()
            .HasMany(o => o.History)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId);

        //shipment propery config and relationship
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Shipment)
            .WithOne()
            .HasForeignKey<Shipment>(s => s.OrderId);
        

        //order item config and relationship
        modelBuilder.Entity<OrderItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            

        //properties for above relationships

         modelBuilder.Entity<Order>()
            .Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);


        modelBuilder.Entity<Order>()
            .Navigation(x => x.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);    

        modelBuilder.Entity<OrderItem>()
        .Property(x => x.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<OrderStatusHistory>()
        .Property(x => x.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Shipment>()
            .Property(x => x.Id)
            .ValueGeneratedNever();


        modelBuilder.Entity<Order>()
            .Property(x => x.TotalAmount)
            .HasPrecision(18, 2);
    
        modelBuilder.Entity<OrderItem>()
            .Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

            
       

        
        

    }


}