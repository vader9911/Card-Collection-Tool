using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Card_Collection_Tool.Models
{
    public class UserCardCollection
    {
        [Key]
        public int CollectionID { get; set; } // Primary Key

        [Required]
        public string UserId { get; set; } // Foreign Key for the User

        [Required]
        public string CollectionName { get; set; } // Name of the Collection

        public DateTime CreatedDate  { get; set; } 
        
        public string ImageUri { get; set; }

        public string Notes {  get; set; }
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


    public class CreateCollectionRequest
    {
        public string CollectionName { get; set; }


        public string ImageUri { get; set; }

        public string Notes { get; set; }
    }

    public class UpdateCollectionRequest
    {
        public string CollectionName { get; set; }


        public string ImageUri { get; set; }

        public string Notes { get; set; }
    }


}