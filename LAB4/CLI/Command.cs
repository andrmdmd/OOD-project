using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class ListCommand : ICommand
    {
        public string Name { get; } = "list";

        public string Description { get; } = "print all objects of a particular type";

        public string Usage => "Usage: list [--<option>] <class name>";

        public void Execute(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(Usage);
                Console.ResetColor();
                return;
            }
            string opt = args.Length == 3 ? args[1] : "";
            string className = args.Length == 2 ? args[1] : args[2];
            if (className == "--all")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Valid invocations of 'list':");
                Console.ResetColor();
                foreach (var e in ZOO.objList)
                {
                    Console.WriteLine("list " + e.Key);
                }
                return;
            }
            if (ZOO.objList.ContainsKey(className) == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid input. To see things you can list write 'list --all'");
                Console.ResetColor();
                return;
            }
            if (opt == "--verbose")
            {
                foreach (var value in ZOO.objList[className])
                {
                    Console.WriteLine(value.ToString());
                }
            }
            else
            {
                foreach (var value in ZOO.objList[className])
                {
                    Console.WriteLine(value.name);
                }
            }
        }
    }
    public class ExitCommand : ICommand
    {
        public string Name => "exit";

        public string Description => "exit the application";
        public string Usage => "Usage: exit";

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

        public void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(Usage);
                Console.ResetColor();
                return;
            }

            string className = args[1].ToLower();

            if (ZOO.objList.ContainsKey(className) == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid class name: {className}");
                Console.ResetColor();
                return;
            }

            var list = ZOO.objList[className];

            if (list.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No objects of type {className} found");
                Console.ResetColor();
                return;
            }

            if (args.Length == 2)
            {
                foreach (var item in list)
                {
                    Console.WriteLine(item);
                }
            }

            var requirements = args.Skip(2).ToArray();

            foreach (var item in list)
            {
                var obj = (IObject)item;
                if (CommandFuncs.CheckRequirements(obj, requirements))
                    Console.WriteLine(obj);


            }

        }


    }

    public class AddCommand : ICommand
    {
        public string Name => "add";

        public string Description => "add a new object of a particular type and in one of two representations";

        public string Usage => "Usage: add <class_name> base|secondary";

        public void Execute(string[] args)
        {
            if(args.Length < 3)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(Usage);
                Console.ResetColor();
                return;
            }
            string className = args[1];
            if (!ZOO.objList.ContainsKey(className))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid class name: {className}");
                Console.ResetColor();
                return;
            }

            string representation = args[2];
            if (representation != "base" && representation != "secondary")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid representation: {representation}");
                Console.ResetColor();
                return;
            }

            IObject obj = ZOO.constructors[className]();
            List<string> fieldNames = new List<string>();  
            foreach(var o in obj.setFields)
            {
                if (obj.getFields[o.Key]() is string || obj.getFields[o.Key]() is int)    
                    fieldNames.Add(o.Key);
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
            Console.WriteLine($"Available fields: {string.Join(", ", fieldNames)}");
            Console.ResetColor();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[+] ");
                Console.ResetColor();
                string input = Console.ReadLine();
                if (input == "DONE")
                {
                    if(representation == "base")
                    {
                        ZOO.objList[className].Add(obj);
                    }
                    else
                    {
                        List<object> values = new List<object>();
                        foreach (var prop in obj.getFields)
                            values.Add(obj.getFields[prop.Key]());
                        IObject objAdapter = ZOO.adapterConstructors[className](values.ToArray());
                        ZOO.objList[className].Add(objAdapter);

                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"\nObject created: "); 
                    Console.ResetColor();
                    Console.WriteLine($"{className} {obj.ToString()}");
                    break;
                }
                else if (input == "EXIT")
                {
                    Console.WriteLine($"\nCreation of {className} abandoned");
                    break;
                }

                var entry = input.Split("=");
                if(entry.Length < 2 )
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid entry {input}");
                    Console.ResetColor();
                    continue;
                }
                if (obj.setFields.ContainsKey(entry[0]) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid fieldname {entry[0]}");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    obj.setFields[entry[0]](entry[1]);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid type: {entry[1]} should be {obj.getFields[entry[0]]().GetType()}");
                    Console.ResetColor();
                    continue;
                }
                
            }


        }
    }

    public class EditCommand : ICommand
    {
        public string Name => "edit";

        public string Description => "edit objects of a particular type matching certain conditions";
        public string Usage => "Usage: edit <class_name> [<requirement> ...]";

        public void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(Usage);
                Console.ResetColor();
                return;
            }

            string className = args[1].ToLower();

            if (ZOO.objList.ContainsKey(className) == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid class name: {className}");
                Console.ResetColor();
                return;
            }

            var list = ZOO.objList[className];

            if (list.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No objects of type {className} found");
                Console.ResetColor();
                return;
            }

            var requirements = args.Skip(2).ToArray();
            List<IObject> matches = new();

            foreach (var item in list)
            {
                var obj = (IObject)item;
                if (CommandFuncs.CheckRequirements(obj, requirements))
                    matches.Add(obj);
            }
            if(matches.Count != 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Found {matches.Count} objects of type {className}:");
                foreach(var item in matches)
                {
                    Console.WriteLine($"{item}");
                }
                Console.WriteLine("Requirements should specify an unique record.");
                Console.ResetColor();
                return;
            }

            IObject originalObj = matches[0];
            IObject editingObj = ZOO.constructors[className]();

            List<string> fieldNames = new List<string>();
            foreach (var o in editingObj.setFields)
            {
                
                if (editingObj.getFields[o.Key]() is string || editingObj.getFields[o.Key]() is int)
                {
                    fieldNames.Add(o.Key);
                    if(editingObj.getFields[o.Key]() is int)
                        editingObj.setFields[o.Key](originalObj.getFields[o.Key]().ToString());
                    else editingObj.setFields[o.Key](originalObj.getFields[o.Key]());
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
            Console.WriteLine($"Available fields: {string.Join(", ", fieldNames)}");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[\\] ");
                Console.ResetColor();
                string input = Console.ReadLine();
                if (input == "DONE")
                {

                    ZOO.objList[className][ZOO.objList[className].IndexOf(originalObj)] = editingObj;


                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"\nObject edited: ");
                    Console.ResetColor();
                    Console.WriteLine($"{className} {editingObj.ToString()}");
                    break;
                }
                else if (input == "EXIT")
                {
                    Console.WriteLine($"\nEdition of {className} abandoned");
                    break;
                }

                var entry = input.Split("=");
                if (entry.Length < 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid entry {input}");
                    Console.ResetColor();
                    continue;
                }
                if (editingObj.setFields.ContainsKey(entry[0]) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid fieldname {entry[0]}");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    editingObj.setFields[entry[0]](entry[1]);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid type: {entry[1]} should be {editingObj.getFields[entry[0]]().GetType()}");
                    Console.ResetColor();
                    continue;
                }

            }

        }
    }

    public static class CommandFuncs
    {

        public static bool CheckRequirements(IObject value, string[] requirements)
        {
            foreach (var requirement in requirements)
            {
                var splitRequirement = requirement.Split(new[] { "=", "<", ">" }, StringSplitOptions.RemoveEmptyEntries);

                if (splitRequirement.Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid requirement format: {requirement}");
                    Console.ResetColor();
                    return false;
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
