using ST.SimModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.GameModel
{
    public class Game
    {
        public int Seed { get; set; }
        public Random Random { get; set; }
        public List<Province> Provinces;
        public List<Religion> religions;
        public List<Prophet> Prophets;

        public Prophet HumanPlayer;
    
        public void Setup()
        {
            Random = new Random(1);

            Provinces = new List<Province>();
            religions = new List<Religion>();
            Prophets = new List<Prophet>() { };


            for (var i = 0; i < 4; i++)
            {
                religions.Add(new Religion());
                switch (i)
                {
                    case 0:
                        religions.Last().Name = "Laisia";
                        religions.Last().Tenants = new List<Tenant>() {
                            new Tenant(){Ideal = Ideals.Allah, Level = TenantLevel.Observed}
                        };
                        break;
                    case 1:
                        religions.Last().Name = "Olum";
                        religions.Last().Tenants = new List<Tenant>() {
                            new Tenant(){Ideal = Ideals.YHWH, Level = TenantLevel.Enforced}
                        };
                        break;
                    case 2:
                        religions.Last().Name = "Szeruk";
                        religions.Last().Tenants = new List<Tenant>() {
                            new Tenant(){Ideal = Ideals.Allah, Level = TenantLevel.Required}
                        };
                        break;
                    case 3:
                        religions.Last().Name = "Hand of Weia";
                        religions.Last().Tenants = new List<Tenant>() {
                            new Tenant(){Ideal = Ideals.Allah, Level = TenantLevel.Observed}
                        };
                        break;
                }
            }




            for (var i = 0; i < 6; i++)
            {
                Provinces.Add(new Province());
                switch (i)
                {
                    case 0:
                        Provinces.Last().Name = "Nomee";
                        break;
                    case 1:
                        Provinces.Last().Name = "Jahilia";
                        break;
                    case 2:
                        Provinces.Last().Name = "Yathrib";
                        break;
                    case 3:
                        Provinces.Last().Name = "Narria";
                        break;
                    case 4:
                        Provinces.Last().Name = "Larro";
                        break;
                }
            }

            List<Person> people = new List<Person>();
            for (var i = 0; i < 100; i++)
            {
                people.Add(new Person(Random) { Religion = religions[Random.Next(religions.Count)], Residence = Provinces[Random.Next(Provinces.Count)] });

                people.Last().Residence.People.Add(people.Last());
            }

            for (var i = 0; i < religions.Count; i++)
            {
                Prophets.Add(new Prophet(Random) { Religion = religions[i], Residence = Provinces[Random.Next(Provinces.Count)] });

                Prophets.Last().Residence.People.Add(Prophets.Last());

                Prophets.Last().KnownProvinces = Provinces;

                people.Add(Prophets.Last());
            }

            HumanPlayer = Prophets.Last();
        }
    }

}
