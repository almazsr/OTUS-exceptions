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
            File.AppendAllLines(_fileName, new[] { $"{task.Date:dd/MM/yy}\t{task.Name}" });
        }

        static ToDoTask[] GetTasks()
        {
            var todoTxtLines = File.ReadAllLines(_fileName);
            return todoTxtLines.Select(l =>
            {
                var split = l.Split('\t');
                var date = DateTime.ParseExact(split[0], "dd/MM/yy", CultureInfo.InvariantCulture);
                var name = split[1];
                return new ToDoTask { Name = name, Date = date };
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
            try
            {
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
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Указано неверное количество параметров. Варианты запуска приложения:");
                Console.WriteLine("\tToDoText add <name> <date>");
                Console.WriteLine("\tToDoText today");
                Console.WriteLine("\tToDoText all");
            }
            catch (FormatException)
            {
                Console.WriteLine("Дата указана в неверном формате. Ожидается формат dd/MM/yy.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл \"{_fileName}\" не найден");
            }
            finally
            {
                Console.WriteLine("Для завершения работы приложения нажмите любую клавишу");
                Console.ReadKey();
            }
        }
    }
}
