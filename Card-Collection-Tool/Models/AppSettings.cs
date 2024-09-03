using System.ComponentModel.DataAnnotations;

namespace Card_Collection_Tool.Models
{
    public class AppSettings
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}