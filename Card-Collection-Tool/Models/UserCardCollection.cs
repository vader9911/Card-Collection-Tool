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

        public List<int> CardIds { get; set; } = new List<int>(); // List of Card IDs
    }
}