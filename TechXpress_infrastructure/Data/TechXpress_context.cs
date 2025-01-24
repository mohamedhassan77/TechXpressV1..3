using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TechXpress_domain.Entities;

namespace TechXpress_infrastructure.Data
{
    public class TechXpress_context : DbContext
    {
        public TechXpress_context(DbContextOptions<TechXpress_context> options)
            : base(options)
        {
        }

        // Define your DbSets here
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }

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
        }
    }
}
