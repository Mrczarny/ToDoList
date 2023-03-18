using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class ToDoModel
    {
        public ToDoModel()
        {
            
        }

        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
