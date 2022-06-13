using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.SimModels
{
    public enum TenantLevel { Enforced, Required, Observed, Denied, Banned, RewardBanned }
    public enum IdealType
    {
        Diety,
        Tradition
    }
    public struct Ideal
    {
        public string Name;
        public string Description;
        public IdealType IdealType;
    }
    public class Tenant : Base {
        public Guid Id { get; set; }
        public Ideal Ideal { get; set; }
        public TenantLevel Level { get; set; } 

        public override string ToString()
        {
            return Ideal.Name + "; " + "All followers must: " + Ideal.Description + ".  This is " + Level.ToString();
        }
    }

    public static class TenantExtensions
    {
        public static bool AmenableToTenant(this Tenant tenant, Tenant otherTenant)
        {
            if(tenant.Ideal.Name == otherTenant.Ideal.Name)
            {
                if((int)tenant.Level < 3 && (int) otherTenant.Level >= 3)
                {
                    return false;
                }
                if((int)tenant.Level >= 3 && (int) otherTenant.Level < 3)
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public static void Randomize(this Tenant tenant, Random random)
        {
            tenant.Ideal = Ideals.Library[random.Next(Ideals.Library.Count)];
            tenant.Level = (TenantLevel)Enum.GetValues(typeof(TenantLevel)).GetValue(random.Next(Enum.GetValues(typeof(TenantLevel)).Length));       
        }
    }

    public static class Ideals
    {
        public static readonly Ideal YHWH = new Ideal() { Name = "YHWH", Description = "Follow the diety Yaweh.", IdealType = IdealType.Diety };
        public static readonly Ideal Allah = new Ideal() { Name = "Allah", Description = "Follow the diety Allah. ", IdealType = IdealType.Diety };
        public static readonly Ideal AlLat = new Ideal() { Name = "Al-Lat", Description = "Follow the diety Al-Lat.", IdealType = IdealType.Diety };
        public static readonly Ideal Insular = new Ideal() { Name = "Insularity", Description = "Socialize only with those of this faith.", IdealType = IdealType.Tradition };
        public static readonly Ideal WeeklyFast = new Ideal() { Name = "Weekly Fast", Description = "Fast once a week from sunrise to sunset.", IdealType = IdealType.Tradition };
        public static readonly Ideal StrictSunPrayer = new Ideal() { Name = "Strict Sun Prayer", Description = "Pray facing the sun at sunrise and sunset.", IdealType = IdealType.Tradition };
        public static readonly Ideal Intolerance = new Ideal() { Name = "Intolerance", Description = "Disband all power and property from infidels.", IdealType = IdealType.Tradition };
        public static readonly Ideal EmpoweredPolygamy = new Ideal() { Name = "Empowered Polygamy", Description = "Abstain from polygamy, excepting the Prophet." };
        public static readonly Ideal EmpoweredPatriachalArrangedMarriage = new Ideal() { Name = "Patriarchal Arranged Marriages", Description = "Arrange Marriages as seen fit by Patriarchal Leaders." };
        public static readonly Ideal NoPork = new Ideal() { Name = "No Pork", Description = "Abstain from eating pork.", IdealType = IdealType.Tradition };
        public static readonly Ideal NoAlcohol = new Ideal() { Name = "No Alcohol", Description = "Abstain from consuming alcohol.", IdealType = IdealType.Tradition };

        public static readonly List<Ideal> Library = new List<Ideal>() { YHWH, Allah, AlLat, Insular, WeeklyFast, StrictSunPrayer, Intolerance, EmpoweredPolygamy, EmpoweredPatriachalArrangedMarriage, NoAlcohol, NoPork };
    }
    
}
