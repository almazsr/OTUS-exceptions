using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ToDoText
{
    class Program
    {
        const string _fileName = "todo.txt";

        static void CheckIfFileExists(string path)
        {
           if(!File.Exists(path))
           {
              throw new FileNotFoundException($"File {Assembly.GetExecutingAssembly().CodeBase}\\{path} does not exist.");
           }
        }
        static void CheckInputParameters(string[] parameters)
        {
            if (parameters.Length == 1)
            {
               CheckAction(parameters[0]);
            }
            else if (parameters.Length == 3)
            {
               CheckAction(parameters[0]);
               CheckDateParameter(parameters[2]);
            }
            else
            {
               throw new ArgumentException("Invalid parameter count, only one or three are allowed");
            }
        }

        static void CheckAction(string parameter)
        {
           if (parameter != "add" || parameter != "today" || parameter != "all")
           {
              throw new ArgumentException("Invalid parameter only \"add\", \"today\" or \"all\" are allowed");
           }
        }

        private static void CheckDateParameter(string date)
        {
            DateTime now = DateTime.Now;
            if (!DateTime.TryParseExact(date, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out now))
            {
               throw new ArgumentException("Invalid DateTime format");
            }
        }

        static void AddTask(string name, DateTime date)
        {
            var task = new ToDoTask { Name = name, Date = date };
            File.AppendAllLines(_fileName, new[] { $"{task.Date:dd/MM/yy}\t{task.Name}" });
         }

        static ToDoTask[] GetTasks()
        {
            var todoTxtLines = File.ReadAllLines(_fileName);
            return todoTxtLines.Select(l =>
            {
               try
               {
                  var split = l.Split('\t');
                  var date = DateTime.ParseExact(split[0], "dd/MM/yy", CultureInfo.InvariantCulture);
                  var name = split[1];
                  return new ToDoTask { Name = name, Date = date };
               }
               catch(IndexOutOfRangeException ex)
               {
                  Console.WriteLine(ex.ToString());
                  return ToDoTask.ErrorTask;
               }
            }).ToArray();
        }

        static void ListTodayTasks()
        {
            var tasks = GetTasks();
            ShowTasks(tasks, DateTime.Today, DateTime.Today);
        }

        static void ListAllTasks()
        {
            var tasks = GetTasks();
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
            CheckIfFileExists(_fileName);
            CheckInputParameters(args);
            switch (args[0])
            {
                case "add":
                    var name = args[1];
                    var date = DateTime.ParseExact(args[2], "dd/MM/yy", CultureInfo.InvariantCulture);
                    AddTask(name, date);
                    break;
                case "today":
                    ListTodayTasks();
                    break;
                case "all":
                    ListAllTasks();
                    break;
                  default:
                    break;
            }
        }
    }
}
