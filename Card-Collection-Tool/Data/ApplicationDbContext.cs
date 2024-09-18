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

        public DbSet<Legalities> Legalities { get; set; }

        public DbSet<ImageUris> ImageUris { get; set; }
        public DbSet<Prices> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserCardCollection>()
                .HasKey(c => c.Id); // Ensure 'Id' is set as the primary key

            modelBuilder.Entity<UserCardCollection>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd(); // Assuming 'Id' is auto-generated

            // Configure Legalities as a one-to-one relationship with ScryfallCard
            modelBuilder.Entity<ScryfallCard>()
                .HasOne(c => c.Legalities)
                .WithOne(l => l.ScryfallCard)
                .HasForeignKey<Legalities>(l => l.ScryfallCardId);

            // Configure Prices as a one-to-one relationship with ScryfallCard
            // Allow NULLs by not enforcing the unique constraint
            modelBuilder.Entity<ScryfallCard>()
                .HasOne(c => c.Prices)
                .WithOne(p => p.ScryfallCard)
                .HasForeignKey<Prices>(p => p.ScryfallCardId)
                .IsRequired(false); // Allow Prices to be optional

            // Configure ImageUris as a one-to-one relationship with ScryfallCard
            modelBuilder.Entity<ScryfallCard>()
                .HasOne(c => c.ImageUris)
                .WithOne(i => i.ScryfallCard)
                .HasForeignKey<ImageUris>(i => i.ScryfallCardId)
                .IsRequired(false); // Allow ImageUris to be optional

            modelBuilder.Entity<UserCardCollection>()
                .Property(c => c.CardIds)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v), // Convert list to JSON string for storage
                    v => JsonConvert.DeserializeObject<List<CardEntry>>(v)); // Convert JSON string back to list
        }

        public DbSet<AppSettings> AppSettings { get; set; }
    }
}