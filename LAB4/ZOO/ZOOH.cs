using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class ZOOH
    {
        public List<VisitorH> visitors = new();
        public List<EnclosureH> enclosures = new();
        public List<EmployeeH> employees = new();
        public List<AnimalH> animals = new();
        public List<SpeciesH> species = new();
    }

    public class VisitorH
    {
        public int nameHash;
        public int surnameHash;
        public List<string> visitedEnclosures = new();

        public VisitorH(string name = "", string surname = "", List<EnclosureH> visitedEnclosures = null)
        {
            nameHash = name.GetHashCode();
            surnameHash = surname.GetHashCode();
            HMap.hashMap.Add(nameHash, name);
            HMap.hashMap.Add(surnameHash, surname);
            if(visitedEnclosures != null)
                foreach (var en in visitedEnclosures)
                    this.visitedEnclosures.Add(en.GetRef());
            
        }
        public string GetRef()
        {
            return HMap.hashMap[nameHash] + ' ' + HMap.hashMap[surnameHash];
        }
    }
    public class EnclosureH
    {
        public int nameHash;
        public List<string> animals = new();
        public string employee;

        Hashtable hashmap = new Hashtable();

        public EnclosureH(string name = "", List<AnimalH> animals = null, EmployeeH employee = null)
        {
            nameHash = name.GetHashCode();
            HMap.hashMap.Add(nameHash, name);
            if (animals == null) animals = new List<AnimalH>() { };
            foreach (var an in animals)
            {
                this.animals.Add(an.GetRef());
            }
            if (employee != null)
            {
                this.employee = employee.GetRef();
            }
        }
        public string GetRef()
        {
            return HMap.hashMap[nameHash];
        }
    }
    public class EmployeeH
    {
        public int nameHash;
        public int surnameHash;
        public int ageHash;
        public List<string> enclosures = new();

        public EmployeeH(string name = "", string surname = "", int age = 0, List<EnclosureH> enclosures = null)
        {
            nameHash = name.GetHashCode();
            surnameHash = surname.GetHashCode();
            ageHash = age.ToString().GetHashCode();
            HMap.hashMap.Add(nameHash, name);
            HMap.hashMap.Add(surnameHash, surname);
            HMap.hashMap.Add(ageHash, age.ToString());
            if(enclosures == null) enclosures = new();
            foreach (var en in enclosures)
            {
                this.enclosures.Add(en.GetRef());
            }
        }

        public string GetRef()
        {
            return HMap.hashMap[nameHash] + ' ' + HMap.hashMap[surnameHash];
        }
    }
    public class AnimalH
    {
        public int nameHash;
        public int ageHash;
        public string species;

        public AnimalH(string name = "", int age = 0, SpeciesH species = null)
        {
            nameHash = name.GetHashCode();
            ageHash = age.ToString().GetHashCode();
            HMap.hashMap.Add(nameHash, name);
            if (HMap.hashMap.ContainsKey(ageHash) == false) HMap.hashMap.Add(ageHash, age.ToString());
            if(species != null) this.species = species.GetRef();
        }
        public string GetRef()
        {
            return HMap.hashMap[nameHash];
        }
    }
    public class SpeciesH
    {
        public int nameHash;
        public List<string>? favoriteFoods = new();

        public SpeciesH(string name = "", List<SpeciesH>? favoriteFoods = null)
        {
            nameHash = name.GetHashCode();
            HMap.hashMap.Add(nameHash, name);
            if (favoriteFoods != null)
            {
                foreach (var sp in favoriteFoods)
                {
                    this.favoriteFoods.Add(sp.GetRef());
                }
            }
        }
        public string GetRef()
        {
            return HMap.hashMap[nameHash];
        }
    }
}
