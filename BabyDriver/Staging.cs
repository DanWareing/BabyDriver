using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;

namespace BabyDriver.Staging
{
    public static class StageConditions
    {

        // Conditions

        public static bool isReadyForResponseToCall(int docAnsweredTick)
        {
            return (docAnsweredTick > 0 && docAnsweredTick < Environment.TickCount - 6000);
        }

        public static bool isReadyForNewJob(Jobs.Stage stageAction, int newJobStartTick)
        {
            return (stageAction == Jobs.Stage.NONE && newJobStartTick != 0 && Environment.TickCount > newJobStartTick);
        }

        public static bool isStagePickupWaitReady(Jobs.Stage stageAction, List<Crew.CrewMember> currentCrew)
        {
            return (stageAction == Jobs.Stage.PICKUP_WAIT && currentCrew.All(x => x.ped.IsInVehicle()));
        }

        public static bool isStageEscapeReady(Jobs.Stage stageAction, List<Crew.CrewMember> currentCrew)
        {
            return (stageAction == Jobs.Stage.HEIST_WAIT && currentCrew.All(x => x.ped.IsInVehicle()));
        }

        public static bool isStageCloseSuccessReady (Jobs.Stage stageAction, Vector3 stageLocation, Vector3 playerPosition)
        {
            return (stageAction == Jobs.Stage.END && playerPosition.DistanceTo2D(stageLocation) > 50f);
        }

        public static bool isStageCloseFailReady(Jobs.Stage stageAction)
        {
            return (stageAction == Jobs.Stage.FAIL && Game.Player.Character.CurrentVehicle.IsStopped && Game.Player.WantedLevel == 0);
        }

        // Stage Progression



    }
}
