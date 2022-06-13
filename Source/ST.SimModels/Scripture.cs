using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.SimModels
{
    public class Scripture : Base
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<Tenant> Tenants { get; set; }
        public List<Religion> RenouncedReligions { get; set; }
        public int Divinity {get; set;}

        public Scripture()
        {
            Id = new Guid();
        }
    }
}
