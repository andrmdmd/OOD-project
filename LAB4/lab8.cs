using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public static class lab8
    {
        public static void lab()
        {
            ZOOS zoo = new ZOOS();

            // Initializing Enclosure objects
            EnclosureS[] enclosures = new EnclosureS[]
            {
                new EnclosureS("311", new int[]{ 15, 10, 9 }, 27),
                new EnclosureS("Break", new int[]{ 13, 11, 12 }, 28),
                new EnclosureS("Jurasic Park", new int[]{ 6, 8, 12 }, 28)
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
                ZOO.objList["enclosure"].Add(en);
            }
            foreach (var e in employees)
            {
                EmployeeAdapterS en = new(e);
                employeesA.Add(en);
                ZOO.objList["employee"].Add(en);
            }

            foreach (var a in animals)
            {
                AnimalAdapterS an = new(a);
                animalsA.Add(an);
                ZOO.objList["animal"].Add(an);
            }

            foreach (var v in visitors)
            {
                VisitorAdapterS vi = new(v);
                visitorsA.Add(vi);
                ZOO.objList["visitor"].Add(vi);
            }

            foreach (var s in species)
            {
                SpeciesAdapterS sp = new(s);
                speciesA.Add(sp);
                ZOO.objList["species"].Add(sp);
            }

            ZOOAdapterS z = new ZOOAdapterS(visitorsA, enclosuresA, employeesA, animalsA, speciesA);

        }
    }

    
}
