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
    public class LiquorStore : Job
    {
        public LiquorStore()
        {
            base.type = "Liquor Store";
            base.cashierModel = PedHash.ShopKeep01;
            base.crewSize = 1;
            base.length = 15000;
            base.size = Size.SMALL;
            base.timeRestriction = Time.ANY;
            base.levelNumber = 1;
            base.locationSet = LocationSets.getIncompleteLocationSetByType(base.type);
            base.valueLower = 500;
            base.valueUpper = 1000;
        }
    }
}
