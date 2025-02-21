using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;

namespace TechXpress_infrastructure.Data
{
    public class TechXpress_context : IdentityDbContext<ApplicationUser>
    {
        public TechXpress_context(DbContextOptions<TechXpress_context> options)
            : base(options)
        {
        }

        // DbSet declarations for all entities
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<LoginHistoryEntry> LoginHistoryEntries { get; set; }

        public DbSet<ActiveSession> ActiveSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add indexes for query optimization
            modelBuilder.Entity<Product>().HasIndex(p => p.Name);
            modelBuilder.Entity<Category>().HasIndex(c => c.Name);
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderDate);

            // 1️⃣ UserProfile & ApplicationUser (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<UserProfile>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2️⃣ UserProfile & Addresses (One-to-Many)
            modelBuilder.Entity<UserProfile>()
                .HasMany(p => p.Addresses)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3️⃣ Categories & Products (One-to-Many)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            // 4️⃣ ApplicationUser & Cart (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
           .HasOne(u => u.Wishlist)
           .WithOne(w => w.ApplicationUser)
           .HasForeignKey<Wishlist>(w => w.UserId)
           .OnDelete(DeleteBehavior.Cascade);


            // 6️⃣ WishlistItem (Composite Key & Relationships)
            modelBuilder.Entity<WishlistItem>() 
                .HasKey(wi => new { wi.WishlistId, wi.ProductId });

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Wishlist)
                .WithMany(w => w.WishlistItems)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 7️⃣ CartItem (Composite Key & Relationships)
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 8️⃣ Orders & ApplicationUser (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ApplicationUser)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 9️⃣ Orders & OrderItems (One-to-Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔟 OrderItems & Products (Foreign Key Handling)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Fixed typo

            // 11️⃣ Shipping & Orders (One-to-One)
            modelBuilder.Entity<Shipping>()
                .HasOne(s => s.Order)
                .WithOne(o => o.Shipping)
                .HasForeignKey<Shipping>(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 12️⃣ Reviews & Products (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 13️⃣ Reviews & ApplicationUser (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.ApplicationUser)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>().HasIndex(p => p.Name);
            modelBuilder.Entity<Category>().HasIndex(c => c.Name);
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderDate);
        }
    }
}