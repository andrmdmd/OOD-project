using System;

namespace LAB
{
	public static class Algorithm<T>
    {
        public static void Print(ICollection<T> c)
        {
            IIterator<T> i = c.GetForwardIterator();
            while (i != null)
            {
                Console.WriteLine(i.Value());
                i = i.Next();
            }
        }
        public static void Print(ICollection<T> c, Predicate<T> pred)
        {
            IIterator<T> i = c.GetForwardIterator();
            while (i != null)
            {
                if (pred(i.Value()))
                {
                    Console.WriteLine(i.Value());
                }
                i = i.Next();
            }
        }
        public static void PrintReverse(ICollection<T> c)
        {
            IIterator<T> i = c.GetReverseIterator();
            while (i != null)
            {
                Console.WriteLine(i.Value());
                i = i.Next();
            }
        }
        public static T? Find(ICollection<T> c, Predicate<T> pred)
        {
            IIterator<T> i = c.GetForwardIterator();

            while (i != null)
            {
                if (pred(i.Value()))
                    return i.Value();
                i = i.Next();
            }
            return default(T);
        }
        public static void ForEach(IIterator<T> it, Action<T> f)
        {
            while (it != null)
            {
                f(it.Value());
                it = it.Next();
            }
        }
        public static int CountIf(ICollection<T> c, Predicate<T> pred)
        {
            IIterator<T> it = c.GetForwardIterator();
            int i = 0;
            while (it != null)
            {
                if (pred(it.Value()))
                    i++;
                it = it.Next();
            }
            return i;
        }
    }
	
}
