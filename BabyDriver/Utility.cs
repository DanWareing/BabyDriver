using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace BabyDriver
{
    public static class Utility
    {
        public static Vector3 heightCorrection(Vector3 givenLocation, Jobs.Stage currentStageAction)
        {
            givenLocation.Z = currentStageAction == Jobs.Stage.ESCAPE ? 111f : World.GetGroundHeight(new Vector2(givenLocation.X, givenLocation.Y));
            return givenLocation;
        }

        public static void removeVehicles(Vector3 givenLocation, float radius)
        {
            Vehicle[] vehicles = World.GetAllVehicles();
            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle.Position.DistanceTo2D(givenLocation) < radius && vehicle != Game.Player.Character.CurrentVehicle) vehicle.Delete();
            }
        }

        public static void DisplayHelpTextThisFrame(string text)
        {
            Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0x238FFE5C7B0498A6, 0, 0, 1, -1);
        }

        public static void DisplayNotificationThisFrame(string messageText, string name, string title)
        {
            Function.Call(Hash._SET_NOTIFICATION_TEXT_ENTRY, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, messageText);
            Function.Call(Hash._SET_NOTIFICATION_MESSAGE, "CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", false, 4, title, name);
            Function.Call(Hash._DRAW_NOTIFICATION, false, true);
        }

        public static void DeleteAllNearLocation(Vector3 givenLocation, float radius)
        {
            foreach (Ped ped in World.GetAllPeds())
            {
                if (ped != Game.Player.Character) ped.Delete();
            }
            foreach (Vehicle vehicle in World.GetNearbyVehicles(givenLocation, radius))
            {
                vehicle.Delete();
            }
        }

        public static string IsVehicleEligible(Vehicle givenVehicle, List<Vehicle> eligibleVehicles)
        {
            return eligibleVehicles.Contains(givenVehicle) ? "Eligible" : "Not Eligible";
        }

        public static void SpawnPoliceAhead(Vector3 playerPosition)
        {
            float playerSpeed = Game.Player.Character.CurrentVehicle != null ? Game.Player.Character.CurrentVehicle.Speed : 1f;
            float minDistanceForSpawn = playerSpeed * 3f > 200f ? 300f : 200f;
            Vehicle cops = World.CreateVehicle(
                VehicleHash.Police, World.GetNextPositionOnStreet(
                    playerPosition + Game.Player.Character.ForwardVector * 300f, true
                )
            );
            cops.Speed = Math.Min(60f, playerSpeed);
            Ped cop1 = cops.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Cop01SFY);
            Function.Call(Hash.SET_PED_AS_COP, cop1, true);
            Ped cop2 = cops.CreatePedOnSeat(VehicleSeat.Passenger, PedHash.Cop01SMY);
            Function.Call(Hash.SET_PED_AS_COP, cop2, true);
            cop1.Task.CruiseWithVehicle(cops, 20f, 786603);
        }

        public static List<string> GetNearbyPedModels()
        {
            List<string> pedModels = new List<string>();
            foreach (Ped ped in World.GetAllPeds())
            {
                if (ped.Position.DistanceTo2D(Game.Player.Character.Position) < 10f && ped != Game.Player.Character)
                {
                    pedModels.Add(ped.Model.ToString());
                }
            }
            return pedModels;
        }

        public static bool IsPlayerDeadOrBusted()
        {
            return (Game.Player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, 0));
        }

        public static bool IsPlayerDriving()
        {
            return (
                Game.Player.Character.IsInVehicle() &&
                Game.Player.Character.CurrentVehicle.Exists() &&
                Game.Player.Character.CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver) == Game.Player.Character
            );
        }
        public static Blip createBlip(Vector3 location, string name)
        {
            Blip currentStageBlip = World.CreateBlip(location);
            currentStageBlip.ShowRoute = true;
            currentStageBlip.Name = name;
            currentStageBlip.Color = BlipColor.Yellow2;
            switch (name)
            {
                case "Crew Pickup":
                    currentStageBlip.Sprite = BlipSprite.Cab;
                    break;
                case "Heist Location":
                    currentStageBlip.Sprite = BlipSprite.DollarSign;
                    break;
                case "Crew Drop-off":
                    currentStageBlip.Sprite = BlipSprite.Garage;
                    break;
            }
            return currentStageBlip;
        }

    }
}
