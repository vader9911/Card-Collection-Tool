using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Card_Collection_Tool.Models
{
    public class UserCardCollection
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        public string UserId { get; set; } // Foreign Key for the User

        [Required]
        public string CollectionName { get; set; } // Name of the Collection

        public List<CardEntry> CardIds { get; set; } = new List<CardEntry>(); // List of Card IDs
    }

    public class CardEntry
    {
        public string CardId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddCardRequest
    {
        public string CardId { get; set; } // The ID of the card being added
        public int Quantity { get; set; } // The quantity of the card being added
    }

}