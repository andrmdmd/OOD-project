using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace LAB
{
    public class ZOOS
    {
        public static int objectId = 1;
        public static Dictionary<int, object> idMap = new();

        public List<VisitorS> visitors = new();
        public List<EnclosureS> enclosures = new();
        public List<EmployeeS> employees = new();
        public List<AnimalS> animals = new();
        public List<SpeciesS> species = new();




    }
    public class VisitorS
    {
        public int visitorId;

        public Tuple<int, Stack<string>> visitorTuple;
        public VisitorS(string name = "", string surname = "", int[] enclosuresId = null)
        {
            visitorId = ZOOS.objectId++;
            Stack<string> s = new Stack<string>();

            s.Push(name);
            s.Push("1");
            s.Push("name");

            s.Push(surname);
            s.Push("1");
            s.Push("surname");
            if (enclosuresId == null) enclosuresId = new int[] { };
            Array.ForEach(enclosuresId, e => s.Push(e.ToString()));
            s.Push(enclosuresId.Length.ToString());
            s.Push("enclosures");

            visitorTuple = new(visitorId, s);
            ZOOS.idMap.Add(visitorId, this);
        }
    }
    public class EnclosureS
    {

        public int enclosureId;
        public Tuple<int, Stack<string>> enclosureTuple;
        public EnclosureS(string name = "" , int[] animalsId = null, int employeeId = 0)
        {
            enclosureId = ZOOS.objectId++;
            Stack<string> s = new Stack<string>();

            s.Push(name);
            s.Push("1");
            s.Push("name");

            if (animalsId == null) animalsId = new int[] { };
            Array.ForEach(animalsId, e => s.Push(e.ToString()));
            s.Push(animalsId.Length.ToString());
            s.Push("animals");

            s.Push(employeeId.ToString());
            s.Push("1");
            s.Push("employee");

            enclosureTuple = new(enclosureId, s);
            ZOOS.idMap.Add(enclosureId, this);
        }
    }
    public class EmployeeS
    {
        public int employeeId;
        public Tuple<int, Stack<string>> employeeTuple;

        public EmployeeS(string name = "", string surname = "", int age = 0, int[] enclosuresId = null)
        {
            employeeId = ZOOS.objectId++;
            Stack<string> s = new Stack<string>();

            s.Push(name);
            s.Push("1");
            s.Push("name");

            s.Push(surname);
            s.Push("1");
            s.Push("surname");

            s.Push(age.ToString());
            s.Push("1");
            s.Push("age");

            if (enclosuresId == null) enclosuresId = new int[] { };
            Array.ForEach(enclosuresId, e => s.Push(e.ToString()));
            s.Push(enclosuresId.Length.ToString());
            s.Push("enclosures");

            employeeTuple = new(employeeId, s);
            ZOOS.idMap.Add(employeeId, this);
        }
    }
    public class AnimalS
    {
        public int animalId;
        public Tuple<int, Stack<string>> animalTuple;
        public AnimalS(string name="", int age=0, int speciesId=0)
        {
            animalId = ZOOS.objectId++;
            Stack<string> s = new Stack<string>();

            s.Push(name);
            s.Push("1");
            s.Push("name");


            s.Push(age.ToString());
            s.Push("1");
            s.Push("age");

            s.Push(speciesId.ToString());
            s.Push("1");
            s.Push("species");

            animalTuple = new(animalId, s);
            ZOOS.idMap.Add(animalId, this);

        }
    }
    public class SpeciesS
    {
        public int speciesId;

        public Tuple<int, Stack<string>> speciesTuple;

        public SpeciesS(string name = "", int[] foodsId = null)
        {
            speciesId = ZOOS.objectId++;
            Stack<string> s = new Stack<string>();

            s.Push(name);
            s.Push("1");
            s.Push("name");

            if (foodsId == null) foodsId = new int[] { };
            Array.ForEach(foodsId, e => s.Push(e.ToString()));
            s.Push(foodsId.Length.ToString());
            s.Push("favoriteFoods");

            speciesTuple = new(speciesId, s);
            ZOOS.idMap.Add(speciesId, this);

        }
    }

}
