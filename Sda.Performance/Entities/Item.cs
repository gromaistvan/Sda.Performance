using System.ComponentModel.DataAnnotations;

namespace Sda.Performance.Entities
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EntryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}