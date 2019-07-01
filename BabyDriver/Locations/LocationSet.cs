using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;

namespace BabyDriver.Locations
{
    public class LocationSet
    {
        public string name;
        public bool isComplete = false;
        public List<Vector3> heistCashierLocations;
        public List<Vector3> heistCrewSpawnLocations;
        public Vector3 heistCrewSpawnRotation;
        public Vector3 heistDropoffLocation;
        public Vector3 heistPickupLocation;
        public List<Vector3> heistStartLocations;

        public LocationSet(
            string name,
            List<Vector3> heistCashierLocations,
            List<Vector3> heistCrewSpawnLocations,
            Vector3 heistCrewSpawnRotation,
            Vector3 heistDropoffLocation,
            Vector3 heistPickupLocation,
            List<Vector3> heistStartLocations
        ) {
            this.name = name;
            this.heistCashierLocations = heistCashierLocations;
            this.heistCrewSpawnLocations = heistCrewSpawnLocations;
            this.heistCrewSpawnRotation = heistCrewSpawnRotation;
            this.heistDropoffLocation = heistDropoffLocation;
            this.heistPickupLocation = heistPickupLocation;
            this.heistStartLocations = heistStartLocations;
        }
    }
}
