using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sda.Performance.Entities
{
    public class Entry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Text { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public long? Number { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}
