using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using BabyDriver.Locations;

namespace BabyDriver.Crew
{
    public static class Crew
    {

        public static List<CrewMember> SpawnCrew(Main instance, LocationSet locationSet)
        {
            List<CrewMember> crew = new List<CrewMember>();
            for (int i=0; i<locationSet.heistCrewSpawnLocations.Count; i++)
            {
                Vector3 crewMemberPos = locationSet.heistCrewSpawnLocations[i];
                crewMemberPos.Z = World.GetGroundHeight(new Vector2(crewMemberPos.X, crewMemberPos.Y));
                var crewPed = World.CreatePed(PedHash.Michael, crewMemberPos);
                crewPed.Rotation = locationSet.heistCrewSpawnRotation;
                crewPed.Money = 69;
                crewPed.Weapons.Give(WeaponHash.Pistol, 20, false, true);
                CrewMember crewMember = new CrewMember(instance, crewPed);
                crewMember.setToDefaultOutfit();
                crew.Add(crewMember);

                PedGroup playerGroup = Game.Player.Character.CurrentPedGroup;
                Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, crewPed, playerGroup);
                Function.Call(Hash.SET_PED_COMBAT_ABILITY, crewPed, 100);
            }
            return crew;
        }

    }
}
