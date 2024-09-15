public class Prices
{
    public int Id { get; set; } // Primary key for Prices table
    public decimal? USD { get; set; }
    public decimal? Eur { get; set; }
    public decimal? Tix { get; set; }

    // Foreign key to ScryfallCard
    public string ScryfallCardId { get; set; }
    public ScryfallCard ScryfallCard { get; set; } // Navigation property
}
