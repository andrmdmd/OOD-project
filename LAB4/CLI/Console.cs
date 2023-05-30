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
        Stack<ICommand> UndoStack = new();
        Stack<ICommand> RedoStack = new();

        public MConsole()
        {
            commands.Add("list", new ListCommand());
            commands.Add("exit", new ExitCommand());
            commands.Add("find", new FindCommand());
            commands.Add("add", new AddCommand());
            commands.Add("edit", new EditCommand());
            commands.Add("undo", new UndoCommand(UndoStack, RedoStack));
            commands.Add("redo", new RedoCommand(UndoStack, RedoStack));
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
                    command.Execute(args);
                    UndoStack.Push(command);
                    if(RedoStack.Count > 0)
                    {
                        RedoStack.Clear();
                    }

                }
                catch (InvalidArgumentsException ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                }
                catch (Exception ex)
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

    public class CommandQueue
    {
        List<ICommand> commands;
    }
}
