using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public interface IObject 
    {
        public Dictionary<string, Func<object>> getFields { get; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public string name { get; }
        public string ToString();
    }
    public interface IVisitor : IObject
    {
        public string name { get; }
        public string surname { get; }
        public List<IEnclosure> visitedEnclosures { get; }
        public string ToString();
    }

    public interface IEnclosure : IObject
    {
        public string name { get; }
        public List<IAnimal> animals { get; }
        public IEmployee employee { get; }
        public string ToString();
    }

    public interface IAnimal : IObject
    {
        public string name { get; }
        public int age { get; }
        public ISpecies species { get; }
    }
    public interface IEmployee : IObject
    {
        public string name { get; }
        public string surname { get; }
        public int age { get; }
        public List<IEnclosure> enclosures { get; }
    }
    public interface ISpecies : IObject
    {
        public string name { get; }
        public List<ISpecies>? favoriteFoods { get; }
    }
}
