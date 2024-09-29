using System.Text.Json.Serialization;

namespace Card_Collection_Tool.Models
{
    public class CollectionCard
    {
        public int CollectionCardID { get; set; } 
        public int CollectionID { get; set; }      
        public string CardID { get; set; }         
        public int Quantity { get; set; }          
    }

    public class AddCardToCollectionRequest
    {
        [JsonPropertyName("collectionID")]
        public int CollectionID { get; set; } // Ensure this is an int, not string

        [JsonPropertyName("cardID")]
        public string CardID { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

}



