using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Card_Collection_Tool.Models; // Imports the Card and Collection models
using Newtonsoft.Json;

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

            modelBuilder.Entity<UserCardCollection>()
            .HasKey(c => c.Id); // Ensure 'Id' is set as the primary key

            modelBuilder.Entity<UserCardCollection>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd(); // Assuming 'Id' is auto-generated

            // Configure ImageUris as an owned entity type
            modelBuilder.Entity<ScryfallCard>().OwnsOne(p => p.ImageUris);
            modelBuilder.Entity<ScryfallCard>().OwnsOne(p => p.Prices);
            modelBuilder.Entity<UserCardCollection>()
                .Property(c => c.CardIds)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v), // Convert list to JSON string for storage
                    v => JsonConvert.DeserializeObject<List<CardEntry>>(v)); // Convert JSON string back to list
        }
    public DbSet<AppSettings> AppSettings { get; set; }
    }
}