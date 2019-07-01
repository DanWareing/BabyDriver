using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
using BabyDriver.Locations;

namespace BabyDriver.Jobs
{

    public abstract class Job
    {
        public string type;
        public string name;
        public int crewSize;
        public int levelNumber;
        public int length;
        public int valueLower;
        public int valueUpper;
        public Size size;
        public Time timeRestriction;
        public PedHash cashierModel;
        public LocationSet locationSet;

        public Ped GetTargetPed()
        {
            Vector3 cashierLocation = locationSet.heistCashierLocations[0];
            cashierLocation.Z = World.GetGroundHeight(new Vector2(cashierLocation.X, cashierLocation.Y));
            Ped[] peds = World.GetNearbyPeds(cashierLocation, 20f);
            foreach (Ped ped in peds)
            {
                if (ped.Model == cashierModel)
                {
                    locationSet.heistCashierLocations[0] = ped.Position;
                    return ped;
                }
            }
            Ped shopKeep = World.CreatePed(cashierModel, cashierLocation);
            shopKeep.Money = 69;
            return shopKeep;
        }
    }
}
