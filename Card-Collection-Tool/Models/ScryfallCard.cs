using System.ComponentModel.DataAnnotations;

namespace Card_Collection_Tool.Models
{
    public class ScryfallCard
    {
        [Key]
        public string Id { get; set; } // Card ID from Scryfall

        public string Name { get; set; }
        public string SetName { get; set; }
        public string ImageUri { get; set; }
        public string ManaCost { get; set; }
        public string TypeLine { get; set; }
        public string OracleText { get; set; }
        //TODO: Add other properties 
    }
}