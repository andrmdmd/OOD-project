using LAB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB4
{
    public class MConsole
    {
        public static Dictionary<string, ICommand> commands = new();
        public static List<string> notQueueable = new();
        public CommandQueue commandQueue;

        public MConsole()
        {
            commandQueue = new CommandQueue();

            commands.Add("list", new ListCommand());
            commands.Add("exit", new ExitCommand());
            commands.Add("find", new FindCommand());
            commands.Add("add", new AddCommand());
            commands.Add("edit", new EditCommand());
            commands.Add("delete", new DeleteCommand());
            commands.Add("queue", new QueueCommand(commandQueue));

            notQueueable.Add("exit");
            notQueueable.Add("queue");

            RunConsole();
        }
        public void RunConsole()
        {
            Console.WriteLine($"Programowanie obiektowe: Command-line interface\nWelcome {Environment.UserName}\n");
            Console.WriteLine("List of available commands:\n");
            foreach(var c in commands)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{c.Value.Name}");
                Console.ResetColor();

                if (c.Key == "queue")
                {
                    QueueCommand cq = (QueueCommand)c.Value;

                    foreach (var q in cq.queueCommands)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write($"\t{q.Value.Name}");
                        Console.ResetColor();
                        Console.Write($"\t{q.Value.Description}");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"\n\t\t{q.Value.Usage}");
                        Console.ResetColor();
                    }
                    continue;
                }

                Console.Write($"\t{c.Value.Description}");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\n\t{c.Value.Usage}");
                Console.ResetColor();

            }

            while (true)
            {
                Console.Write("\n> ");
                string input = Console.ReadLine();

                string[] args = input.Split(' ');

                if (args.Length == 0)
                {
                    continue;
                }

                string commandName = args[0].ToLower();

                if (!commands.ContainsKey(commandName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Unknown command {commandName}");
                    Console.ResetColor();
                    continue;
                }

                ICommand command = commands[commandName];

                try
                {
                    string[] obj = command.Prepare(args);
                    if (!notQueueable.Contains(commandName))
                    {
                        commandQueue.commands.Add(new SerializableCommandObject(commandName, obj));
                        commandQueue.inputs.Add(input);
                        Console.WriteLine($"{input} added to queue");
                    }
                        
                }
                catch(InvalidArgumentsException ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                }

                if (command is ExitCommand)
                {
                    break;
                }
            }

            Console.WriteLine("Goodbye!");
        }

    }
    [Serializable]
    public class CommandQueue
    {
        public List<SerializableCommandObject> commands = new();
        public List<string> inputs = new();
    }
}
