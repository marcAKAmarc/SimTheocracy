using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ST.SimModels
{
    public class EventReport
    {
        public Guid Id { get; set; }
        public List<Base> LearnableItems;
        public string Text;

        public EventReport()
        {
            Id = Guid.NewGuid();
            LearnableItems = new List<Base>();
        }
        private sealed class EventReportEqualityComparer : IEqualityComparer<EventReport>
        {
            public bool Equals(EventReport x, EventReport y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(EventReport obj)
            {
                return (obj.Text != null ? obj.Text.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<EventReport> Comparer { get; } = new EventReportEqualityComparer();
    }
    public class Person : Base
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Province Residence { get; set; }
        public Religion Religion { get; set; }
        public List<PercievedReligion> KnownReligions;

        public List<EventReport> EventReports;

        public Person()
        {
            KnownReligions = new List<PercievedReligion>();
            EventReports = new List<EventReport>();
        }

        public Person(Random random)
        {
            this.NewName(random);
            Id = Guid.NewGuid();
            KnownReligions = new List<PercievedReligion>();
            EventReports = new List<EventReport>();
        }

        public void Convert(Religion religion)
        {
            if (Religion != null)
            {
                Religion.Followers = Religion.Followers.Where(f => f.Id != Id).ToList();
            }

            religion.Followers.Add(this);

            Religion = religion;
        }

        public override string ToString()
        {
            return "Commoner " + Name;
        }

        public void RememberEvent(EventReport report)
        {
            EventReports.Add(report);
        }
    }

    public static class PersonExtensions
    {
        public static string NewName(this Person person, Random random)
        {
            string result = "";
            int i = 0;
            int goal = random.Next(1, 2);
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
            goal = random.Next(2, 3);
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
        public static readonly string[] Syllables = { "mar","cus","jes","sica","sid","ney","den","ke","ben","ja","min","mu","ham","med","al","urn","lan", "der", "derd", "lang", "ler", "lerd", "derl", "lal", "end", "doo", "de", "sterp", "el", "en", "deo", "scho", "gench", "kri", "scho" };
    }
}
