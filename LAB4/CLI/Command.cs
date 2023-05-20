using LAB4;
using Microsoft.Identity.Client;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LAB
{
    [Serializable]
    public class SerializableCommandObject
    {
        public string commandName { get; set; }
        public string[] commandArguments { get; set; }

        public SerializableCommandObject() { }
        public SerializableCommandObject(string commandName, string[] commandArguments)
        {
            this.commandName = commandName;
            this.commandArguments = commandArguments;
        }
    }
    public class ListCommand : ICommand
    {
        public string Name { get; } = "list";

        public string Description { get; } = "print all objects of a particular type";

        public string Usage => "Usage: list <class name>";
        public string[] Prepare(string[] args)
        {
            if (args.Length != 2)
            {
                throw new InvalidArgumentsException(Usage);
            }
            string className = args[1];

            if (ZOO.objList.ContainsKey(className) == false)
            {
                throw new InvalidClassnameException(className);
            }

            return new string[] { className };
        }
        public void Execute(string[] args)
        {
            string className = args[0];

            foreach (var value in ZOO.objList[className])
            {
                Console.WriteLine(value.name);
            }

        }
    }
    public class ExitCommand : ICommand
    {
        public string Name => "exit";

        public string Description => "exit the application";
        public string Usage => "Usage: exit";

        public string[] Prepare(string[] args)
        {
            Execute(null);
            return null;
        }
 
        public void Execute(string[] args)
        {
            Console.WriteLine("Exiting...");
        }
    }

    public class FindCommand : ICommand
    {
        public string Name => "find";

        public string Description => "find objects of a particular type matching certain conditions";
        public string Usage => "Usage: find <class_name> [<requirement> ...]";
        public string[] Prepare(string[] args)
        {
            if (args.Length < 2)
            {
                throw new InvalidArgumentsException(Usage);
            }

            string className = args[1].ToLower(); 

            if (ZOO.objList.ContainsKey(className) == false)
            {
                throw new InvalidClassnameException(className);

            }

            var requirements = args.Skip(2).ToArray();

            List<string> ret = new List<string> { className };

            foreach (var requirement in requirements)
            {
                ret.Add(requirement);
            }
            return ret.ToArray();



        }
        public void Execute(string[] args)
        {
            string className = args[0];

            var list = ZOO.objList[className];

            if (list.Count == 0)
            {
                throw new NoObjectsFoundException(className);
            }

            List<IObject> result = new List<IObject>();

            if (args.Length == 1)
            {
                foreach (var item in list)
                {
                    result.Add(item);
                }
            }
            else
            {
                var requirements = args.Skip(1).ToArray();

                foreach (var item in list)
                {
                    var obj = (IObject)item;
                    if (CommandFuncs.CheckRequirements(obj, requirements))
                        result.Add(obj);
                }

            }

            foreach(var item in result)
            {
                Console.WriteLine(item.ToString());
            }

        }

    }

    public class AddCommand : ICommand
    {
        public string Name => "add";

        public string Description => "add a new object of a particular type and in one of two representations";

        public string Usage => "Usage: add <class_name> base|secondary";

        public string[] Prepare(string[] args)
        {
            // string classname, string representation, object obj)
            if (args.Length < 3)
            {
                throw new InvalidArgumentsException(Usage);
            }
            string className = args[1];
            if (!ZOO.objList.ContainsKey(className))
            {
                throw new InvalidClassnameException(className);
            }

            string representation = args[2];
            if (representation != "base" && representation != "secondary")
            {
                throw new InvalidValueException("representation", representation);
            }

            return new string[] { className, representation };

        }
        public void Execute(string[] args)
        {
            (string className, string representation) = (args[0], args[1]);

            IObject obj = ZOO.constructors[className]();

            Dictionary<string, string> props = new();

            foreach (var o in obj.getFields)
            {

                if (o.Value() is string || o.Value() is int)
                {
                    props.Add(o.Key, o.Value().ToString());
                }

            }

            Console.Write($"You have entered object creation mode\nPlease enter fields in format ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("<field_name>=<value>\n");
            Console.ResetColor();
            Console.Write("To finish adding the object use ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("DONE");
            Console.ResetColor();
            Console.Write("To exit the creator use ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("EXIT\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Available fields: {string.Join(", ", props.Keys)}");
            Console.ResetColor();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[+] ");
                Console.ResetColor();
                string input = Console.ReadLine();
                if (input == "DONE")
                {
                    break;
                }
                else if (input == "EXIT")
                {
                    throw new EditionAbandonedException(className);
                }

                var entry = input.Split("=");
                try
                {
                    if (entry.Length < 2)
                    {
                        throw new InvalidValueException("entry", input);
                    }
                    if (obj.setFields.ContainsKey(entry[0]) == false)
                    {
                        throw new InvalidValueException("fieldname", input);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    if (obj.getFields[entry[0]]() is int)
                        int.Parse(entry[1]);
                    else
                        props[entry[0]] = entry[1];
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid type: {entry[1]} should be {obj.getFields[entry[0]]().GetType()}");
                    Console.ResetColor();
                    continue;
                }
            }

            foreach (var p in props)
            {
                obj.setFields[p.Key](p.Value);
            }

            if (representation == "base")
            {
                ZOO.objList[className].Add(obj);
            }
            else
            {
                List<object> values = new List<object>();
                foreach (var p in obj.getFields)
                    values.Add(obj.getFields[p.Key]());

                IObject objAdapter = ZOO.adapterConstructors[className](values.ToArray());
                ZOO.objList[className].Add(objAdapter);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\nObject created: ");
            Console.ResetColor();
            Console.WriteLine($"{className} {obj.ToString()}");
        }

    }

    public class EditCommand : ICommand
    {
        public string Name => "edit";

        public string Description => "edit objects of a particular type matching certain conditions";
        public string Usage => "Usage: edit <class_name> [<requirement> ...]";

        public string[] Prepare(string[] args)
        {

            if (args.Length < 2)
            {
                throw new InvalidArgumentsException(Usage);
            }

            string className = args[1].ToLower();

            if (ZOO.objList.ContainsKey(className) == false)
            {
                throw new InvalidClassnameException(className);
            }

            var requirements = args.Skip(2).ToArray();

            List<string> ret = new List<string> { className };

            foreach(var requirement in requirements)
            {
                ret.Add(requirement);
            }
            return ret.ToArray();
        }
        public void Execute(string[] args)
        {
            string className = args[0];
            var requirements = args.Skip(1).ToArray();

            var list = ZOO.objList[className];

            if (list.Count == 0)
            {
                throw new NoObjectsFoundException(className);
            }

            List<IObject> matches = new();

            foreach (var item in list)
            {
                var _obj = (IObject)item;
                if (CommandFuncs.CheckRequirements(_obj, requirements))
                    matches.Add(_obj);
            }
            if (matches.Count != 1)
            {

                throw new NotUniqueMatchException(className, matches.Select(x => x.name).ToList());
            }

            IObject obj = matches[0];
            //IObject editingObj = ZOO.constructors[className]();
            int index = ZOO.objList[className].IndexOf(obj);


            Dictionary<string, string> props = new();
            foreach (var o in obj.getFields)
            {

                if (o.Value() is string || o.Value() is int)
                {
                    props.Add(o.Key, o.Value().ToString());
                }

            }

            Console.Write($"You have entered object edition mode\nPlease enter fields in format ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("<field_name>=<value>\n");
            Console.ResetColor();
            Console.Write("To finish adding the object use ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("DONE");
            Console.ResetColor();
            Console.Write("To exit the creator use ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("EXIT\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Available fields: {string.Join(", ", props.Select(x => x.Key))}");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[\\] ");
                Console.ResetColor();
                string input = Console.ReadLine();
                if (input == "DONE")
                {
                    break;

                }
                else if (input == "EXIT")
                {
                    throw new EditionAbandonedException(className);
                }

                var entry = input.Split("=");
                try
                {
                    if (entry.Length < 2)
                    {
                        throw new InvalidValueException("entry", input);
                    }
                    if (props.ContainsKey(entry[0]) == false)
                    {
                        throw new InvalidValueException("fieldname", input);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                    continue;
                }


                try
                {
                    //editingObj.setFields[entry[0]](entry[1]);
                    if (obj.getFields[entry[0]]() is int)
                        int.Parse(entry[1]);
                    else
                        props[entry[0]] = entry[1];
                }
                catch
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid type: {entry[1]} should be {obj.getFields[entry[0]]().GetType()}");
                    Console.ResetColor();
                    continue;
                }
            }

            foreach(var p in props)
            {
                obj.setFields[p.Key](p.Value);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\nObject edited: ");
            Console.ResetColor();
            Console.WriteLine($"{className} {obj.ToString()}");
        }
    }

    public class DeleteCommand : ICommand
    {
        public string Name => "delete";
        public string Description => "delete objects of a particular type matching certain conditions";
        public string Usage => "Usage: delete <class_name> [<requirement> ...]";

        public string[] Prepare(string[] args)
        {

            if (args.Length < 2)
            {
                throw new InvalidArgumentsException(Usage);
            }

            string className = args[1].ToLower();

            if (ZOO.objList.ContainsKey(className) == false)
            {
                throw new InvalidClassnameException(className);
            }


            var requirements = args.Skip(2).ToArray();

            List<string> ret = new List<string> { className };

            foreach (var requirement in requirements)
            {
                ret.Add(requirement);
            }
            return ret.ToArray();

        }
        public void Execute(string[] args)
        {
            string className = args[0];

            var list = ZOO.objList[className];
            var requirements = args.Skip(1).ToArray();

            if (list.Count == 0)
            {
                throw new NoObjectsFoundException(className);
            }

            List<IObject> matches = new();

            foreach (var item in list)
            {
                var _obj = (IObject)item;
                if (CommandFuncs.CheckRequirements(_obj, requirements))
                    matches.Add(_obj);
            }
            if (matches.Count != 1)
            {
                throw new NotUniqueMatchException(className, matches.Select(x => x.name).ToList());
            }

            IObject obj = matches[0];
            int index = ZOO.objList[className].IndexOf(obj);


            ZOO.objList[className].RemoveAt(index);
        }
    }

    public class QueueCommand : ICommand
    {
        public Dictionary<string, ICommand> queueCommands = new();
        CommandQueue queue;
        public string Name => "queue";
        public string Description => "";
        public string Usage => "";
        public QueueCommand(CommandQueue q)
        {
            queue = q;
            queueCommands.Add("print", new QueuePrint(q));
            queueCommands.Add("commit", new QueueCommit(q));
            queueCommands.Add("export", new QueueExport(q));
            queueCommands.Add("dismiss", new QueueDismiss(q));
            queueCommands.Add("load", new QueueLoad(q));
        }

        public string[] Prepare(string[] args)
        {
            if (args.Length < 2)
                throw new InvalidArgumentsException(Usage);
            string[] subargs = args.Skip(1).ToArray();

            if (!queueCommands.ContainsKey(subargs[0]))
            {
                throw new InvalidArgumentsException(Usage);
            }
            ICommand cmd = queueCommands[subargs[0]];
            cmd.Prepare(subargs);
            Execute(null);
            return null;

        }
        public void Execute(string[] args)
        {

        }

    }
    public class QueuePrint : ICommand
    {
        CommandQueue queue;
        public string Name => "print";

        public string Description => "print all objects in queue";
        public string Usage => "Usage: queue print";
        public QueuePrint(CommandQueue q)
        {
            queue = q;
        }
        public string[] Prepare(string[] args)
        {
            Execute(null);
            return null;
        }
        public void Execute(string[] args)
        {
            foreach(var i in queue.inputs)
            {
                Console.WriteLine(i);
            }
        }
    }
    public class QueueCommit : ICommand
    {
        CommandQueue queue;
        public string Name => "commit";
        public string Description => "execute all objects in queue";
        public string Usage => "Usage: queue commit";
        public QueueCommit(CommandQueue q)
        {
            queue = q;
        }
        public string[] Prepare(string[] args)
        {
            Execute(null);
            return null;
        }
        public void Execute(string[] args)
        {
            int j = 0;
            foreach (var i in queue.commands)
            { 
                ICommand cmd = MConsole.commands[i.commandName];
                try
                {
                    Console.WriteLine($">>> {queue.inputs[j]}");
                    cmd.Execute(i.commandArguments);
                    Console.WriteLine();
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
                j++;
            }
            queue.commands.Clear();
            queue.inputs.Clear();
        }
    }
    public class QueueExport : ICommand
    {
        CommandQueue queue;
        public string Name => "export";
        public string Description => "export all objects in queue to a file of a specified format";
        public string Usage => "Usage: queue export filename [xml|plaintext]";
        public QueueExport(CommandQueue q)
        {
            queue = q;
        }
        public string[] Prepare(string[] args)
        {
            string filename, format;
            if (args.Length != 2 && args.Length != 3)
                throw new InvalidArgumentsException(Usage);

            if (args.Length == 2)
                format = "xml";
            else
                format = args[2];
            filename = args[1];

            Execute(new string[] { filename, format });
            return null;
        }
        public void Execute(string[] args)
        {
            (string filename, string format) = (args[0], args[1]);
            CommandFuncs.ExportToFile(queue, filename, format);
        }
    }

    public class QueueDismiss : ICommand
    {
        CommandQueue queue;
        public string Name => "dismiss";
        public string Description => "clear all objects in queue";
        public string Usage => "Usage: queue dismiss";
        public QueueDismiss(CommandQueue q)
        {
            queue = q;
        }
        public string[] Prepare(string[] args)
        {
            Execute(null);
            return null;
        }
        public void Execute(string[] args)
        {
            queue.commands.Clear();
            queue.inputs.Clear();
        }
    }

    public class QueueLoad : ICommand
    {
        CommandQueue queue;
        public string Name => "load";
        public string Description => "load commands from a file";
        public string Usage => "Usage: queue load filename";
        public QueueLoad(CommandQueue q)
        {
            queue = q;
        }
        public string[] Prepare(string[] args)
        {
            if(args.Length != 2)
            {
                throw new InvalidArgumentsException(Usage);
            }
            string[] file = args[1].Split('.');
            string filename = args[1];
            string format = file[1];
            if(format.ToLower() != "xml" && format.ToLower() != "txt")
            {
                throw new InvalidValueException("format", format);
            }

            Execute(new string[] { filename, format });
            return null;
            
        }
        public void Execute(string[] args)
        {
            (string filename, string format) = (args[0], args[1]);
            CommandQueue q = CommandFuncs.DeserializeFromFile(filename, format);
            foreach(var c in q.commands)
                queue.commands.Add(c);
            foreach(var i in q.inputs)
                queue.inputs.Add(i);

        }
    }

    public static class CommandFuncs
    {
        public static void ExportToFile(CommandQueue q, string fileName, string format)
        {
            if (format.ToLower() == "xml")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CommandQueue));
                using StreamWriter writer = new StreamWriter(fileName);
                serializer.Serialize(writer, q);
            }
            else if (format.ToLower() == "plaintext")
            {
                using StreamWriter writer = new StreamWriter(fileName);
                foreach (string input in q.inputs)
                {
                    writer.WriteLine(input);
                }
            }
            else
            {
                throw new ArgumentException("Invalid export format. Supported formats are XML and plaintext.");
            }
        }
        public static CommandQueue DeserializeFromFile(string fileName, string format)
        {
            if (format.ToLower() == "xml")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CommandQueue));
                using (StreamReader reader = new StreamReader(fileName))
                {
                    return (CommandQueue)serializer.Deserialize(reader);
                }
            }
            else if (format.ToLower() == "txt")
            {
                CommandQueue queue = new CommandQueue();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        queue.inputs.Add(line);
                        string[] args = line.Split(' ');
                        string[] obj = MConsole.commands[args[0]].Prepare(args);
                        queue.commands.Add(new SerializableCommandObject(args[0], obj)); 


                    }
                }
                return queue;
            }
            else
            {
                throw new ArgumentException("Invalid export format. Supported formats are XML and plaintext.");
            }
        }

        public static bool CheckRequirements(IObject value, string[] requirements)
        {
            foreach (var requirement in requirements)
            {
                var splitRequirement = requirement.Split(new[] { "=", "<", ">" }, StringSplitOptions.RemoveEmptyEntries);

                if (splitRequirement.Length != 2)
                {
                    throw new InvalidValueException("requirement", requirement);
                }

                var op = requirement.Substring(requirement.IndexOf(splitRequirement[1]) - 1, 1);
                var reqValue = splitRequirement[1];
                var field = splitRequirement[0];

                switch (op)
                {
                    case "=":
                        if (reqValue.CompareTo(value.getFields[field]().ToString()) != 0)
                        {
                            return false;
                        }
                        break;
                    case "<":
                        if (value.getFields[field]() is int)
                        {
                            int reqInt = int.Parse(reqValue);
                            int valInt = (int)value.getFields[field]();
                            if (valInt >= reqInt)
                                return false;
                        }
                        else if (reqValue.CompareTo(value.getFields[field]().ToString()) != 1)
                        {
                            return false;
                        }
                        break;
                    case ">":
                        if (value.getFields[field]() is int)
                        {
                            int reqInt = int.Parse(reqValue);
                            int valInt = (int)value.getFields[field]();
                            if (valInt <= reqInt)
                                return false;
                        }
                        else if (reqValue.CompareTo(value.getFields[field]().ToString()) != -1)
                        {
                            return false;
                        }
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Invalid operator: {op}");
                        Console.ResetColor();
                        return false;
                }
            }

            return true;
        }
    }
}
