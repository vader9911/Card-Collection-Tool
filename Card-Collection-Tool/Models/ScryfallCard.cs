using System.Text.Json.Serialization;

public class ScryfallCard
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; set; }

    [JsonPropertyName("mana_cost")]
    public string? ManaCost { get; set; }

    [JsonPropertyName("set_name")]
    public string? SetName { get; set; }

    [JsonPropertyName("type_line")]
    public string? TypeLine { get; set; }

    [JsonPropertyName("scryfall_uri")]
    public string? ScryfallUri { get; set; }

    [JsonPropertyName("flavor_text")]
    public string? FlavorText { get; set; }

    [JsonPropertyName("artist")]
    public string? Artist { get; set; }

    [JsonPropertyName("rarity")]

    public string? Rarity { get; set; }

    [JsonPropertyName("released_at")]

    public string? ReleaseDate { get; set; }

    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }

    [JsonPropertyName("image_uris")]
    public ImageUris? ImageUris { get; set; } // This will not directly map to a DB column

    [JsonPropertyName("prices")]
    public Prices? Prices { get; set; }
}


public class ImageUris
{
    [JsonPropertyName("small")]
    public string? Small { get; set; }

    [JsonPropertyName("normal")]
    public string? Normal { get; set; }

    [JsonPropertyName("large")]
    public string? Large { get; set; }
}

public class Prices
    {
        [JsonPropertyName("usd")]
        public string? USD { get; set; }

        [JsonPropertyName("usd_foil")]
        public string? USDFoil { get; set; }

        [JsonPropertyName("usd_etched")]
        public string? USDEtched { get; set; }
}
