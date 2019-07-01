using GTA;
using GTA.Math;
using GTA.Native;

namespace BabyDriver.Crew
{
    public class CrewMember
    {

        // Variables
        public Main instance;
        public Ped ped;
        public Ped heistTargetPed;
        public Vector3 targetLocation;
        public float distanceFromTargetLocation;
        public bool shouldEnterPlayerVehicle = false;
        public bool isRunning = false;
        public bool isEnteringVehicle = false;
        public bool isAliveAtDropoff = false;
        public bool followPlayer = false;
        public bool isMovingToLocation = false;
        public bool isFollowing = false;

        // Constructor
        public CrewMember(Main instance, Ped ped)
        {
            this.instance = instance;
            this.ped = ped;
        }

        // On Tick
        public void onTick()
        {
            Vehicle playerVehicle = Game.Player.Character.CurrentVehicle;
            Vehicle pedVehicle = this.ped.CurrentVehicle;
            if (this.followPlayer) targetLocation = Game.Player.Character.Position;
            distanceFromTargetLocation = this.ped.Position.DistanceTo2D(targetLocation);

            if (playerVehicle == null && pedVehicle != null) this.ped.Task.LeaveVehicle();
            if (this.distanceFromTargetLocation < 4f) this.isMovingToLocation = false;
            if (playerVehicle != null && this.ped.Position.DistanceTo2D(playerVehicle.Position) > 5f) this.isEnteringVehicle = false;

            if (this.followPlayer && !this.isFollowing)
            {
                if (playerVehicle != null && playerVehicle == pedVehicle)
                {

                } else
                {
                    this.shouldEnterPlayerVehicle = true;
                    this.isFollowing = true;
                    float speed = isRunning ? 5f : 1f;
                    Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY, this.ped, Game.Player.Character, 0f, 0f, 0f, speed, -1, 1.0f, 1);
                }
            }

            if (this.ped.CurrentVehicle == Game.Player.Character.CurrentVehicle) this.hasEnteredVehicle();
            if (this.shouldBeginEnter()) this.getInVehicle(playerVehicle);
            if (this.shouldBeginHeist()) this.beginHeist();
        }

        // Helper Methods
        public void hasEnteredVehicle()
        {
            this.shouldEnterPlayerVehicle = false;
            this.isEnteringVehicle = false;
            this.isFollowing = false;
            if (this.ped.Money > 69) this.setToDefaultOutfit();
        }

        private bool shouldBeginEnter()
        {
            return (
                Game.Player.Character.CurrentVehicle != null && this.shouldEnterPlayerVehicle && !this.isEnteringVehicle && this.distanceFromTargetLocation < 4f
            );
        }

        private void getInVehicle(Vehicle vehicle)
        {
            this.isEnteringVehicle = true;
            this.ped.Task.ClearAll();
            foreach (VehicleSeat seat in Main.seats)
            {
                if (Game.Player.Character.CurrentVehicle.IsSeatFree(seat))
                {
                    this.ped.Task.EnterVehicle(Game.Player.Character.CurrentVehicle, seat);
                    break;
                }
            }
        }

        public void moveToTargetLocation()
        {
            this.isMovingToLocation = true;
            this.ped.Task.ClearAll();
            if (isRunning)
            {
                this.ped.Task.RunTo(this.targetLocation, false, -1);
            }
            else
            {
                this.ped.Task.GoTo(this.targetLocation, false, -1);
            }
        }

        private bool shouldBeginHeist()
        {
            return (
                this.heistTargetPed != null && this.ped.Position.DistanceTo(this.heistTargetPed.Position) < 4f
            );
        }

        private void beginHeist()
        {
            this.ped.Task.ClearAll();
            this.ped.CanSwitchWeapons = true;
            this.ped.Weapons.Select(this.ped.Weapons.BestWeapon.Hash, true);
            this.ped.Task.AimAt(heistTargetPed, -1);
            if (instance.currentStageAction == Jobs.Stage.HEIST_ARRIVAL) instance.currentStageAction = Jobs.Stage.HEIST_START;
        }

        public void setToHeistOutfit()
        {
            if (ped.Model == PedHash.Franklin)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 1, 2, 0, 0); // Head
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 2, 4, 0, 0); // Hair
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 3, 13, 0, 0); // Torso
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 4, 13, 0, 0); // Legs
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 5, 1, 0, 0); // Gloves
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 4, 0, 0); // Accessories
            }
            else if (ped.Model == PedHash.Michael)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 1, 3, 0, 0); // Head
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 2, 1, 0, 0); // Hair
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 3, 30, 0, 0); // Torso
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 4, 4, 0, 0); // Legs
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 5, 1, 0, 0); // Gloves
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 6, 4, 0, 0); // Feet
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 7, 0, 1); // Accessories
            }
            else if (ped.Model == PedHash.Trevor)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 1, 2, 0, 0); // Head
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 2, 1, 0, 0); // Hair
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 3, 14, 0, 0); // Torso
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 4, 18, 0, 0); // Legs
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 5, 1, 0, 0); // Gloves
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 6, 5, 0, 0); // Feet
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 6, 0, 2); // Accessories
            }
        }

        public void setToDefaultOutfit()
        {
            if (ped.Model == PedHash.Franklin)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 1, 2, 0, 0); // Head
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 2, 4, 0, 0); // Hair
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 3, 13, 0, 0); // Torso
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 4, 13, 0, 0); // Legs
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 5, 0, 0, 0); // Gloves
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 0, 0, 0); // Accessories
            }
            else if (ped.Model == PedHash.Michael)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 1, 3, 0, 0); // Head
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 2, 4, 0, 0); // Hair
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 3, 30, 0, 0); // Torso
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 4, 4, 0, 0); // Legs
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 6, 4, 0, 0); // Feet
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 5, 0, 0, 0); // Gloves
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 0, 0, 0); // Accessories
            }
            else if (ped.Model == PedHash.Trevor)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 1, 2, 0, 0); // Head
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 2, 1, 0, 0); // Hair
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 3, 14, 0, 0); // Torso
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 4, 18, 0, 0); // Legs
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 6, 5, 0, 0); // Feet
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 5, 0, 0, 0); // Gloves
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 0, 0, 0); // Accessories
            }
        }

    }
}
