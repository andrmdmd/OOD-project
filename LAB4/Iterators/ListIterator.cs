using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class ListIterator<T> : IIterator<T>
    {
        Node<T>? current;
        public ListIterator(Node<T> c)
        {
            current = c;
        }
        public IIterator<T> Next()
        {
            current = current!.next;
            return this;
        }
        public T Value()
        {
            return current!.value;
        }
        public bool HasNext()
        {
            return current!.next != null;
        }
    }
}
