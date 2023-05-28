using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class ToDoModel
    {
        public ToDoModel()
        {
            
        }
        public ToDoModel(ToDoDto m, string userId)
        {
            Name = m.Name;
            Description = m.Description;
            if (m.Date != default) Date = (DateTime) m.Date;
            UserId = userId;
        }

        [Required]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
