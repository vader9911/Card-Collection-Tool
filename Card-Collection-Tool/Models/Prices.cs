namespace Card_Collection_Tool.Models
{
    public class Prices
    {
        public int Id { get; set; }
        public string ScryfallCardId { get; set; }
        public ScryfallCard ScryfallCard { get; set; }

        // Different price types
        public string? Usd { get; set; }
        public string? Usd_Foil { get; set; }
        public string? Usd_Etched { get; set; }
        public string? Eur { get; set; }
        public string? Eur_Foil { get; set; }
        public string? Tix { get; set; }
    }

}
