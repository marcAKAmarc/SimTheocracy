using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.SimModels
{
    public class Province : Base
    {
        public static List<Province> Provinces;
        public Guid Id;
        public List<Guid> PeopleIds;
        public virtual List<Person> People { get; set; }
        public string Name;

        public Province()
        {
            if (Provinces == null)
                Provinces = new List<Province>();
            Provinces.Add(this);
            People = new List<Person>();
        }

        public override string ToString()
        {
            return "Province of " + Name;
        }

        public float GetReligionDemographic(Religion religion)
        {
            return People.Count(x => x.Religion.Id == religion.Id) / People.Count();
        }

        public float GetSharedTenantDemographic(List<Tenant> tenants)
        {
            return People.SelectMany(x => x.Religion.Tenants).Count(localTen => !tenants.Any(myTen => !myTen.AmenableToTenant(localTen)))
                / People.SelectMany(x => x.Religion.Tenants).Count();
        }
    }
}
