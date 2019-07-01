using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyDriver.Jobs
{
    public enum Stage { NONE, PICKUP, PICKUP_WAIT, JOURNEY, HEIST_ARRIVAL, HEIST_START, HEIST_WAIT, ESCAPE, END, FAIL }
    public enum Size { SMALL, MEDIUM, LARGE };
    public enum Time { ANY, DAY_ONLY, NIGHT_ONLY };
    public enum Type { }

    public static class Jobs
    {

    }
}
