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
    public class _247 : Job
    {
        public _247()
        {
            base.type = "24/7";
            base.cashierModel = PedHash.ShopKeep01;
            base.crewSize = 1;
            base.length = 15000;
            base.size = Size.SMALL;
            base.timeRestriction = Time.ANY;
            base.levelNumber = 3;
            base.locationSet = LocationSets.getIncompleteLocationSetByType(base.type);
            base.valueLower = 500;
            base.valueUpper = 1000;
        }
    }
}
