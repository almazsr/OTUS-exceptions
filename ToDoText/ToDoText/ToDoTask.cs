using System;
using System.Diagnostics;
using System.Globalization;

namespace ToDoText
{
    public class ToDoTask
    {
        public static readonly DateTime MinDate = new DateTime(2000, 1, 1);
        public static readonly DateTime MaxDate = new DateTime(2100, 1, 1);

        public ToDoTask(string name, DateTime date)
        {
            if (name == null|| name.Length==0)
                throw new NotImplementedException("Недопустимо пустое имя задачи!");
            if (date < MinDate || date > MaxDate)
                throw new NotImplementedException($"Дата должна быть между {MinDate} и {MaxDate}");
            Name = name;
            Date = date;
        }

        public static ToDoTask Parse(string input)
        {
            var split = input.Split('\t');
            var date = DateTime.ParseExact(split[0], "dd/MM/yy", CultureInfo.InvariantCulture);
            var name = split[1];
            return new ToDoTask(name, date);
        }

        public static bool TryParse(string input, out ToDoTask toDoTask)
        {
            try
            {
                toDoTask= Parse(input);
                return true;
            }
            catch (Exception e) 
            {
                Console.WriteLine($"Ошибка при разборе файла задач{e.ToString()}");
                toDoTask = null;
                return false;

            }
        }

        public DateTime Date { get; }
        public string Name { get; }
    }
}
