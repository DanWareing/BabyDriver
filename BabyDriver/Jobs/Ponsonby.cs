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
    public class Ponsonby : Job
    {
        public Ponsonby()
        {
            base.type = "Ponsonby";
            base.cashierModel = PedHash.Hotposh01;
            base.crewSize = 1;
            base.length = 15000;
            base.size = Size.SMALL;
            base.timeRestriction = Time.DAY_ONLY;
            base.levelNumber = 4;
            base.locationSet = LocationSets.getIncompleteLocationSetByType(base.type);
            base.valueLower = 25000;
            base.valueUpper = 30000;
        }
    }
}
