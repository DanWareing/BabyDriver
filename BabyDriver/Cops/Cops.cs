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
    }
}
