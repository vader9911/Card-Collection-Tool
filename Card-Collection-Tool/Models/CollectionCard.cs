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

public class UpdateCardQuantityRequest
{
    public int CollectionId { get; set; }
    public string CardId { get; set; }
    public int QuantityChange { get; set; }
}




