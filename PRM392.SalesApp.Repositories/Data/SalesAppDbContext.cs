using Microsoft.EntityFrameworkCore;
using PRM392.SalesApp.Repositories.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PRM392.SalesApp.Repositories.Data
{
    public class SalesAppDbContext : DbContext
    {
        public SalesAppDbContext(DbContextOptions<SalesAppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<StoreLocation> StoreLocations => Set<StoreLocation>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Users
            b.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.UserID);
                e.Property(x => x.Username).HasMaxLength(50).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
                e.Property(x => x.Email).HasMaxLength(100).IsRequired();
                e.Property(x => x.PhoneNumber).HasMaxLength(15);
                e.Property(x => x.Address).HasMaxLength(255);
                e.Property(x => x.Role).HasMaxLength(50).IsRequired();
            });

            // Categories
            b.Entity<Category>(e =>
            {
                e.ToTable("Categories");
                e.HasKey(x => x.CategoryID);
                e.Property(x => x.CategoryName).HasMaxLength(100).IsRequired();
            });

            // Products
            b.Entity<Product>(e =>
            {
                e.ToTable("Products");
                e.HasKey(x => x.ProductID);
                e.Property(x => x.ProductName).HasMaxLength(100).IsRequired();
                e.Property(x => x.BriefDescription).HasMaxLength(255);
                e.Property(x => x.FullDescription);
                e.Property(x => x.TechnicalSpecifications);
                e.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();
                e.Property(x => x.ImageURL).HasMaxLength(255);

                e.HasOne(x => x.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(x => x.CategoryID)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // Carts
            b.Entity<Cart>(e =>
            {
                e.ToTable("Carts");
                e.HasKey(x => x.CartID);
                e.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
                e.Property(x => x.Status).HasMaxLength(50).IsRequired();

                // UserID là tham chiếu logic (có thể để NoAction để tránh xoá dây chuyền)
                e.HasOne<User>()
                 .WithMany()
                 .HasForeignKey(x => x.UserID)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // CartItems
            b.Entity<CartItem>(e =>
            {
                e.ToTable("CartItems");
                e.HasKey(x => x.CartItemID);
                e.Property(x => x.Quantity).IsRequired();
                e.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();

                e.HasOne(x => x.Cart)
                 .WithMany(c => c.Items)
                 .HasForeignKey(x => x.CartID)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<Product>()
                 .WithMany()
                 .HasForeignKey(x => x.ProductID)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // Orders
            b.Entity<Order>(e =>
            {
                e.ToTable("Orders");
                e.HasKey(x => x.OrderID);
                e.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();
                e.Property(x => x.BillingAddress).HasMaxLength(255).IsRequired();
                e.Property(x => x.OrderStatus).HasMaxLength(50).IsRequired();
                e.Property(x => x.OrderDate).HasDefaultValueSql("GETDATE()");

                e.HasOne<Cart>()
                 .WithMany()
                 .HasForeignKey(x => x.CartID)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasOne<User>()
                 .WithMany()
                 .HasForeignKey(x => x.UserID)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // Payments
            b.Entity<Payment>(e =>
            {
                e.ToTable("Payments");
                e.HasKey(x => x.PaymentID);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
                e.Property(x => x.PaymentStatus).HasMaxLength(50).IsRequired();
                e.Property(x => x.PaymentDate).HasDefaultValueSql("GETDATE()");

                e.HasOne<Order>()
                 .WithMany()
                 .HasForeignKey(x => x.OrderID)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Notifications
            b.Entity<Notification>(e =>
            {
                e.ToTable("Notifications");
                e.HasKey(x => x.NotificationID);
                e.Property(x => x.Message).HasMaxLength(255);
                e.Property(x => x.IsRead).HasDefaultValue(false);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

                e.HasOne<User>()
                 .WithMany()
                 .HasForeignKey(x => x.UserID)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatMessages
            b.Entity<ChatMessage>(e =>
            {
                e.ToTable("ChatMessages");
                e.HasKey(x => x.ChatMessageID);
                e.Property(x => x.Message);
                e.Property(x => x.SentAt).HasDefaultValueSql("GETDATE()");

                e.HasOne<User>()
                 .WithMany()
                 .HasForeignKey(x => x.UserID)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // StoreLocations
            b.Entity<StoreLocation>(e =>
            {
                e.ToTable("StoreLocations");
                e.HasKey(x => x.LocationID);
                e.Property(x => x.Address).HasMaxLength(255).IsRequired();
                e.Property(x => x.Latitude).HasColumnType("decimal(9,6)").IsRequired();
                e.Property(x => x.Longitude).HasColumnType("decimal(9,6)").IsRequired();
            });
        }
    }
}
