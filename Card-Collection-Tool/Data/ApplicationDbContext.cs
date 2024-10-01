// ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Card_Collection_Tool.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Keep DbSets related to Identity and other essential configuration
        // Remove if not directly used by EF Core in your current architecture

        // public DbSet<UserCardCollection> UserCardCollection { get; set; } (Remove if not needed)
        // public DbSet<ScryfallCard> ScryfallCards { get; set; } (Remove if not needed)
        // public DbSet<Legalities> Legalities { get; set; } (Remove if not needed)
        // public DbSet<ImageUris> ImageUris { get; set; } (Remove if not needed)
        // public DbSet<Prices> Prices { get; set; } (Remove if not needed)
        // public DbSet<CollectionCard> CollectionCards { get; set; } (Remove if not needed)
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Retain Identity-related configurations
            // Remove any table-specific configurations not required by Identity

            // Example:
            // modelBuilder.Entity<UserCardCollection>()
            //     .HasKey(c => c.Id); (Remove if not needed)
        }
    }
}
