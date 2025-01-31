using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TechXpress_domain.Entities;

namespace TechXpress_infrastructure.Data
{
    public class TechXpress_context : IdentityDbContext<ApplicationUser>
    {
        public TechXpress_context(DbContextOptions<TechXpress_context> options)
            : base(options)
        {
        }

        // Define your DbSets here
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        builder.Entity<Product>()
        .Property(p => p.Price)
        .HasColumnType("decimal(18,2)");

            // Configure one-to-many relationship between UserProfile and Address
            builder.Entity<UserProfile>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure Category-Product relationship
            builder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
