using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Card_Collection_Tool.Models; // Imports the Card and Collection models

namespace Card_Collection_Tool.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserCardCollection> UserCardCollections { get; set; }

        public DbSet<ScryfallCard> ScryfallCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ImageUris as an owned entity type
            modelBuilder.Entity<ScryfallCard>().OwnsOne(p => p.ImageUris);
            modelBuilder.Entity<ScryfallCard>().OwnsOne(p => p.Prices);
        }
    public DbSet<AppSettings> AppSettings { get; set; }
    }
}