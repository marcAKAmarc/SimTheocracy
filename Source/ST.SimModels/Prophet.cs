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
        public int Following { get { return GetFollowing(); } set { return; } }
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

        public override string About()
        {
            var str = this.ToString();
            str += Environment.NewLine;
            str += "Following:  " + Following.ToString();
            str += Environment.NewLine;
            str += "Divinity:  " + Divination.ToString();
            str += Environment.NewLine;
            str += "Donations:  " + Donations.ToString();
            str += Environment.NewLine;
            if (RecievedTenants.Count > 0)
            {
                str += "The Prophet has " + RecievedTenants.Count + " tenants recieved from the divine that have yet to be transcribed:" + Environment.NewLine;
                foreach (var t in RecievedTenants)
                {
                    str += t.ToString();
                    str += Environment.NewLine;
                }
            }
            if(KnownProphets.Count > 0)
            {
                str += "The Prophet has encountered " + KnownProphets.Count + " other \"prophets\":" + Environment.NewLine;
                foreach(var p in KnownProphets)
                {
                    str += p.ToString();
                    str += Environment.NewLine;
                }
            }

            return str;
                
        }
        private int GetFollowing()
        {
            return Religion.Followers.Count;
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

                    return KnownReligions.First(x => x.Id == prophet.Religion.Id);
                }
            }

            return null;
        }

        public void WriteScripture(Random random, List<Tenant> tenants, List<Religion> RenouncedReligions, int DivinityAdd, string text)
        {
            State = ProphetState.Writing;
            Religion.Scriptures.Add(new Scripture() { Text = text, Tenants = tenants, Divinity = DivinityAdd, RenouncedReligions = RenouncedReligions });
            foreach(var t in tenants)
            {
                RecievedTenants = RecievedTenants.Where(x => x.Id != t.Id).ToList();
                Religion.Tenants.Add(t);
            }

            ActionResult = new ActionResult()
            {
                DivinationChange = -DivinityAdd
            };
            Divination -= DivinityAdd;
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
            if (Religion.Scriptures.Count == 0)
                return;

            State = ProphetState.Reciting;

            var probability = Math.Max(Religion.Scriptures.Count / 30, .25);

            int followersBefore = Following;

            //already of religion cache
            var already = CurrentLocation.People.Where(x => x.Religion.Id == Religion.Id);


            foreach (var person in CurrentLocation.People.Where(x=>x.Religion.Id != Religion.Id))
            {
                if(random.Next(100) < probability * 100)
                {
                    person.Convert(Religion);
                }
            }

            int followerChange = Following - followersBefore;

            //event report
            var sawEvent = new EventReport()
            {
                LearnableItems = new List<Base>()
                {
                    Religion
                },
                Text = "Today, I saw " + followerChange + " people get converted to the religion of " + Religion.Name + " in the plaza of " + CurrentLocation + "."
            };
            var wasEvent = new EventReport()
            {
                LearnableItems = new List<Base>()
                {
                    Religion
                },
                Text = "Along with " + followerChange + " people, I was converted to the religion of " + Religion.Name + " in the plaza of " + CurrentLocation + "!"
            };


            //saw conversions
            foreach (var person in CurrentLocation.People.Where(x=>x.Religion.Id!=Religion.Id))
            {
                person.RememberEvent(sawEvent);
            }
            foreach (var person in CurrentLocation.People.Where(x=>x.Religion.Id == Religion.Id  && !already.Any(p=>p.Id == x.Id))){
                person.RememberEvent(wasEvent);
            }

            ActionResult = new ActionResult()
            {
                FollowingChange = followerChange
            };
        }

        
    }

    
}
