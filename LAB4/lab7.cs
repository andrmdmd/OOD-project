using LAB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB7
{
    public static class Lab7
    {
        public static void lab()
        {
            ZOOAdapterS zoo = Lab5.lab();
            SortedArray<IEnclosure> enclosureSortedArray = new SortedArray<IEnclosure>(Comparer<IEnclosure>.Create((y, x) => x.name.CompareTo(y.name)));
            foreach (var e in zoo.enclosures)
            {
                enclosureSortedArray.AddObject(e);
            }
            Console.WriteLine("--- Print:");
            Algorithm<IEnclosure>.Print(enclosureSortedArray);
            Console.WriteLine("\n--- Reverse print:");
            Algorithm<IEnclosure>.PrintReverse(enclosureSortedArray);
            Console.WriteLine("\n--- Foreach:");
            Algorithm<IEnclosure>.ForEach(enclosureSortedArray.GetForwardIterator(), Console.WriteLine);
            Console.WriteLine("\n--- Find:");
            var x = Algorithm<IEnclosure>.Find(enclosureSortedArray, new Predicate<IEnclosure>(x => x.name.CompareTo("Break") == 0));
            Console.WriteLine(x);
            Console.WriteLine("\n--- CountIf:");
            var y = Algorithm<IEnclosure>.CountIf(enclosureSortedArray, new Predicate<IEnclosure>(x => x.name.CompareTo("Break") != 0));
            Console.WriteLine(y);   


        }
    }
}
