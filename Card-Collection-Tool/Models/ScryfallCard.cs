using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Card_Collection_Tool.Models
{
    public class ScryfallCard
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("set_name")]
        public string SetName { get; set; }

        [JsonPropertyName("image_uri")]
        public string? ImageUri { get; set; }

        [JsonPropertyName("oracle_text")]
        public string? OracleText { get; set; }

        [JsonPropertyName("mana_cost")]
        public string? ManaCost { get; set; }

        

        [JsonPropertyName("type_line")]
        public string? TypeLine { get; set; }
    }
}