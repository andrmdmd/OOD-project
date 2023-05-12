using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public static class ZOO
    {
        public static List<IVisitor> visitors = new();
        public static List<IEnclosure> enclosures = new();
        public static List<IEmployee> employees = new();
        public static List<IAnimal> animals = new();
        public static List<ISpecies> species = new();

        public static Dictionary<string, List<IObject>> objList = new();
        public static Dictionary<string, Func<IObject>> constructors = new();
        public static Dictionary<string, Func<object[], IObject>> adapterConstructors = new();

        static ZOO()
        {
            objList.Add("visitor", new List<IObject>());
            objList.Add("enclosure", new List<IObject>());
            objList.Add("employee", new List<IObject>());
            objList.Add("animal", new List<IObject>());
            objList.Add("species", new List<IObject>());

            constructors = new Dictionary<string, Func<IObject>>{
                {"visitor", () => new Visitor() },
                {"enclosure", () => new Enclosure() },
                {"employee", () => new Employee() },
                {"animal", () => new Animal() },
                {"species", () => new Species() }

            };

            adapterConstructors = new Dictionary<string, Func<object[], IObject>>
            {
                ["visitor"] = (object[] obj) => new VisitorAdapterS(new VisitorS((string)obj[0], (string)obj[1])),
                ["enclosure"] = (object[] obj) => new EnclosureAdapterS(new EnclosureS((string)obj[0])),
                ["employee"] = (object[] obj) => new EmployeeAdapterS(new EmployeeS((string)obj[0], (string)obj[1], (int)obj[2])),
                ["animal"] = (object[] obj) => new AnimalAdapterS(new AnimalS((string)obj[0], (int)obj[1])),
                ["species"] = (object[] obj) => new SpeciesAdapterS(new SpeciesS((string)obj[0]))
            };
    }
    }

    public class Visitor : IVisitor
    {
        public string name { get; set; }
        public string surname { get; set; }
        public List<IEnclosure> visitedEnclosures { get; set; }

        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public Visitor(string name_ = "", string surname_ = "", List<IEnclosure> visitedEnclosures_ = null ) {
            this.name = name_;
            this.surname = surname_;
            this.visitedEnclosures = visitedEnclosures_ != null ? new List<IEnclosure>(visitedEnclosures_) : new List<IEnclosure>();
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["surname"] = () => surname,
                ["visited_enclosures"] = () => visitedEnclosures
            };
            setFields = new Dictionary<string, Action<object>>
            {
                ["name"] = value => name = (string)value,
                ["surname"] = value => surname = (string)value,
                ["visited_enclosures"] = value => visitedEnclosures = (List<IEnclosure>)value

            };
        }
        public override string ToString() =>
            name + ", " + surname + ", [" + string.Join(", ", visitedEnclosures.Select(e => e.name)) + ']';
    }
    public class Enclosure : IEnclosure
    {
        public string name { get; set; }
        public List<IAnimal> animals { get; set; }
        public IEmployee employee { get; set; }
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }

        public Enclosure(string name_ = "", List<IAnimal> animals_ = null, IEmployee employee_ = null)
        {
            this.name = name_;
            this.animals = animals_ == null ? new List<IAnimal>() : new List<IAnimal>(animals_);
            this.employee = employee_;
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["animals"] = () => animals,
                ["employee"] = () => employee
            };
            setFields = new Dictionary<string, Action<object>>
            {
                ["name"] = value => name = (string)value,
                ["animals"] = value => animals = (List<IAnimal>)value,
                ["employee"] = value => employee = (IEmployee)value
            };
        }

        public override string ToString() =>
            name + ", [" + string.Join(", ", animals.Select(e => e.species.name)) + ']';
    }
    public class Employee : IEmployee
    {
        public string name { get; set; }
        public string surname { get; set; }
        public int age { get; set; }
        public List<IEnclosure> enclosures { get; set; }
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }

        public Employee(string name_ = "", string surname_ = "", int age_ = 0, List<IEnclosure> enclosures_ = null)
        {
            this.name = name_;
            this.surname = surname_;
            this.age = age_;
            this.enclosures = enclosures_ == null ? new List<IEnclosure>() : new List<IEnclosure>(enclosures_);
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["surname"] = () => surname,
                ["enclosures"] = () => enclosures,
                ["age"] = () => age
            };
            setFields = new Dictionary<string, Action<object>>
            {
                ["name"] = value => name = (string)value,
                ["surname"] = value => surname = (string)value,
                ["enclosures"] = value => enclosures = (List<IEnclosure>)value,
                ["age"] = value => age = int.Parse((string)value),
            };
        }

        public override string ToString() =>
            name + ", " + surname + ", [" + string.Join(", ", enclosures.Select(e => e.name)) + ']';
    }
    public class Animal : IAnimal 
    {
        public string name { get; set; }
        public int age { get; set; }
        public ISpecies species { get; set; }
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }

        public Animal(string name_ = "", int age_ = 0, ISpecies species_ = null)
        {
            this.name = name_;
            this.age = age_;
            this.species = species_;
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["age"] = () => age,
                ["species"] = () => species
            };
            setFields = new Dictionary<string, Action<object>>
            {
                ["name"] = value => name = (string)value,
                ["age"] = value => age = int.Parse((string)value),
                ["species"] = value => species = (ISpecies)value
            };
        }
        public override string ToString() =>
            name + ", " + age.ToString() + (species == null ? "" : ", " + species.name);
    }
    public class Species : ISpecies
    {
        public string name { get; set; }
        public List<ISpecies>? favoriteFoods { get; set; }
        public Dictionary<string, Func<object>> getFields {  get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }

        public Species(string name_ = "", List<ISpecies>? favoriteFoods_ = null)
        {
            this.name = name_;
            if (favoriteFoods_ != null)
            {
                this.favoriteFoods = new List<ISpecies>(favoriteFoods_);
            }
            else this.favoriteFoods = null;
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["favorite_foods"] = () => favoriteFoods
            };
            setFields = new Dictionary<string, Action<object>>
            {
                ["name"] = value => name = (string)value,
                ["favorite_foods"] = value => favoriteFoods = (List<ISpecies>)value
            };
        }

        public override string ToString() =>
            favoriteFoods == null ? name + ", []" : name + ", [" + string.Join(", ", favoriteFoods.Select(e => e.name)) + ']';
    }

}
