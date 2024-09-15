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

        public DbSet<Legalities> ImageUris { get; set; }
        public DbSet<Legalities> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserCardCollection>()
            .HasKey(c => c.Id); // Ensure 'Id' is set as the primary key

            modelBuilder.Entity<UserCardCollection>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd(); // Assuming 'Id' is auto-generated


            // Configure one-to-one relationship between ScryfallCard and Prices
            modelBuilder.Entity<ScryfallCard>()
                .HasOne(c => c.Prices)
                .WithOne(p => p.ScryfallCard)
                .HasForeignKey<Prices>(p => p.ScryfallCardId)
                .IsRequired(false); // Allow Prices to be optional

            // Configure one-to-one relationship between ScryfallCard and Legalities
            modelBuilder.Entity<ScryfallCard>()
                .HasOne(c => c.Legalities)
                .WithOne(l => l.ScryfallCard)
                .HasForeignKey<Legalities>(l => l.ScryfallCardId);

            // Configure one-to-one relationship between ScryfallCard and ImageUris
            modelBuilder.Entity<ScryfallCard>()
                .HasOne(c => c.ImageUris)
                .WithOne(i => i.ScryfallCard)
                .HasForeignKey<ImageUris>(i => i.ScryfallCardId)
                .IsRequired(false); // Allow ImageUris to be optional

            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<UserCardCollection>()
                .Property(c => c.CardIds)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v), // Convert list to JSON string for storage
                    v => JsonConvert.DeserializeObject<List<CardEntry>>(v)); // Convert JSON string back to list
           
            base.OnModelCreating(modelBuilder);
        }
    public DbSet<AppSettings> AppSettings { get; set; }
    }
}