namespace Card_Collection_Tool.Models
{
    public class Prices
    {
        public int Id { get; set; }
        public string ScryfallCardId { get; set; }
        public ScryfallCard ScryfallCard { get; set; }

        // Different price types
        public string? Usd { get; set; }
        public string? UsdFoil { get; set; }
        public string? UsdEtched { get; set; }
        public string? Eur { get; set; }
        public string? EurFoil { get; set; }
        public string? Tix { get; set; }
    }

}
