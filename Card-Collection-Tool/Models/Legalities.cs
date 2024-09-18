
namespace Card_Collection_Tool.Models
{
    public class Legalities
    {
        public int Id { get; set; } // Primary key for the Legalities table

        // All legalities fields from the JSON structure
        public string? Standard { get; set; }
        public string? Future { get; set; }
        public string? Historic { get; set; }
        public string? Timeless { get; set; }
        public string? Gladiator { get; set; }
        public string? Pioneer { get; set; }
        public string? Explorer { get; set; }
        public string? Modern { get; set; }
        public string? Legacy { get; set; }
        public string? Pauper { get; set; }
        public string? Vintage { get; set; }
        public string? Penny { get; set; }
        public string? Commander { get; set; }
        public string? Oathbreaker { get; set; }
        public string? StandardBrawl { get; set; }
        public string? Brawl { get; set; }
        public string? Alchemy { get; set; }
        public string? PauperCommander { get; set; }
        public string? Duel { get; set; }
        public string? OldSchool { get; set; }
        public string? Premodern { get; set; }
        public string? Predh { get; set; }

        // Foreign key relationship
        public string ScryfallCardId { get; set; }
        public ScryfallCard ScryfallCard { get; set; }

    }
}