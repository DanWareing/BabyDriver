using GTA;
using GTA.Math;
using GTA.Native;

namespace BabyDriver.Cops
{
    public static class CopGenerator
    {
        // Static Methods
        public static Vehicle SpawnCar(int passengers, Vector3 location, float heading)
        {
            location.Z = World.GetGroundHeight(location);
            Vehicle cops = World.CreateVehicle(VehicleHash.Police, location, heading);
            //cops.PlaceOnNextStreet();
            Ped cop1 = cops.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Cop01SFY);
            cop1.Weapons.Give(WeaponHash.Pistol, 20, false, true);
            Function.Call(Hash.SET_PED_AS_COP, cop1, true);
            Ped cop2 = cops.CreatePedOnSeat(VehicleSeat.Passenger, PedHash.Cop01SMY);
            cop2.Weapons.Give(WeaponHash.Pistol, 20, false, true);
            Function.Call(Hash.SET_PED_AS_COP, cop2, true);
            if (passengers == 4)
            {
                Ped cop3 = cops.CreatePedOnSeat(VehicleSeat.LeftRear, PedHash.Cop01SFY);
                Function.Call(Hash.SET_PED_AS_COP, cop3, true);
                Ped cop4 = cops.CreatePedOnSeat(VehicleSeat.RightRear, PedHash.Cop01SMY);
                Function.Call(Hash.SET_PED_AS_COP, cop4, true);
            }
            return cops;
        }

        public static void SpawnPeds(int number, Vector3 location)
        {
            for (int i = 0; i < number; i++)
            {
                Ped cop = World.CreatePed(PedHash.Cop01SMY, location);
                Function.Call(Hash.SET_PED_AS_COP, cop, true);
            }
        }

        public static void MissionFailSpawn()
        {
            Vector3 carOneLocation = new Vector3(127.4381f, 361.2418f, 1000f);
            Vehicle copCarOne = SpawnCar(4, carOneLocation, 120f);
            Vector3 copCarOneTarget = new Vector3(137.5328f, 309.8216f, 1000f);
            copCarOneTarget.Z = World.GetGroundHeight(copCarOneTarget);
            copCarOne.Driver.Task.DriveTo(copCarOne, copCarOneTarget, 2f, 30f);

            Vehicle copCarTwo = SpawnCar(4, carOneLocation, 120f);
            Vector3 copCarTwoTarget = new Vector3(128.898f, 308.2f, 1000f);
            copCarTwoTarget.Z = World.GetGroundHeight(copCarTwoTarget);
            copCarTwo.Position += copCarTwo.ForwardVector * -30f;
            copCarTwo.Driver.Task.DriveTo(copCarTwo, copCarTwoTarget, 2f, 30f);

            Vector3 copPedLocation = new Vector3(154.6699f, 304.3619f, 1000f);
            copPedLocation.Z = World.GetGroundHeight(copPedLocation);
            SpawnPeds(2, copPedLocation);
        }
    }
}
