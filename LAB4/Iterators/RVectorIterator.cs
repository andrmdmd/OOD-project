using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class RVectorIterator<T> : IIterator<T>
    {
        public MyVector<T> vector;
        public int index;
        public RVectorIterator(MyVector<T> v)
        {
            vector = v;
            index = vector.array.Length - 1;
        }
        public IIterator<T> Next()
        {
            index--;
            return this;
        }
        public T Value()
        {
            return vector.array[index];
        }
        public bool HasNext()
        {
            return index > 0;
        }
    }
}

