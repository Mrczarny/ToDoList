using System;

namespace ToDoList.Models
{
    public class ToDoModel
    {
        public ToDoModel()
        {
            
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Day { get; set; }
    }
}
