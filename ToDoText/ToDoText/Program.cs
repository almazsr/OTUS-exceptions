using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ToDoText
{
    class Program
    {
        const string _fileName = "todo.txt";
        private static StringBuilder warning;

        static void AddTask(string name, DateTime date)
        {
            var task = new ToDoTask { Name = name, Date = date };
            File.AppendAllLines(_fileName, new[] { $"{task.Date:dd/MM/yy}\t{task.Name}" });
        }

        static List<ToDoTask> GetTasks()
        {
            warning = new StringBuilder();
            var taskList = new List<ToDoTask>();
            var todoTxtLines = File.ReadAllLines(_fileName);
            foreach (var line in todoTxtLines)
            {
                try
                {
                    taskList.Add(MakeTaskFromString(line));
                }
                catch (ArgumentException ex)
                {
                    warning.Append($"Некорректная строка: {line} {ex.Message}{Environment.NewLine}");
                }
            }
            return taskList;
        }

        static ToDoTask MakeTaskFromString(string str)
        {
            if (!str.Contains('\t'))
                throw new ArgumentException("Строка не содержит разделитель");
            var split = str.Split('\t');
            if (split.Length != 2)
                throw new ArgumentException("Строка не может быть корректно разделена на 2");
            if (DateTime.TryParse(split[0], out var date))
            {
                var name = split[1];
                return new ToDoTask { Name = name, Date = date };
            }
            else
            {
                throw new ArgumentException("Не удалось получить дату из строки");
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
            if (tasks.Count > 0)
            {
                var minDate = tasks.Min(t => t.Date);
                var maxDate = tasks.Max(t => t.Date);
                ShowTasks(tasks, minDate, maxDate);
            }
            else
                Console.WriteLine("Не найдено заданий");
        }

        static void ShowTasks(List<ToDoTask> tasks, DateTime from, DateTime to)
        {
            Console.WriteLine(warning);
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
                Console.WriteLine("Не задан аргумент");
                Console.ReadKey();
                return;
            }

            if (!File.Exists(_fileName))
            {
                Console.WriteLine("Не найден файл: " + _fileName);
                Console.ReadKey();
                return;
            }

            switch (args[0])
            {
                case "add":
                    if (args.Length != 3)
                    {
                        Console.WriteLine("Некорректное колчество аргументов");
                        Console.ReadKey();
                        return;
                    }
                    var name = args[1];
                    if (DateTime.TryParse(args[2], out var date))
                    {
                        AddTask(name, date);
                    }
                    else
                    {
                        Console.WriteLine("Неверная дата: " + args[2]);
                        Console.ReadKey();
                    }
                    break;
                case "today":
                    ListTodayTasks();
                    break;
                case "all":
                    ListAllTasks();
                    break;
                default:
                    Console.WriteLine("Некорректный аргумент");
                    break;
            }
        }
    }
}
