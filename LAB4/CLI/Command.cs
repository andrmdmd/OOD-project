using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class ListCommand : ICommand
    {
        public string Name { get; } = "list";

        public string Description { get; } = "print all objects of a particular type";

        public string Usage => "Usage: list <class name>";

        public void Execute(string[] args)
        {
            if (args.Length != 2)
                throw new InvalidArgumentsException(Usage);

            string className = args[1];

            if (ZOO.objList.ContainsKey(className) == false)
                throw new InvalidClassnameException(className);

            foreach (var value in ZOO.objList[className])
                Console.WriteLine(value.name);
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
                throw new InvalidArgumentsException(Usage);

            string className = args[1].ToLower();

            if (ZOO.objList.ContainsKey(className) == false)
                throw new InvalidClassnameException(className);

            var requirements = args.Skip(2).ToArray();

            var list = ZOO.objList[className];

            if (list.Count == 0)
                throw new NoObjectsFoundException(className);

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
                foreach (var item in list)
                {
                    var obj = (IObject)item;
                    if (CommandFuncs.CheckRequirements(obj, requirements))
                        result.Add(obj);
                }
            }

            foreach (var item in result)
                Console.WriteLine(item.ToString());
        }
    }

    public class AddCommand : IUndoable
    {
        public string Name => "add";

        public string Description => "add a new object of a particular type and in one of two representations";

        public string Usage => "Usage: add <class_name> base|secondary";

        Stack<(string, IObject)> UndoStack = new();
        Stack<(string, IObject)> RedoStack = new();

        public void Undo()
        {
            if (UndoStack.Count == 0) return;

            (string classname, IObject obj) = UndoStack.Pop();
            ZOO.objList[classname].Remove(obj);
            RedoStack.Push((classname, obj));

        }

        public void Redo()
        {
            if(RedoStack.Count == 0) return;

            (string classname, IObject obj) = RedoStack.Pop();
            ZOO.objList[classname].Add(obj);
            UndoStack.Push((classname, obj));
        }

        public void Execute(string[] args)
        {
            if (args.Length != 3)
                throw new InvalidArgumentsException(Usage);

            string className = args[1];

            if (!ZOO.objList.ContainsKey(className))
                throw new InvalidClassnameException(className);

            string representation = args[2];

            if (representation != "base" && representation != "secondary")
                throw new InvalidValueException("representation", representation);

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

            if (RedoStack.Count > 0)
                RedoStack.Clear();

            foreach (var p in props)
            {
                obj.setFields[p.Key](p.Value);
            }

            if (representation == "base")
            {
                ZOO.objList[className].Add(obj);
                UndoStack.Push((className, obj));
            }
            else
            {
                List<object> values = new List<object>();
                foreach (var p in obj.getFields)
                    values.Add(obj.getFields[p.Key]());

                IObject objAdapter = ZOO.adapterConstructors[className](values.ToArray());
                ZOO.objList[className].Add(objAdapter);
                UndoStack.Push((className, objAdapter));
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\nObject created: ");
            Console.ResetColor();
            Console.WriteLine($"{className} {obj.ToString()}");


        }
    }

    public class EditCommand : IUndoable
    {
        public string Name => "edit";

        public string Description => "edit objects of a particular type matching certain conditions";
        public string Usage => "Usage: edit <class_name> [<requirement> ...]";

        Stack<(string, IObject, IObject)> UndoStack = new();
        Stack<(string, IObject, IObject)> RedoStack = new();


        public void Undo()
        {
            if (UndoStack.Count == 0) return;

            (string classname, IObject originalObj, IObject editedObj) = UndoStack.Pop();
            ZOO.objList[classname].Remove(editedObj);
            ZOO.objList[classname].Add(originalObj);
            RedoStack.Push((classname, originalObj, editedObj));

        }

        public void Redo()
        {
            if (RedoStack.Count == 0) return;

            (string classname, IObject originalObj, IObject editedObj) = RedoStack.Pop();
            ZOO.objList[classname].Remove(originalObj);
            ZOO.objList[classname].Add(editedObj);
            UndoStack.Push((classname, originalObj, editedObj));
        }
        public void Execute(string[] args)
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

            var list = ZOO.objList[className];

            if (list.Count == 0)
            {
                throw new NoObjectsFoundException(className);
            }

            List<IObject> matches = new();

            foreach (var item in list)
            {
                if (CommandFuncs.CheckRequirements(item, requirements))
                    matches.Add(item);
            }
            if (matches.Count != 1)
            {

                throw new NotUniqueMatchException(className, matches.Select(x => x.name).ToList());
            }

            IObject obj = matches[0];
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

            IObject editingObj = ZOO.constructors[className]();
            foreach (var p in props)
            {
                editingObj.setFields[p.Key](p.Value);
            }
            ZOO.objList[className][ZOO.objList[className].IndexOf(obj)] = editingObj;
            if (RedoStack.Count > 0)
            {
                RedoStack.Clear();
            }
            UndoStack.Push((className, obj, editingObj));
        }

        public class DeleteCommand : IUndoable
        {
            public string Name => "delete";
            public string Description => "delete objects of a particular type matching certain conditions";
            public string Usage => "Usage: delete <class_name> [<requirement> ...]";

            Stack<(string, IObject)> UndoStack = new();
            Stack<(string, IObject)> RedoStack = new();

            public void Undo()
            {
                if (UndoStack.Count == 0) return;

                (string classname, IObject obj) = UndoStack.Pop();
                ZOO.objList[classname].Add(obj);
                RedoStack.Push((classname, obj));

            }

            public void Redo()
            {
                if (RedoStack.Count == 0) return;

                (string classname, IObject obj) = RedoStack.Pop();
                ZOO.objList[classname].Remove(obj);
                UndoStack.Push((classname, obj));
            }

            public void Execute(string[] args)
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


                var list = ZOO.objList[className];


                if (list.Count == 0)
                {
                    throw new NoObjectsFoundException(className);
                }

                List<IObject> matches = new();

                foreach (var item in list)
                {
                    if (CommandFuncs.CheckRequirements(item, requirements))
                        matches.Add(item);
                }
                if (matches.Count != 1)
                {
                    throw new NotUniqueMatchException(className, matches.Select(x => x.name).ToList());
                }

                IObject obj = matches[0];


                ZOO.objList[className].Remove(obj);

                UndoStack.Push((className, obj));
                if (RedoStack.Count > 0) RedoStack.Clear();
            }
        }
    }

    public class UndoCommand : ICommand
    {
        public string Name => "undo";
        public string Description => "undo last used command";
        public string Usage => "Usage: undo";

        Stack<IUndoable> UndoStack;
        Stack<IUndoable> RedoStack;
        public UndoCommand(Stack<IUndoable> undoStack, Stack<IUndoable> redoStack)
        {
            UndoStack = undoStack;
            RedoStack = redoStack;
        }

        public void Execute(string[] args)
        {
            if (args.Length != 1)
                throw new InvalidArgumentsException(Usage);

            if (UndoStack.Count == 0)
                return;

            IUndoable command = UndoStack.Pop();
            command.Undo();
            RedoStack.Push(command);
            
            Console.WriteLine($"undo: {command.Name}");

        }
    }

    public class RedoCommand : ICommand
    {
        public string Name => "redo";
        public string Description => "redo last used command";
        public string Usage => "Usage: redo";

        Stack<IUndoable> UndoStack;
        Stack<IUndoable> RedoStack;
        public RedoCommand(Stack<IUndoable> undoStack, Stack<IUndoable> redoStack)
        {
            UndoStack = undoStack;
            RedoStack = redoStack;
        }

        public void Execute(string[] args)
        {
            if (args.Length != 1)
                throw new InvalidArgumentsException(Usage);

            if (RedoStack.Count == 0)
                return;

            IUndoable command = RedoStack.Pop();
            command.Redo();
            UndoStack.Push(command);

            Console.WriteLine($"redo: {command.Name}");

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
