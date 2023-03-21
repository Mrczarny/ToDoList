using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class ToDoModel
    {
        public ToDoModel()
        {
            
        }
        public ToDoModel(ToDoDto m)
        {
            Name = m.Name;
            Description = m.Description;
            if (m.Date != null) Date = (DateTime) m.Date;
        }

        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
