using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyDriver.Jobs
{
    public static class JobFactory
    {
        public static Job CreateNewJob(int currentLevel)
        {
            switch (currentLevel)
            {
                case 0:
                    List<int> intList = new List<int> { 1, 2, 3, 4 };
                    Random rand = new Random();
                    int randomInt = rand.Next(intList.Count());
                    return CreateNewJob(randomInt);
                case 1:
                    return new LiquorStore();
                case 2:
                    return new GasStation();
                case 3:
                    return new _247();
                /*case 4:
                    return new Ponsonby();*/
                default:
                    return new LiquorStore();
            }
        }
    }
}
