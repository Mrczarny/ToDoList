using System;

namespace ToDoList.Models
{
    public class ToDoDto
    {
        public ToDoDto(string name, string description, DateTime date)
        {
            this.Guid = Guid.NewGuid();
            this.Name = name;
            this.Description = description;
            this.Date = date;
        }

        public ToDoDto(ToDoModel m)
        {
            Date = m.Date;
            Name = m.Name;
            Description = m.Description;
        }

        public Guid Guid {
            get => this.Guid;
            set => Guid.NewGuid();
        } 

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}
