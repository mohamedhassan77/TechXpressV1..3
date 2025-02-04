using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TechXpress_domain.Entities;
using TechXpress_domain.ValueObjects;

public class TechXpress_context : IdentityDbContext<ApplicationUser>
{
    public TechXpress_context(DbContextOptions<TechXpress_context> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Product configuration
        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // UserProfile configuration
        builder.Entity<UserProfile>()
            .HasMany(u => u.Addresses)
            .WithOne(a => a.UserProfile)
            .HasForeignKey(a => a.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // ApplicationUser - UserProfile relationship
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.UserProfile)
            .WithOne()
            .HasForeignKey<ApplicationUser>(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Category configuration
        builder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cart configuration
        builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Wishlist configuration
        builder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<CartItem>()
    .HasOne(c => c.Cart)
    .WithMany()
    .HasForeignKey(c => c.CartId)
    .OnDelete(deleteBehavior: DeleteBehavior.NoAction);



        // Add this to configure the ApplicationUser relationship
        builder.Entity<CartItem>()
            .HasOne(ci => ci.applicationUser)
            .WithMany()
            .HasForeignKey(ci => ci.UserId)
    .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        builder.Entity<CartItem>()
     .HasKey(ci => new { ci.ProductId, ci.CartId });
    }
}