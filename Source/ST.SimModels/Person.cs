using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ST.SimModels
{
    public class Person : Base
    {
        public Guid Id {get; set;}
        public string Name { get; set; }
        public Province Residence { get; set; }
        public Religion Religion { get; set; }
        public List<PercievedReligion> KnownReligions;

        public Person()
        {
            KnownReligions = new List<PercievedReligion>();
        }

        public Person(Random random)
        {
            this.NewName(random);
            Id = Guid.NewGuid();
            KnownReligions = new List<PercievedReligion>();
        }

        public override string ToString()
        {
            return "Commoner " + Name;
        }
    }

    public static class PersonExtensions
    {
        public static string NewName(this Person person, Random random)
        {
            string result = "";
            int i = 0;
            int goal = random.Next(1, 3);
            while (i <= goal)
            {
                result += NameData.Syllables[random.Next(NameData.Syllables.Length)];
                i++;
            }

            result += " ";

            i = 0;
            goal = random.Next(1, 2);
            while (i <= goal)
            {
                result += NameData.Syllables[random.Next(NameData.Syllables.Length)];
                i++;
            }

            result += " ";

            i = 0;
            goal = random.Next(2, 4);
            while (i <= goal)
            {
                result += NameData.Syllables[random.Next(NameData.Syllables.Length)];
                i++;
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            result = textInfo.ToTitleCase(result);
            person.Name = result;
            return result;
        }
    }

    public static class NameData
    {
        public static readonly string[] Syllables = { "lan", "der", "derd", "lang", "ler", "lerd", "derl", "lal", "end", "doo", "de", "sterp", "el", "en", "deo", "scho", "gench", "kri", "scho" };
    }
}
