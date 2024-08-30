using System.ComponentModel.DataAnnotations;

namespace Card_Collection_Tool.Models
{
    public class Collection
    {

        public int id { get; set; }
        public string Name { get; set; }

        [Required]

        public int? Description { get; set; }
    }
}
