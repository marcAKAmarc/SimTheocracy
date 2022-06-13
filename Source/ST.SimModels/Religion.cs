using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.SimModels
{
    public class Religion : Base
    {
        public Guid Id;
        public string Name { get; set; }
        public List<Scripture> Scriptures { get; set; }
        public virtual List<Tenant> Tenants { get; set; }

        public Religion()
        {
            Id = Guid.NewGuid();
            Scriptures = new List<Scripture>();
            Tenants = new List<Tenant>();
        }
    }

    public class PercievedReligion:Base
    {
        public Guid Id { get; set; }
        public Guid ReligionId { get; set; }
        public virtual Religion Religion { get; set; }
        public virtual Prophet Prophet { get; set; }
        public Guid ProphetId { get; set; }
        public List<Guid> PercievedTenantIds { get; set; }
        public virtual List<Tenant> PercievedTenants { get; set; }
        public List<Guid> SupportsIds { get; set; }
        public virtual List<PercievedReligion> Supports { get; set; }
        public List<Guid> SupportedByIds { get; set; }
        public List<PercievedReligion> SupportedBy { get; set; }

        public PercievedReligion()
        {
            PercievedTenantIds = new List<Guid>();
            PercievedTenants = new List<Tenant>();
            SupportsIds = new List<Guid>();
            Supports = new List<PercievedReligion>();
            SupportedByIds = new List<Guid>();
            SupportedBy = new List<PercievedReligion>();
        }

        public override string ToString()
        {
            string str = "The religion of " + Religion.Name;
            if (Prophet != null) {
                str += Environment.NewLine + "  -Led by Prophet " + Prophet.Name;
            }
            if (PercievedTenants.Any())
            {
                foreach(var tenant in PercievedTenants)
                {
                    str += Environment.NewLine + "  -" + tenant.Level.ToString() + " Ideal of " + tenant.Ideal.Name + ".  \"" + tenant.Ideal.Description + "\"";
                }                
            }
            return str;
        }
    }
}
