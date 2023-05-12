using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class ZOOAdapterS
    {
        public List<IVisitor> visitors = new(); 
        public List<IEnclosure> enclosures = new();
        public List<IEmployee> employees = new();
        public List<IAnimal> animals = new();
        public List<ISpecies> species = new();
        

        public ZOOAdapterS(List<IVisitor> visitors, List<IEnclosure> enclosures, List<IEmployee> employees, List<IAnimal> animals, List<ISpecies> species)
        {
            this.visitors = new List<IVisitor>(visitors);
            this.enclosures = new List<IEnclosure>(enclosures);
            this.employees = new List<IEmployee>(employees);
            this.animals = new List<IAnimal>(animals);
            this.species = new List<ISpecies>(species);

        }
    }
    public class VisitorAdapterS : IVisitor
    {
        VisitorS visitor;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public VisitorAdapterS(VisitorS visitor)
        {
            this.visitor = visitor;
            HMap.idMap.Add(visitor.visitorId, this);
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["surname"] = () => surname,
                ["visited_enclosures"] = () => visitedEnclosures
            };

        }
        public string name
        {
            get
            {
                Stack<string> s = new(new Stack<string>(visitor.visitorTuple.Item2));
                while (s.Pop() != "name")
                    ;
                s.Pop();
                return s.Pop();
            }
        }
        public string surname
        {
            get
            {
                Stack<string> s = new(new Stack<string>(visitor.visitorTuple.Item2));
                while (s.Pop() != "surname")
                    ;
                s.Pop();
                return s.Pop();
            }
        }
        public List<IEnclosure> visitedEnclosures
        {
            get
            {
                List<IEnclosure> enclosures = new();
                Stack<string> s = new(new Stack<string>(visitor.visitorTuple.Item2));
                while (s.Pop() != "enclosures")
                    ;
                int enclosuresNum = int.Parse(s.Pop());
                while (enclosuresNum-- > 0)
                {
                    int id = int.Parse(s.Pop());
                    enclosures.Add((IEnclosure)HMap.idMap[id]);
                }
                return enclosures;
            }
        }
        public override string ToString() =>
            name + ", " + surname + ", [" + string.Join(", ", visitedEnclosures.Select(e => e.name)) + ']';
    }
    public class EnclosureAdapterS : IEnclosure
    {
        EnclosureS enclosure;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public EnclosureAdapterS(EnclosureS enclosure)
        {
            this.enclosure = enclosure;
            HMap.idMap.Add(enclosure.enclosureId, this);
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["animals"] = () => animals,
                ["employee"] = () => employee
            };
        }

        public string name
        {
            get
            {
                Stack<string> s = new(new Stack<string>(enclosure.enclosureTuple.Item2));
                while (s.Pop() != "name")
                    ;
                s.Pop();
                return s.Pop();
            }
        }
        public List<IAnimal> animals
        {
            get
            {
                List<IAnimal> animals = new();
                Stack<string> s = new(new Stack<string>(enclosure.enclosureTuple.Item2));
                while (s.Pop() != "animals")
                    ;
                int animalsNum = int.Parse(s.Pop());
                while (animalsNum-- > 0)
                {
                    int id = int.Parse(s.Pop());
                    animals.Add((IAnimal)HMap.idMap[id]);
                }
                return animals;
            }
        }
        public IEmployee employee
        {
            get
            {
                Stack<string> s = new(new Stack<string>(enclosure.enclosureTuple.Item2));
                while (s.Pop() != "employee")
                    ;
                s.Pop();
                return (IEmployee)HMap.idMap[int.Parse(s.Pop())];
            }
        }
        public override string ToString() =>
            name + ", [" + string.Join(", ", animals.Select(e => e.species.name)) + ']';
    }
    public class EmployeeAdapterS : IEmployee
    {
        EmployeeS employee;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public EmployeeAdapterS(EmployeeS employee)
        {
            this.employee = employee;
            HMap.idMap.Add(employee.employeeId, this);
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["surname"] = () => surname,
                ["enclosures"] = () => enclosures,
                ["age"] = () => age
            };
        }
        public string name
        {
            get
            {
                Stack<string> s = new(new Stack<string>(employee.employeeTuple.Item2));
                while (s.Pop() != "name")
                    ;
                s.Pop();
                return s.Pop();
            }
        }
        public string surname
        {
            get
            {
                Stack<string> s = new(new Stack<string>(employee.employeeTuple.Item2));
                while (s.Pop() != "surname")
                    ;
                s.Pop();
                return s.Pop();
            }
        }
        public int age
        {
            get
            {
                Stack<string> s = new(new Stack<string>(employee.employeeTuple.Item2));
                while (s.Pop() != "age")
                    ;
                s.Pop();
                return int.Parse(s.Pop());
            }
        }
        public List<IEnclosure> enclosures
        {
            get
            {
                List<IEnclosure> enclosures = new();
                Stack<string> s = new(new Stack<string>(employee.employeeTuple.Item2));
                while (s.Pop() != "enclosures")
                    ;
                int enclosuresNum = int.Parse(s.Pop());
                while (enclosuresNum-- > 0)
                {
                    int id = int.Parse(s.Pop());
                    enclosures.Add((IEnclosure)HMap.idMap[id]);
                }
                return enclosures;
            }
        }
        public override string ToString() =>
            name + ", " + surname + ", [" + string.Join(", ", enclosures.Select(e => e.name)) + ']';

    }
    public class AnimalAdapterS : IAnimal
    {
        AnimalS animal;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public AnimalAdapterS(AnimalS animal)
        {
            this.animal = animal;
            HMap.idMap.Add(animal.animalId, this);
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["age"] = () => age,
                ["species"] = () => species
            };
        }
        public string name
        {
            get
            {
                Stack<string> s = new(new Stack<string>(animal.animalTuple.Item2));
                while (s.Pop() != "name")
                    ;
                s.Pop();
                return s.Pop();
            }
        }
        public int age
        {
            get
            {
                Stack<string> s = new(new Stack<string>(animal.animalTuple.Item2));
                while (s.Pop() != "age")
                    ;
                s.Pop();
                return int.Parse(s.Pop());
            }
        }
        public ISpecies species
        {
            get
            {
                Stack<string> s = new(new Stack<string>(animal.animalTuple.Item2));
                while (s.Pop() != "species")
                    ;
                s.Pop();
                return (ISpecies)HMap.idMap[int.Parse(s.Pop())];
            }
        }
        public override string ToString() =>
            name + ", " + age.ToString() + ", " + species.name;

    }
    public class SpeciesAdapterS : ISpecies
    {
        SpeciesS species;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public SpeciesAdapterS(SpeciesS species)
        {
            this.species = species;
            HMap.idMap.Add(species.speciesId, this);
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["favorite_foods"] = () => favoriteFoods
            };
        }

        public string name
        {
            get
            {
                Stack<string> s = new(new Stack<string>(species.speciesTuple.Item2));
                while (s.Pop() != "name")
                    ;
                s.Pop();
                return s.Pop();
            }
        }
        public List<ISpecies>? favoriteFoods
        {
            get
            {
                List<ISpecies> foods = new();
                Stack<string> s = new(new Stack<string>(species.speciesTuple.Item2));
                while (s.Pop() != "favoriteFoods")
                    ;
                int enclosuresNum = int.Parse(s.Pop());
                while (enclosuresNum-- > 0)
                {
                    int id = int.Parse(s.Pop());
                    foods.Add((ISpecies)HMap.idMap[id]);
                }
                return foods;
            }
        }
        public override string ToString() =>
            favoriteFoods == null ? name + ", []" : name + ", [" + string.Join(", ", favoriteFoods.Select(e => e.name)) + ']';
    }
    public static class Lab5
    {
        public static ZOOAdapterS lab()
        {
            ZOOS zoo = new ZOOS();

            // Initializing Enclosure objects
            EnclosureS[] enclosures = new EnclosureS[]
            {
                new EnclosureS("311", new int[]{ 15, 10, 9 }, 2),
                new EnclosureS("Break", new int[]{ 13, 11, 12 }, 3),
                new EnclosureS("Jurasic Park", new int[]{ 6, 8, 12 }, 3)
            };

            // Initializing Animal objects
            AnimalS[] animals = new AnimalS[]
            {
                new AnimalS("Harold", 2, 18),
                new AnimalS("Ryan", 1, 18),
                new AnimalS("Jenkins", 15, 19),
                new AnimalS("Kaka", 10, 19),
                new AnimalS("Ada", 13, 20),
                new AnimalS("Jett", 2, 21),
                new AnimalS("Conda", 4, 22),
                new AnimalS("Samuel", 2, 22),
                new AnimalS("Claire", 2, 23),
                new AnimalS("Andy", 3, 24),
                new AnimalS("Arrow", 5, 25),
                new AnimalS("Arch", 1, 26),
                new AnimalS("Ubuntu", 1, 26),
                new AnimalS("Fedora", 1, 26)
            };

            // Initializing Species objects
            SpeciesS[] species = new SpeciesS[]
            {
                new SpeciesS("Meerkat", new int[]{ 18 }),
                new SpeciesS("Kakapo", new int[]{ }),
                new SpeciesS("Bengal Tiger", new int[]{ 21, 24, 25 }),
                new SpeciesS("Panda", new int[]{ }),
                new SpeciesS("Python", new int[]{ 20, 21 }),
                new SpeciesS("Dungeness Crab", new int[]{ 22 }),
                new SpeciesS("Gopher", new int[]{ }),
                new SpeciesS("Cats", new int[]{ 24 }),
                new SpeciesS("Penguin", new int[]{ 20 })
            };

            // Initializing Employee objects
            EmployeeS[] employees = new EmployeeS[]
            {
                new EmployeeS("Ricardo", "Stallmano", 35, new int[]{ 1 }),
                new EmployeeS("Steve", "Irvin", 40, new int[]{ 2, 3 }),
            };

            // Initializing Visitor objects
            VisitorS[] visitors = new VisitorS[]
            {
                new VisitorS("Tomas", "German", new int[] { 1, 2 }),
                new VisitorS("Silvester", "Ileen", new int[] { 3 }),
                new VisitorS("Basil", "Bailey", new int[] { 1, 3 }),
                new VisitorS("Ryker", "Polly", new int[] { 2 })
            };

            List<IEnclosure> enclosuresA = new();
            List<IEmployee> employeesA = new();
            List<IAnimal> animalsA = new();
            List<IVisitor> visitorsA = new();
            List<ISpecies> speciesA = new();

            foreach (var e in enclosures)
            {
                EnclosureAdapterS en = new(e);
                enclosuresA.Add(en);
            }
            foreach (var e in employees)
            {
                EmployeeAdapterS en = new(e);
                employeesA.Add(en);
            }

            foreach (var a in animals)
            {
                AnimalAdapterS an = new(a);
                animalsA.Add(an);

            }

            foreach (var v in visitors)
            {
                VisitorAdapterS vi = new(v);
                visitorsA.Add(vi);
            }

            foreach (var s in species)
            {
                SpeciesAdapterS sp = new(s);
                speciesA.Add(sp);
            }

            ZOOAdapterS z = new ZOOAdapterS(visitorsA, enclosuresA, employeesA, animalsA, speciesA);
            ZOO.animals = animalsA;
            ZOO.visitors = visitorsA;
            ZOO.species = speciesA;
            ZOO.employees = employeesA;
            ZOO.enclosures = enclosuresA;


            foreach (var e in z.enclosures)
            {
                double avg = e.animals.ToArray().Select(a => a.age).Average();
                if (avg < 3) { }
                    //Console.WriteLine(e.ToString());
            }
            return z;
        }
    }
}
