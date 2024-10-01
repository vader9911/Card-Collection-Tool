using Newtonsoft.Json;

namespace Card_Collection_Tool.Models
{
    public class ScryfallCard
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cmc")]
        public float? Cmc { get; set; }

        [JsonProperty("color_identity")]
        public List<string?>? ColorIdentity { get; set; }

        [JsonProperty("colors")]
        public List<string?>? Colors { get; set; }

        [JsonProperty("keywords")]
        public List<string?>? Keywords { get; set; }

        [JsonProperty("mana_cost")]
        public string? ManaCost { get; set; }

        [JsonProperty("power")]
        public string? Power { get; set; }

        [JsonProperty("toughness")]
        public string? Toughness { get; set; }

        [JsonProperty("type_line")]
        public string? TypeLine { get; set; }

        [JsonProperty("artist")]
        public string? Artist { get; set; }

        [JsonProperty("collector_number")]
        public string? CollectorNumber { get; set; }

        [JsonProperty("digital")]
        public bool Digital { get; set; }

        [JsonProperty("flavor_text")]
        public string? FlavorText { get; set; }

        [JsonProperty("oracle_text")]
        public string? OracleText { get; set; }

        [JsonProperty("full_art")]
        public bool? FullArt { get; set; }

        [JsonProperty("games")]
        public List<string?>? Games { get; set; }

        [JsonProperty("rarity")]
        public string? Rarity { get; set; }

        [JsonProperty("released_at")]
        public string? ReleaseDate { get; set; }

        [JsonProperty("reprint")]
        public bool? Reprint { get; set; }

        [JsonProperty("set_name")]
        public string? SetName { get; set; }

        [JsonProperty("set")]
        public string? Set { get; set; }

        [JsonProperty("set_id")]
        public string? SetId { get; set; }

        [JsonProperty("variation")]
        public bool Variation { get; set; }

        [JsonProperty("variation_of")]
        public string? VariationOf { get; set; }

        // Navigation properties for related entities
        [JsonProperty("legalities")]
        public Legalities Legalities { get; set; }

        [JsonProperty("prices")]
        public Prices Prices { get; set; }

        [JsonProperty("image_uris")]
        public ImageUris ImageUris { get; set; }

        public ScryfallCard()
        {
            // Initialize objects to avoid null reference exceptions
            Legalities = new Legalities();
            Prices = new Prices();
            ImageUris = new ImageUris();
        }
    }
}
