using System;
using System.Collections.Generic;
using System.Linq;
using GTA.Math;
using System.Reflection;

namespace BabyDriver.Locations
{
    public static class LocationSets
    {

        public static List<LocationSet> AllLocationSets = new List<LocationSet>();

        public static void setupLocationSets()
        {
            Type locationSetsType = (typeof(LocationSets));
            var jobFields = locationSetsType.GetFields(BindingFlags.Static|BindingFlags.Public).Where(x => x.Name.StartsWith("Job_"));
            foreach (FieldInfo job in jobFields)
            {
                AllLocationSets.Add((LocationSet)job.GetValue(null));
            }
        }

        public static LocationSet getLocationSetByName(string name)
        {
            return AllLocationSets.Where(x => x.name == name).First();
        }

        public static LocationSet getLocationSetByType(string type)
        {
            return AllLocationSets.Where(x => x.name.StartsWith(type)).First();
        }

        public static LocationSet getIncompleteLocationSetByType(string type)
        {
            return AllLocationSets.Where(x => x.name.StartsWith(type) && !x.isComplete).First();
        }

        /**
         * LIQUOR STORES
        **/

        public static LocationSet Job_LiquorStore = new LocationSet(
            "Liquor Store - First",
            new List<Vector3> { new Vector3(-1487.548f, -378.7894f, 1000f) },   // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(-1504.258f, -386.3975f, 1000f),                         // dropoff
            new Vector3(-2030.986f, -262.1189f, 1000f),                         // pickup
            new List<Vector3> { new Vector3(-1487.548f, -378.7894f, 1000f) }    // heist start
        );


        /**
         * GAS STATIONS
        **/

        public static LocationSet Job_GasStation_RichmanGlen = new LocationSet(
            "Gas Station - Richman Glen",
            new List<Vector3> { new Vector3(-1820.605f, 794.8023f, 1000f) },    // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(-1821.534f, 783.0057f, 1000f),                          // dropoff
            new Vector3(-2030.986f, -262.1189f, 1000f),                         // pickup
            new List<Vector3> { new Vector3(-1821.474f, 793.4889f, 1000f) }     // heist start
        );

        public static LocationSet Job_GasStation_LittleSeoul = new LocationSet(
            "Gas Station - Little Seoul",
            new List<Vector3> { new Vector3(-706.0657f, -913.0418f, 1000f) },   // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(-714.7231f, -920.9158f, 1000f),                         // dropoff
            new Vector3(-2030.986f, -262.1189f, 1000f),                         // pickup
            new List<Vector3> { new Vector3(-707.4089f, -913.4289f, 1000f) }    // heist start
        );

        public static LocationSet Job_GasStation_Davis = new LocationSet(
            "Gas Station - Davis",
            new List<Vector3> { new Vector3(-46.09691f, -1757.244f, 1000f) },   // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(-54.18535f, -1764.838f, 1000f),                         // dropoff
            new Vector3(126.5448f, -2199.37f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(-47.44284f, -1756.762f, 1000f) }    // heist start
        );

        public static LocationSet Job_GasStation_MirrorPark = new LocationSet(
            "Gas Station - Mirror Park",
            new List<Vector3> { new Vector3(1164.666f, -322.2213f, 1000f) },    // cashier
            new List<Vector3> { new Vector3(579.2148f, 135.5722f, 1000f) },     // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(1157.582f, -331.2304f, 1000f),                          // dropoff
            new Vector3(578.0603f, 131.3417f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(1163.23f, -322.8311f, 1000f) }      // heist start
        );

        /**
         * 24/7s
        **/

        public static LocationSet Job_247_BanhamCanyon = new LocationSet(
            "24/7 - Banham Canyon",
            new List<Vector3> { new Vector3(-3040.929f, 583.676f, 1000f) },     // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(-3031.847f, 591.486f, 1000f),                           // dropoff
            new Vector3(-2030.986f, -262.1189f, 1000f),                         // pickup
            new List<Vector3> { new Vector3(-3040.156f, 586.9678f, 1000f) }     // heist start
        );

        public static LocationSet Job_247_Chumash = new LocationSet(
            "24/7 - Chumash",
            new List<Vector3> { new Vector3(-3243.187f, 1001.677f, 1000f) },    // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(-3234.959f, 1004.041f, 1000f),                          // dropoff
            new Vector3(-2030.986f, -262.1189f, 1000f),                         // pickup
            new List<Vector3> { new Vector3(-3243.187f, 1001.677f, 1000f) }     // heist start
        );

        public static LocationSet Job_247_Central = new LocationSet(
            "24/7 - Central",
            new List<Vector3> { new Vector3(24.10367f, -1345.734f, 1000f) },    // cashier
            new List<Vector3> { new Vector3(132.1101f, -2204.764f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(26.63486f, -1356.371f, 1000f),                          // dropoff
            new Vector3(126.5448f, -2199.37f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(26.09224f, -1346.5f, 1000f) }       // heist start
        );

        public static LocationSet Job_247_TataviumMountains = new LocationSet(
            "24/7 - Tatavium Mountains",
            new List<Vector3> { new Vector3(2555.931f, 382.1496f, 1000f) },     // cashier
            new List<Vector3> { new Vector3(579.2148f, 135.5722f, 1000f) },     // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(2564.386f, 385.0746f, 1000f),                           // dropoff
            new Vector3(578.0603f, 131.3417f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(2554.982f, 380.8832f, 1000f) }      // heist start
        );

        public static LocationSet Job_247_GrandSenora = new LocationSet(
            "24/7 - Grand Senora",
            new List<Vector3> { new Vector3(2677.663f, 3281.233f, 1000f) },     // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(2686.282f, 3280.033f, 1000f),                           // dropoff
            new Vector3(578.0603f, 131.3417f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(2676.271f, 3280.325f, 1000f) }      // heist start
        );

        public static LocationSet Job_247_Harmony = new LocationSet(
            "24/7 - Harmony",
            new List<Vector3> { new Vector3(547.9881f, 2669.741f, 1000f) },     // cashier
            new List<Vector3> { new Vector3(579.2148f, 135.5722f, 1000f) },     // crew spawn
            new Vector3(0, 0, 0),
             new Vector3(543.0946f, 2678.143f, 1000f),                          // dropoff
            new Vector3(578.0603f, 131.3417f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(549.3752f, 2669.43f, 1000f) }       // heist start
        );

        public static LocationSet Job_247_DowntownVinewood = new LocationSet(
            "24/7 - Downtown Vinewood",
            new List<Vector3> { new Vector3(374.114f, 327.3558f, 1000f) },      // cashier
            new List<Vector3> { new Vector3(579.2148f, 135.5722f, 1000f) },     // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(373.3623f, 317.4489f, 1000f),                           // dropoff
            new Vector3(578.0603f, 131.3417f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(373.0158f, 328.4777f, 1000f) }      // heist start
        );

        /**
         * PONSONBYS
        **/

        public static LocationSet Job_Ponsonby_ = new LocationSet(
            "Ponsonby",
            new List<Vector3> { new Vector3(-709.8244f, -153.2769f, 1000f) },   // cashier
            new List<Vector3> { new Vector3(-2024.668f, -254.908f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(-728.5435f, -149.0536f, 1000f),                         // dropoff
            new Vector3(-2030.986f, -262.1189f, 1000f),                         // pickup
            new List<Vector3> { new Vector3(-708.1223f, -153.0347f, 1000f) }    // heist start
        );

        /**
         * FLEECAS
        **/

        public static LocationSet Job_Fleeca_Burton = new LocationSet(
            "Fleeca - Burton",
            new List<Vector3> { new Vector3(149.8117f, -1040.702f, 1000f) },    // cashier
            new List<Vector3> { new Vector3(132.1101f, -2204.764f, 1000f) },    // crew spawn
            new Vector3(0, 0, 0),
            new Vector3(153.9129f, -1030.703f, 1000f),                          // dropoff
            new Vector3(126.5448f, -2199.37f, 1000f),                           // pickup
            new List<Vector3> { new Vector3(149.8117f, -1040.702f, 1000f) }     // heist start
        );

    }
}
