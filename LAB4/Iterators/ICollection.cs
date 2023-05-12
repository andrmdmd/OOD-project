using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public interface ICollection<T>
    {
        public void AddObject(T obj);
        public bool DeleteObject(T obj);
        public int GetLength();
        public IIterator<T> GetForwardIterator();
        public IIterator<T> GetReverseIterator();
    }
}
