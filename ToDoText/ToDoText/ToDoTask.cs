using System;

namespace ToDoText
{
    public class ToDoTask
    {
        public static ToDoTask ErrorTask = new ToDoTask() { Name = "ErrorTask", Date = DateTime.Now };
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}
