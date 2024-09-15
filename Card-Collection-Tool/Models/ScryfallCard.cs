using System.Text.Json.Serialization;
using System.Collections.Generic;

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

    [JsonPropertyName("cmc")]
    public float? Cmc { get; set; } // Converted Mana Cost

    [JsonPropertyName("type_line")]
    public string? TypeLine { get; set; }

    [JsonPropertyName("set_name")]
    public string? SetName { get; set; }

    [JsonPropertyName("set")]
    public string? Set { get; set; }

    [JsonPropertyName("set_id")]
    public string? SetId { get; set; }

    [JsonPropertyName("rarity")]
    public string? Rarity { get; set; }

    [JsonPropertyName("released_at")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("flavor_text")]
    public string? FlavorText { get; set; }

    [JsonPropertyName("artist")]
    public string? Artist { get; set; }

    [JsonPropertyName("collector_number")]
    public string? CollectorNumber { get; set; }

    [JsonPropertyName("digital")]
    public bool? Digital { get; set; }

    [JsonPropertyName("full_art")]
    public bool? FullArt { get; set; }

    [JsonPropertyName("games")]
    public List<string>? Games { get; set; }

    [JsonPropertyName("image_uris")]
    public ImageUris? ImageUris { get; set; }

    [JsonPropertyName("colors")]
    public List<string>? Colors { get; set; }

    [JsonPropertyName("color_identity")]
    public List<string>? ColorIdentity { get; set; }

    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }

    [JsonPropertyName("legalities")]
    public required Legalities Legalities { get; set; }

    [JsonPropertyName("power")]
    public string? Power { get; set; }

    [JsonPropertyName("toughness")]
    public string? Toughness { get; set; }

    [JsonPropertyName("reprint")]
    public bool? Reprint { get; set; }

    [JsonPropertyName("variation")]
    public bool? Variation { get; set; }

    [JsonPropertyName("variation_of")]
    public string? VariationOf { get; set; }

    [JsonPropertyName("prices")]
    public Prices? Prices { get; set; }

    [JsonPropertyName("scryfall_uri")]

    public string? ScryfallUri { get; set; }

    public int LegalitiesId { get; set; }

}
