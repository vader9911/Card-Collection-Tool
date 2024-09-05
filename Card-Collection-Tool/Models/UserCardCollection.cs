using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Card_Collection_Tool.Models
{
   public class UserCardCollection
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        public string UserId { get; set; } // Foreign Key for the User

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; } // Navigation property for Identity User

        [Required]
        public string CardName { get; set; } // Name of the Card
        public string CollectionName { get; set; } // Name of the Collection
        public string CardId { get; set; } // Card Id
    }
}
