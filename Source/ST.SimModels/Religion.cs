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

        public override string ToString()
        {
            return "The religion of " + Religion.Name + ", led by Prophet " + Prophet.Name;
        }
    }
}
