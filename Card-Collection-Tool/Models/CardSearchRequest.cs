using Newtonsoft.Json;

namespace Card_Collection_Tool.Models
{
    public class CardSearchRequest
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("set")]
        public string? Set { get; set; }

        [JsonProperty("oracleText")]
        public string? OracleText { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("colors")]
        public string? Colors { get; set; }

        [JsonProperty("colorCriteria")]
        public string? ColorCriteria { get; set; }

        [JsonProperty("colorIdentity")]
        public string? ColorIdentity { get; set; }

        [JsonProperty("colorIdentityCriteria")]
        public string? ColorIdentityCriteria { get; set; }

        [JsonProperty("manaValue")]
        public float? ManaValue { get; set; }

        [JsonProperty("manaValueComparator")]
        public string? ManaValueComparator { get; set; }

        [JsonProperty("power")]
        public string? Power { get; set; }

        [JsonProperty("powerComparator")]
        public string? PowerComparator { get; set; }

        [JsonProperty("toughness")]
        public string? Toughness { get; set; }

        [JsonProperty("toughnessComparator")]
        public string? ToughnessComparator { get; set; }

        [JsonProperty("loyalty")]
        public string? Loyalty { get; set; }

        [JsonProperty("loyaltyComparator")]
        public string? LoyaltyComparator { get; set; }

        [JsonProperty("sortOrder")]
        public string? SortOrder { get; set; }

        [JsonProperty("sortDirection")]
        public string? SortDirection { get; set; }
    }


}
