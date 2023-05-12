using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
	public interface IIterator<T>
	{
		public IIterator<T> Next();
		public T Value();
		public bool HasNext();
	}
}
