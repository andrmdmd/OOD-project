using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class SortedArray<T> : ICollection<T>
    {
        public List<T> array;
        IComparer<T> comparer;

        public SortedArray(IComparer<T> comp)
        {
            array = new List<T>();
            comparer = comp;
            
        }
        public void AddObject(T obj)
        {
            array.Add(obj); int index = 0;
            while (comparer.Compare(array[index], obj) == 1)
                index++;
            for (int i = array.Count - 1; i > index; i--)
                array[i] = array[i - 1];
            array[index] = obj;

        }
        public bool DeleteObject(T obj)
        { 
            return array.Remove(obj);
        }
        public void Delete()
        {
            array.RemoveAt(array.Count - 1);
        }
        public int GetLength() => array.Count;
        public int Find(T obj)
        {
            int l = 0, r = array.Count, m;
            while (l < r)
            {
                m = (l - r) / 2;
                switch (comparer.Compare(array[m], obj))
                {
                    case 0:
                        return m;
                    case 1:
                        l = m;
                        break;
                    case -1:
                        r = m;
                        break;
                }

            }
            return -1;
        }
        public IIterator<T> GetForwardIterator() => new SortedArrayIterator<T>(this);
        public IIterator<T> GetReverseIterator() => new ReverseSortedArrayIterator<T>(this);
    }
    public class SortedArrayIterator<T> : IIterator<T>
    {
        SortedArray<T> sortedArray;
        int index;

        public SortedArrayIterator(SortedArray<T> array)
        {
            this.sortedArray = array;
            index = 0;
        }

        public IIterator<T>? Next()
        {
            if (this.HasNext())
            {
                index++;
                return this;
            }
            return null;
        }
        public T Value() => sortedArray.array[index];
        public bool HasNext() => index < sortedArray.array.Count - 1;
    }
    public class ReverseSortedArrayIterator<T> : IIterator<T>
    {
        SortedArray<T> sortedArray;
        int index;

        public ReverseSortedArrayIterator(SortedArray<T> array)
        {
            this.sortedArray = array;
            index = array.array.Count - 1;
        }

        public IIterator<T> Next()
        {
            if (this.HasNext())
            {
                index--;
                return this;
            }
            return null;
        }
        public T Value() => sortedArray.array[index];
        public bool HasNext() => index > 0;
    }

}
