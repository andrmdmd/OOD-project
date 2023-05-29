using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class InvalidArgumentsException : Exception
    {
        string usage;
        public InvalidArgumentsException(string usage)
        {
            this.usage = usage;
        }
        public override string ToString()
        {
            return usage;
        }

    }
    public class InvalidClassnameException : Exception
    {
        string classname;
        public InvalidClassnameException(string classname)
        {
            this.classname = classname;
        }
        public override string ToString()
        {
            return "Invalid class name: " + classname;
        }
    }
    public class NoObjectsFoundException : Exception
    {
        string classname;
        public NoObjectsFoundException(string classname)
        {
            this.classname = classname;
        }
        public override string ToString()
        {
            return "No objects of type " + classname + " found";
        }
    }
    public class InvalidValueException : Exception
    {
        string valueName, input;

        public InvalidValueException(string valueName, string input)
        {
            this.valueName = valueName;
            this.input = input;
        }
        public override string ToString()
        {
            return "Invalid " + valueName + ": " + input;
        }
    }
    public class InvalidTypeException : Exception
    {
        string typeName, value;
        public InvalidTypeException(string typeName, string value)
        {
            this.typeName = typeName;
            this.value = value;
        }
        public override string ToString()
        {
            return "Invalid type: " + value + " should be of type " + typeName;
        }
    }
    public class NotUniqueMatchException : Exception
    {
        string className;
        List<string> matches;

        public NotUniqueMatchException(string className, List<string> matches)
        {
            this.className = className;
            this.matches = matches;
        }
        public override string ToString()
        {
            string output = $"Found {matches.Count} objects of type {className}:\n";
            foreach (var item in matches)
            {
                output += item + "\n";
            }
            return output;
        }
    }
    public class EditionAbandonedException : Exception
    {
        string className;
        public EditionAbandonedException(string className)
        {
            this.className = className;
        }
        public override string ToString()
        {
            return "Editon of " + className + " abandoned";
        }
    }
}
