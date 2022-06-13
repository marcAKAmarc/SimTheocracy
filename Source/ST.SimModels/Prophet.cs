using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.SimModels
{
    

    public class ActionResult
    {
        public int DivinationChange;
        public int FollowingChange;
        public int DonationsChange;

        public override string ToString()
        {
            var result = "";
            if(DivinationChange > 0)
            {
                result += "Divination +" + DivinationChange;
            }else if(DivinationChange < 0)
            {
                result += "Divination " + DivinationChange;
            }

            if (FollowingChange > 0)
            {
                result += "Following +" + FollowingChange;
            }
            else if (FollowingChange < 0)
            {
                result += "Following " + FollowingChange;
            }

            if (DonationsChange > 0)
            {
                result += "Donations +" + DonationsChange;
            }
            else if (DonationsChange < 0)
            {
                result += "Donation " + DonationsChange;
            }

            return result;
        }
    }

    public enum ProphetState { Observing, Reciting, Meditating, Writing, VisitingTemple, AtResidence }

    public class Prophet : Person
    {
        public int Following { get; set; }
        public int Divination { get; set; }
        public int Donations { get; set; }

        public List<Tenant> RecievedTenants;
        public List<Province> KnownProvinces;
        public List<Prophet> KnownProphets;

        public ProphetState State;

        public Province CurrentLocation;

        public ActionResult ActionResult;

        public Prophet(Random random) : base(random)
        {
            RecievedTenants = new List<Tenant>();
            KnownProphets = new List<Prophet>();
            KnownProvinces = new List<Province>();
            GoHome();
        }

        public override string ToString()
        {
            return "Prophet " + Name + ", leader of the religion of " + Religion.Name;
        }

        public void GoHome()
        {
            State = ProphetState.AtResidence;
            CurrentLocation = Residence;
            ActionResult = new ActionResult();
        }

        public Tenant GoToHolyMountain(Random random)
        {
            CurrentLocation = null;
            State = ProphetState.Meditating;

            Tenant returnTenant = null;
            int divchange = 0;
            if (random.Next(2) >= 1)
            {
                divchange = 1 + random.Next(2);
                if(random.Next(2) >= 1)
                {
                    RecievedTenants.Add(new Tenant());
                    RecievedTenants.Last().Randomize(random);
                    returnTenant =RecievedTenants.Last();
                }
            }
            Divination += divchange;

            ActionResult = new ActionResult()
            {
                DivinationChange = divchange
            };

            return returnTenant;
            
        }

        public void GoToPlaza(Province province)
        {
            CurrentLocation = province;
        }

        public Base ListenToProphet(Prophet prophet, Random random)
        {
            prophet.State = ProphetState.Observing;
            //add religion if unknown
            if(!KnownReligions.Any(x=>x.Religion.Id == prophet.Religion.Id))
            {
                KnownReligions.Add(
                    new PercievedReligion()
                    {
                        Religion = prophet.Religion,
                        Prophet = prophet
                    }    
                );

                return KnownReligions.Last();
            }
            else
            {
                //else add tenants if unknown
                if(prophet.Religion.Tenants.Any(pt=> !KnownReligions.First(x=>x.Id == prophet.Religion.Id).PercievedTenants.Any(t=>t.Id == pt.Id))){

                    KnownReligions.First(x => x.Id == prophet.Religion.Id).PercievedTenants.Add(
                        prophet.Religion.Tenants.First(pt => !KnownReligions.First(x => x.Id == prophet.Religion.Id).PercievedTenants.Any(t => t.Id == pt.Id))
                    );

                    return KnownReligions.First(x => x.Id == prophet.Religion.Id).PercievedTenants.Last();
                }
            }

            return null;
        }

        public bool CheckAttacked(Random random)
        {
            if (CurrentLocation.GetReligionDemographic(Religion) > .5)
                return false;
            if (CurrentLocation.GetSharedTenantDemographic(Religion.Tenants) > .25)
                return false;

            if(random.Next(2) < 1)
            {
                return true;
            }

            return false;
        }

        public void Proselytize(Random random)
        {
            State = ProphetState.Reciting;
        }

        
    }

    
}
