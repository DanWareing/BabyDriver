using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;

namespace BabyDriver.Cops
{
    public class Cop
    {

        // Variables
        public Main instance;
        public Ped ped;
        private float distanceToPlayer;
        public bool isInSightOfPlayer;

        // Constructor
        public Cop(Main instance, Ped ped)
        {
            this.instance = instance;
            this.ped = ped;
        }

        // On Tick
        public void onTick()
        {
            distanceToPlayer = ped.Position.DistanceTo(Game.Player.Character.Position);
            if (distanceToPlayer > 150f && Game.Player.WantedLevel == 0)
            {
                this.ped.MarkAsNoLongerNeeded();
            }

            checkIfInSightOfPlayer();
            Vehicle playerVehicle = Game.Player.Character.CurrentVehicle;
            if (
                isInSightOfPlayer && playerVehicle != null && instance != null &&
                (
                    instance.wantedVehicleName != null && playerVehicle.DisplayName == instance.wantedVehicleName &&
                    playerVehicle.PrimaryColor == instance.wantedVehicleColor
                ) ||
                playerVehicle.IsStolen
            )
            {
                Game.Player.WantedLevel = instance.prevWantedLevel > 0 ? instance.prevWantedLevel : 1;
            }

        }

        // Methods
        private void checkIfInSightOfPlayer()
        {
            float distanceToPlayer = ped.Position.DistanceTo(instance.currentPlayerPosition);
            if (distanceToPlayer < 100f)
            {
                RaycastResult rayResult;
                Vector3 castFrom = this.ped.Position + new Vector3(0f, 0f, 0.1f);

                rayResult = World.Raycast(
                    castFrom,
                    instance.currentPlayerPosition,
                    IntersectOptions.Everything
                );

                if (ped.CurrentVehicle != null && rayResult.HitEntity == ped.CurrentVehicle)
                {
                    castFrom = rayResult.HitCoords;
                    rayResult = World.Raycast(
                        castFrom,
                        instance.currentPlayerPosition,
                        IntersectOptions.Everything
                    );
                }

                if (rayResult.HitEntity == Game.Player.Character.CurrentVehicle)
                {
                    Vector2 pedPos = new Vector2(ped.Position.X, ped.Position.Y);
                    Vector2 pedFwdPos = new Vector2(ped.Position.X + ped.ForwardVector.X, ped.Position.Y + ped.ForwardVector.Y);
                    Vector2 playerPos = new Vector2(Game.Player.Character.Position.X, Game.Player.Character.Position.Y);
                    Vector2 pedVec = (pedFwdPos - pedPos);
                    Vector2 playerVec = (playerPos - pedPos);
                    float angle = Vector2.Angle(pedVec, playerVec);

                    if (
                        (angle < 80f && distanceToPlayer < 40f) ||
                        (angle < 130f && distanceToPlayer < 20f) ||
                        (angle > 230f && distanceToPlayer < 20f)
                    )
                    {
                        this.isInSightOfPlayer = true;
                    }
                }
            }
        }

    }
}
