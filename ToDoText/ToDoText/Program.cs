using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ToDoText
{
    class Program
    {
        const string _fileName = "todo.txt";

        static void AddTask(string name, DateTime date)
        {
            var task = new ToDoTask { Name = name, Date = date };
            try
            {
	            File.AppendAllLines(_fileName, new[] {$"{task.Date:dd/MM/yy}\t{task.Name}"});
            }
            catch (FileNotFoundException)
            {
	            Console.WriteLine($"File {_fileName} not found");
	            throw;
            }
        }

        static ToDoTask[] GetTasks()
        {
	        try
	        {
				var todoTxtLines = File.ReadAllLines(_fileName);
				return todoTxtLines.Select(l =>
				{
					var split = l.Split('\t');
					if (split.Length < 2
					    || DateTime.TryParseExact(split[0], "dd/MM/yy", CultureInfo.InvariantCulture,
						    DateTimeStyles.None, out var date))
						throw new Exception($"Wrong format in {_fileName}");
					var name = split[1];
					return new ToDoTask { Name = name, Date = date };
				}).ToArray();
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine($"File {_fileName} not found");
				throw;
			}
        }

        static void ListTodayTasks()
        {
            var tasks = GetTasks();
            ShowTasks(tasks, DateTime.Today, DateTime.Today);
        }

        static void ListAllTasks()
        {
            var tasks = GetTasks();
            if (tasks.Length == 0) return;
            var minDate = tasks.Min(t => t.Date);
            var maxDate = tasks.Max(t => t.Date);
            ShowTasks(tasks, minDate, maxDate);
        }

        static void ShowTasks(ToDoTask[] tasks, DateTime from, DateTime to)
        {
            for (var date = from.Date; date < to.Date.AddDays(1); date += TimeSpan.FromDays(1))
            {
                var dateTasks = tasks.Where(t => t.Date >= date && t.Date < date.AddDays(1)).ToArray();
                if (dateTasks.Any())
                {
                    Console.WriteLine($"{date:dd/MM/yy}");
                    foreach (var task in dateTasks)
                    {
                        Console.WriteLine($"\t{task.Name}");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
	        if (args.Length == 0)
	        {
				Console.WriteLine("Use command line arguments");
				return;
	        }

            switch (args[0])
            {
                case "add":
                    var name = args[1];
					if (DateTime.TryParseExact(args[2], "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
						AddTask(name, date);
					else
						Console.WriteLine("Use dd/MM/yy date format");
                    break;
                case "today":
                    ListTodayTasks();
                    break;
                case "all":
                    ListAllTasks();
                    break;
            }
        }
    }
}
