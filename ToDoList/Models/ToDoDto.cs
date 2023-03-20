using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class ToDoDto
    {
        public ToDoDto()
        {
            
        }
        public ToDoDto(string name, string description, DateTime date)
        {
            this.ToDoGuid = Guid.NewGuid();
            this.Name = name;
            this.Description = description;
            this.Date = date;
        }

        public ToDoDto(ToDoModel m)
        {
            ToDoGuid = Guid.NewGuid();
            Date = m.Date;
            Name = m.Name;
            Description = m.Description;
        }

        public Guid ToDoGuid { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
