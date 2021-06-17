using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;



namespace ToDoText
{
    using System;

    class Program
    {
        const string _fileName = "todo.txt";
        const string _logFileName = "log.txt";

        static void AddTask(string name, DateTime date)
        {
            var task = new ToDoTask(name, date);
            if (!File.Exists(_fileName))
            {
                Console.WriteLine($"Файл {_fileName} не существует. Создать его? y/n");
                if (Console.ReadKey().KeyChar == 'n')
                    return;
            }
            File.AppendAllLines(_fileName, new[] { $"{task.Date:dd/MM/yy}\t{task.Name}" });
        }

        static ToDoTask[] GetTasks()
        {
            if (File.Exists(_fileName))
            {
                try
                {
                    var todoTxtLines = File.ReadAllLines(_fileName);
                    if (todoTxtLines.Length > 0)
                    {
                        return todoTxtLines.Select( x =>
                        {
                            if (ToDoTask.TryParse(x, out var todoTask))
                                return todoTask;
                            return null;
                        }).ToArray();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            Console.WriteLine($"Список задач не существует, нужно сначала создать его.");
            return new ToDoTask[0];
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
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Run(new [] { "add", "mytask1", "01/12/20" });
            Run(new[] { "add", "mytask2", "01/12/21" });
            Run(new[] { "add", "mytask3", "01/12/22" });
            Run(new [] { "today"});
            Run(new [] { "all"});
            Run(new[] { "nothing" });
        }
        static void Run(string[] args)
        {
            
            if (args.Length>0 && args[0]!= null)
            switch (args[0])
            {
                case "add":
                        if (args.Length != 3)
                        {
                            Console.WriteLine($"Неверно количество аргументов. формат команды: add name date");
                            return;
                        }
                            var name = args[1];
                            if (!DateTime.TryParseExact(args[2], "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                            {
                                Console.WriteLine($"Неверный формат даты {args[2]}. Используйте формат dd/mm/yy");
                                return;
                            }
                            AddTask(name, date);
                        
                        
                        
                    break;
                case "today":
                    ListTodayTasks();
                    break;
                case "all":
                    ListAllTasks();
                    break;
                default: 
                    Console.WriteLine($"Неверная команда {args[0]} доступные команды: add, today, all");
                    break;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Global exception {e.ExceptionObject.ToString()}");
            File.AppendAllLines(_logFileName, new[] { DateTime.Now.ToString(), "Global exception:", e.ExceptionObject.ToString() }); ;
        }
    }
}
