using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAB;

namespace LAB4
{
    public class ZOOAdapterH
    {
        public List<IVisitor> visitors = new();
        public List<IEnclosure> enclosures = new();
        public List<IEmployee> employees = new();
        public List<IAnimal> animals = new();
        public List<ISpecies> species = new();

        public ZOOAdapterH(List<IVisitor> visitors, List<IEnclosure> enclosures, List<IEmployee> employees, List<IAnimal> animals, List<ISpecies> species)
        {
            this.visitors = new List<IVisitor>(visitors);
            this.enclosures = new List<IEnclosure>(enclosures);
            this.employees = new List<IEmployee>(employees);
            this.animals = new List<IAnimal>(animals);
            this.species = new List<ISpecies>(species);
        }

    }
    public class VisitorAdapterH : IVisitor
    {
        VisitorH visitor;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public VisitorAdapterH(VisitorH visitor)
        {
            this.visitor = visitor;
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["surname"] = () => surname,
                ["visited_enclosures"] = () => visitedEnclosures
            };
        }
        public string name
        {
            get => HMap.hashMap[visitor.nameHash];
        }
        public string surname
        {
            get => HMap.hashMap[visitor.surnameHash];
        }
        public List<IEnclosure> visitedEnclosures
        {
            get
            {
                List<IEnclosure> enclosures = new();
                foreach (var e in visitor.visitedEnclosures)
                {
                    enclosures.Add((IEnclosure)HMap.adMap[e]);
                }
                return enclosures;
            }
        }
        public override string ToString() =>
            name + ", " + surname + ", [" + string.Join(", ", visitedEnclosures.Select(e => e.name)) + ']';



    }
    public class EnclosureAdapterH : IEnclosure
    {
        EnclosureH enclosure;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public EnclosureAdapterH(EnclosureH enclosure)
        {
            this.enclosure = enclosure;
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["animals"] = () => animals,
                ["employee"] = () => employee
            };
        }

        public string name
        {
            get => HMap.hashMap[enclosure.nameHash];
        }
        public List<IAnimal> animals
        {
            get
            {
                List<IAnimal> animals = new();
                foreach (var e in enclosure.animals)
                {
                    animals.Add((IAnimal)HMap.adMap[e]);
                }
                return animals;
            }
        }
        public IEmployee employee
        {
            get => (IEmployee)HMap.adMap[enclosure.employee];
        }
        public override string ToString() =>
            name + ", [" + string.Join(", ", animals.Select(e =>  e.species == null ? "" : e.species.name )) + ']';
    }
    public class EmployeeAdapterH : IEmployee
    {
        EmployeeH employee;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public EmployeeAdapterH(EmployeeH employee)
        {
            this.employee = employee;
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
            get => HMap.hashMap[employee.nameHash];
        }
        public string surname
        {
            get => HMap.hashMap[employee.surnameHash];
        }
        public int age
        {
            get => int.Parse(HMap.hashMap[employee.ageHash]);
        }
        public List<IEnclosure> enclosures
        {
            get
            {
                List<IEnclosure> enclosures = new();
                foreach (var e in employee.enclosures)
                {
                    enclosures.Add((IEnclosure)HMap.adMap[e]);
                }
                return enclosures;
            }
        }
        public override string ToString() =>
            name + ", " + surname + ", [" + string.Join(", ", enclosures.Select(e => e.name)) + ']';

    }
    public class AnimalAdapterH : IAnimal
    {
        AnimalH animal;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }
        public AnimalAdapterH(AnimalH animal)
        {
            this.animal = animal;
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["age"] = () => age,
                ["species"] = () => species
            };
        }
        public string name
        {
            get => HMap.hashMap[animal.nameHash];
        }
        public int age
        {
            get => int.Parse(HMap.hashMap[animal.ageHash]);
        }
        public ISpecies species
        {
            get => (ISpecies)HMap.adMap[animal.species];
        }
        public override string ToString() =>
            name + ", " + age.ToString() + (species == null ? "" : ", " + species.name);

    }
    public class SpeciesAdapterH : ISpecies
    {
        SpeciesH species;
        public Dictionary<string, Func<object>> getFields { get; set; }
        public Dictionary<string, Action<object>> setFields { get; set; }

        public SpeciesAdapterH(SpeciesH species)
        {
            this.species = species;
            getFields = new Dictionary<string, Func<object>>
            {
                ["name"] = () => name,
                ["favorite_foods"] = () => favoriteFoods
            };
        }

        public string name
        {
            get => HMap.hashMap[species.nameHash];
        }
        public List<ISpecies>? favoriteFoods
        {
            get
            {
                var list = new List<ISpecies>();
                if (species.favoriteFoods != null)
                {
                    foreach (var e in species.favoriteFoods)
                    {
                        list.Add((ISpecies)HMap.adMap[e]);
                    }
                }
                return list;
            }
        }
        public override string ToString() =>
            favoriteFoods == null ? name + ", []" : name + ", [" + string.Join(", ", favoriteFoods.Select(e => e.name)) + ']';
    }
    public static class Lab4
    {
        public static void lab4()
        {
            EnclosureH[] enclosures = {
                new EnclosureH("311", new List<AnimalH>(), null),
                new EnclosureH("Break", new List<AnimalH>(), null),
                new EnclosureH("Jurasic Park", new List<AnimalH>(), null)
            };

            VisitorH[] visitors = {
                new VisitorH("Tomas", "German", new List<EnclosureH>{ enclosures[0], enclosures[1] }),
                new VisitorH("Silvester", "Ileen", new List<EnclosureH>{ enclosures[2] }),
                new VisitorH("Basil", "Bailey", new List<EnclosureH> { enclosures[0], enclosures[1] }),
                new VisitorH("Ryker", "Polly", new List<EnclosureH>{ enclosures[1] })
            };

            EmployeeH[] employees =
                {
                new EmployeeH("Ricardo", "Stallmano", 73, new List<EnclosureH>{ enclosures[0] }),
                new EmployeeH("Steve", "Irvin", 43, new List<EnclosureH>{ enclosures[1], enclosures[2] })
            };

            SpeciesH[] species = {
                new SpeciesH("Meerkat", null),
                new SpeciesH("Kakapo", null),
                new SpeciesH("Bengal Tiger", null),
                new SpeciesH("Panda", null),
                new SpeciesH("Python", null),
                new SpeciesH("Dungeness Crab", null),
                new SpeciesH("Gopher", null),
                new SpeciesH("Cats", null),
                new SpeciesH("Penguin", null)
            };

            species[0].favoriteFoods = new List<string> { species[0].GetRef() };
            species[2].favoriteFoods = new List<string> { species[3].GetRef(), species[6].GetRef(), species[7].GetRef() };
            species[4].favoriteFoods = new List<string> { species[3].GetRef(), species[2].GetRef() };
            species[5].favoriteFoods = new List<string> { species[4].GetRef() };
            species[6].favoriteFoods = new List<string> { species[7].GetRef() };
            species[7].favoriteFoods = new List<string> { species[6].GetRef() };
            species[8].favoriteFoods = new List<string> { species[2].GetRef() };

            AnimalH[] animals = new AnimalH[]
                {
                new AnimalH("Harold", 2, species[0]),
                new AnimalH("Ryan", 1, species[0]),
                new AnimalH("Jenkins", 15, species[1]),
                new AnimalH("Kaka", 10, species[1]),
                new AnimalH("Ada", 13, species[2]),
                new AnimalH("Jett", 2, species[3]),
                new AnimalH("Conda", 4, species[4]),
                new AnimalH("Samuel", 2, species[4]),
                new AnimalH("Claire", 2, species[5]),
                new AnimalH("Andy", 3, species[6]),
                new AnimalH("Arrow", 5, species[7]),
                new AnimalH("Arch", 1, species[8]),
                new AnimalH("Ubuntu", 1, species[8]),
                new AnimalH("Fedora", 1, species[8])
                };

            enclosures[0].animals = new List<string> { animals[13].GetRef(), animals[11].GetRef(), animals[5].GetRef(), animals[8].GetRef() };
            enclosures[1].animals = new List<string> { animals[0].GetRef(), animals[1].GetRef(), animals[9].GetRef() };
            enclosures[2].animals = new List<string> { animals[2].GetRef(), animals[3].GetRef(), animals[4].GetRef(), animals[6].GetRef(), animals[7].GetRef(), animals[10].GetRef() };

            enclosures[0].employee = employees[0].GetRef();
            enclosures[1].employee = employees[1].GetRef();
            enclosures[2].employee = employees[1].GetRef();

            List<IEnclosure> enclosuresA = new();
            List<IEmployee> employeesA = new();
            List<IAnimal> animalsA = new();
            List<IVisitor> visitorsA = new();
            List<ISpecies> speciesA = new();

            foreach (var e in enclosures)
            {
                EnclosureAdapterH en = new(e);
                enclosuresA.Add(en);
                HMap.adMap.Add(e.GetRef(), en);
            }
            foreach (var e in employees)
            {
                EmployeeAdapterH en = new(e);
                employeesA.Add(en);
                HMap.adMap.Add(e.GetRef(), en);
            }

            foreach (var a in animals)
            {
                AnimalAdapterH an = new(a);
                animalsA.Add(an);
                HMap.adMap.Add(a.GetRef(), an);
            }

            foreach (var v in visitors)
            {
                VisitorAdapterH vi = new(v);
                visitorsA.Add(vi);
                HMap.adMap.Add(v.GetRef(), vi);
            }

            foreach (var s in species)
            {
                SpeciesAdapterH sp = new(s);
                speciesA.Add(sp);
                HMap.adMap.Add(s.GetRef(), sp);
            }

            ZOOAdapterH zoo = new ZOOAdapterH(visitorsA, enclosuresA, employeesA, animalsA, speciesA);

            Console.WriteLine("Enclosures:\n");
            foreach (var e in enclosuresA)
            {
                Console.WriteLine('\t' + e.ToString());
            }
            Console.WriteLine("\nEmployees:\n");
            foreach (var e in employeesA)
            {
                Console.WriteLine('\t' + e.ToString());
            }
            Console.WriteLine("\nAnimals:\n");
            foreach (var a in animalsA)
            {
                Console.WriteLine('\t' + a.ToString());
            }
            Console.WriteLine("\nVisitors:\n");
            foreach (var v in visitorsA)
            {
                Console.WriteLine('\t' + v.ToString());
            }
            Console.WriteLine("\nSpecies:\n");
            foreach (var s in speciesA)
            {
                Console.WriteLine('\t' + s.ToString());
            }



        }
    }
}
