using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
	public class MyVector<T> : ICollection<T>
	{
		public T[] array;
		public MyVector()
		{
			array = new T[0];
		}
		public void AddObject(T obj)
		{
			Array.Resize(ref array, array.Length + 1);
			array[array.Length - 1] = obj;
		}
		public bool DeleteObject(T obj)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(obj))
				{
					array[i] = array[array.Length - 1];
					Array.Resize(ref array, array.Length - 1);
					return true;
				}
			}
			return false;
		}
		public int GetLength()
        {
			return array.Length;
        }
        public IIterator<T> GetForwardIterator()
        {
			return new VectorIterator<T>(this);
		}
        public IIterator<T> GetReverseIterator()
        {
			return new RVectorIterator<T>(this);
		}
		
	}
}
