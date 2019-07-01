using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using iFruitAddon2;
using BabyDriver.Cops;
using BabyDriver.Crew;
using BabyDriver.Jobs;
using BabyDriver.Locations;
using BabyDriver.Staging;

namespace BabyDriver
{

    // Main Class
    public class Main : Script
    {

        // CONSTANTS
        const BlipColor MY_BLIP_COLOR = BlipColor.Yellow2;
        const string CREW_PICKUP_BLIP_NAME = "Crew Pickup";
        const BlipSprite CREW_PICKUP_BLIP_SPRITE = BlipSprite.Cab;
        const string HEIST_LOCATION_BLIP_NAME = "Heist Location";
        const BlipSprite HEIST_LOCATION_BLIP_SPRITE = BlipSprite.DollarSign;
        const string CREW_DROPOFF_BLIP_NAME = "Crew Drop-off";
        const BlipSprite CREW_DROPOFF_BLIP_SPRITE = BlipSprite.Garage;
        const int NEW_JOB_MIN_TICKS = 10000;
        const int NEW_JOB_MAX_TICKS = 11000;

        // iFruit
        private List<string> messageQueue = new List<string>();
        private int messageQueueInterval;
        private int messageQueueReferenceTime;
        CustomiFruit _iFruit;

        // NativeUI
        UIMenu mainMenu;
        MenuPool menuPool;

        // VARIABLES
        public static List<VehicleSeat> seats = new List<VehicleSeat>() { VehicleSeat.LeftFront, VehicleSeat.LeftRear, VehicleSeat.RightFront, VehicleSeat.RightRear };
        private bool isPlayerEligible = true;
        private Job currentJob;
        public Vector3 currentPlayerPosition;
        private Vector3 currentStageLocation;
        private Blip currentStageBlip;
        public Jobs.Stage currentStageAction;
        private bool hasPreparedLocation;
        private List<CrewMember> currentCrew = new List<CrewMember>();
        private List<Ped> heistTargets = new List<Ped>();
        private List<Ped> allPedsInvolved = new List<Ped>();
        private List<Cop> currentCops = new List<Cop>();
        private List<Vehicle> eligibleVehicles = new List<Vehicle>();
        public List<Ped> testPeds = new List<Ped>();
        bool debugMode;
        
        int newJobStartTick;
        int heistEndTick;
        int jobPayout;

        bool showPoliceOnMap = false;
        int currentLevel = 1;

        string currentSubtitle;
        int subtitleEndTick;

        public string wantedVehicleName;
        public VehicleColor wantedVehicleColor;
        public int prevWantedLevel;
        Vehicle vehicleToReportStolen;
        int reportStolenTick;

        // Main - Constructor
        public Main()
        {
            resetAll();
            Function.Call(Hash.SET_CREATE_RANDOM_COPS, true);
            LocationSets.setupLocationSets();
            setupPhone();
            this.Tick += onTick;
            this.KeyUp += onKeyUp;
            this.KeyDown += onKeyDown;
            if (Game.Player.Character.CurrentVehicle != null && Game.Player.Character.CurrentVehicle.IsStolen) eligibleVehicles.Add(Game.Player.Character.CurrentVehicle);
            UI.Notify("Scripts loaded.");
        }

        // Main - On Tick
        private void onTick(object sender, EventArgs e)
        {
            // Always
            this.handleMessages();
            this.handleSubtitles();
            _iFruit.Update();
            menuPool.ProcessMenus();
            currentPlayerPosition = Game.Player.Character.GetOffsetInWorldCoords(new Vector3(0, 0, 0));

            if (vehicleToReportStolen != null && Game.Player.Character.CurrentVehicle != null && 
                vehicleToReportStolen == Game.Player.Character.CurrentVehicle &&
                Environment.TickCount > reportStolenTick
            ) {
                Game.Player.Character.CurrentVehicle.IsStolen = true;
                vehicleToReportStolen = null;
                reportStolenTick = 0;
            }

            if (Game.Player.WantedLevel > 0 && this.wantedVehicleName == null)
            {
                if (
                    Game.Player.Character.CurrentVehicle != null && Game.Player.Character.CurrentVehicle.DisplayName != this.wantedVehicleName)
                {
                    this.wantedVehicleName = Game.Player.Character.CurrentVehicle.DisplayName;
                    this.wantedVehicleColor = Game.Player.Character.CurrentVehicle.PrimaryColor;
                }
            }
            this.prevWantedLevel = Game.Player.WantedLevel > 0 ? Game.Player.WantedLevel : this.prevWantedLevel;

            // World Management
            foreach (Vehicle vehicle in World.GetNearbyVehicles(Game.Player.Character, 20f))
            {
                if (!eligibleVehicles.Contains(vehicle) && !vehicle.Driver.Exists())
                {
                    vehicle.LockStatus = VehicleLockStatus.CanBeBrokenInto;
                    vehicle.HasAlarm = true;
                    vehicle.NeedsToBeHotwired = true;
                }
            }

            // Vehicle Management
            if (Game.Player.Character.IsInVehicle() && !Game.Player.Character.CurrentVehicle.AlarmActive && !Game.Player.Character.CurrentVehicle.EngineRunning)
            {
                Game.Player.Character.CurrentVehicle.LockStatus = VehicleLockStatus.Unlocked;
                Game.Player.Character.CurrentVehicle.StartAlarm();
                vehicleToReportStolen = Game.Player.Character.CurrentVehicle;
                reportStolenTick = Environment.TickCount + 120000;
            }
            if (Utility.IsPlayerDriving()) vehicleOnTickUpdates();
            if (Game.Player.Character.IsInVehicle()) Game.Player.Character.CurrentVehicle.HandbrakeOn = shouldDisableVehicle();
            if (Game.Player.Character.IsJacking) playerIsJackingCar();

            // 
            if (StageConditions.isReadyForNewJob(currentStageAction, newJobStartTick)) createNewJob();
            if (isCloseToCurrentStageLocation()) closeToStageLocationActions();
            if (isAtCurrentStageLocation()) nextStage();
            if (shouldSpawnCrew()) spawnCrew();
            if (StageConditions.isStageCloseSuccessReady(currentStageAction, currentStageLocation, currentPlayerPosition)) stageCloseSuccess();
            if (StageConditions.isStageCloseFailReady(currentStageAction)) stageCloseFail();
            if (Utility.IsPlayerDeadOrBusted() && currentStageAction != Jobs.Stage.NONE) resetAll();

            // Cop Mechanics
            Game.ShowsPoliceBlipsOnRadar = showPoliceOnMap;

            currentCops = new List<Cop>();
            foreach (Ped cop in World.GetNearbyPeds(currentPlayerPosition, 100f).Where(x => x.RelationshipGroup.GetHashCode() == -1533126372)) {
                currentCops.Add(new Cop(this, cop));
            }

            // Subclass Onticks
            foreach (CrewMember crewMember in currentCrew) crewMember.onTick();
            foreach (Cop cop in currentCops)
            {
                if (cop.ped.Exists())
                {
                    cop.onTick();
                }
            }
            
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (debugMode && e.KeyCode == Keys.NumPad1)
            {
                UI.Notify(currentPlayerPosition.ToString());
                UI.Notify(currentStageLocation.ToString());
                UI.Notify(currentStageAction.ToString());
                UI.Notify(prevWantedLevel.ToString());
                if (wantedVehicleName != null) UI.Notify(wantedVehicleName.ToString());
                if (wantedVehicleColor != null) UI.Notify(wantedVehicleColor.ToString());
                if (Game.Player.Character.CurrentVehicle != null) UI.Notify(Game.Player.Character.CurrentVehicle.PrimaryColor.ToString());
                if (Game.Player.Character.CurrentVehicle != null) UI.Notify(Game.Player.Character.CurrentVehicle.IsStolen.ToString());
            }
            if (e.KeyCode == Keys.NumPad0)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }

        }

        // Staging Methods
        private void stagePickup()
        {
            if (eligibleVehicles.Contains(Game.Player.Character.CurrentVehicle))
            {
                if (Game.Player.Character.CurrentVehicle.PassengerSeats < currentCrew.Count)
                {
                    Random random = new Random();
                    int index = random.Next(Dialogue.notEnoughSeats.Count);
                    string response = Dialogue.notEnoughSeats[index];
                    UI.ShowSubtitle(response, 5);
                } else
                {
                    foreach (CrewMember crewMember in currentCrew)
                    {
                        crewMember.followPlayer = true;
                    }
                    currentStageAction = Jobs.Stage.PICKUP_WAIT;
                }
            } else {
                Random random = new Random();
                int index = random.Next(Dialogue.notStolen.Count);
                string response = Dialogue.notStolen[index];
                UI.ShowSubtitle(response, 5);
            }
        }

        private void stagePickupWait()
        {
            if (currentStageBlip != null && currentStageBlip.Exists()) currentStageBlip.Remove();
            currentStageLocation = currentJob.locationSet.heistDropoffLocation;
            currentStageLocation.Z = World.GetGroundHeight(new Vector2(currentStageLocation.X, currentStageLocation.Y));
            createBlip(currentStageLocation, true, HEIST_LOCATION_BLIP_NAME, HEIST_LOCATION_BLIP_SPRITE);
            currentStageAction = Jobs.Stage.JOURNEY;
        }
        
        private void stageHeistArrival()
        {
            Ped heistTargetPed = currentJob.GetTargetPed();
            heistTargets.Add(heistTargetPed);
            allPedsInvolved.Add(heistTargetPed);
            Vector3 heistLocation = currentJob.locationSet.heistStartLocations[0];
            heistLocation.Z = World.GetGroundHeight(new Vector2(heistLocation.X, heistLocation.Y));
            foreach (CrewMember crewMember in currentCrew)
            {
                crewMember.setToHeistOutfit();
                crewMember.followPlayer = false;
                crewMember.ped.Task.LeaveVehicle();
                crewMember.heistTargetPed = heistTargetPed;
                crewMember.targetLocation = heistTargetPed.Position;
                crewMember.moveToTargetLocation();
            }
            currentStageAction = Jobs.Stage.HEIST_ARRIVAL;
        }

        private void stageHeistStart()
        {
            foreach (Ped ped in heistTargets) ped.Task.HandsUp(120);
            heistEndTick = Environment.TickCount + currentJob.length;
        }

        private void stageHeistWait()
        {
            jobPayout = new Random().Next(currentJob.valueLower, currentJob.valueUpper);
            foreach (CrewMember crewMember in currentCrew)
            {
                crewMember.ped.Money = jobPayout / currentCrew.Count();
                crewMember.heistTargetPed = null;
                crewMember.isRunning = true;
                crewMember.followPlayer = true;
            }
            currentStageAction = Jobs.Stage.HEIST_WAIT;
        }

        private void stageEscape()
        {
            Game.Player.WantedLevel = 2;
            if (currentStageBlip != null && currentStageBlip.Exists()) currentStageBlip.Remove();
            currentStageLocation = new Vector3(146.7975f, 321.4382f, 1000f);
            createBlip(currentStageLocation, true, CREW_DROPOFF_BLIP_NAME, CREW_DROPOFF_BLIP_SPRITE);
            currentStageAction = Jobs.Stage.ESCAPE;
        }

        private void stageEnd()
        {
            if (Game.Player.WantedLevel == 0)
            {
                int jobPayoutEach = jobPayout / (currentCrew.Count() + 1);
                foreach (CrewMember crewMember in currentCrew)
                {
                    crewMember.ped.Money = jobPayoutEach;
                    crewMember.followPlayer = false;
                    crewMember.ped.Task.LeaveVehicle();
                    crewMember.isAliveAtDropoff = true;
                }
                Game.Player.Money += jobPayoutEach;
                currentStageAction = Jobs.Stage.END;
                currentJob.locationSet.isComplete = true;
                if (currentStageBlip != null && currentStageBlip.Exists()) currentStageBlip.Remove();
                currentLevel++;
            } else
            {
                currentStageAction = Jobs.Stage.FAIL;
                Random random = new Random();
                int index = random.Next(Dialogue.wantedOnArrival.Count);
                string response = Dialogue.wantedOnArrival[index];
                UI.ShowSubtitle(response, 5);

                // Spawn Cop Cars and Peds
                Vector3 carOneLocation = new Vector3(127.4381f, 361.2418f, 1000f);
                Vehicle copCarOne = CopGenerator.SpawnCar(4, carOneLocation, 120f);
                Vector3 copCarOneTarget = new Vector3(137.5328f, 309.8216f, 1000f);
                copCarOneTarget.Z = World.GetGroundHeight(copCarOneTarget);
                copCarOne.Driver.Task.DriveTo(copCarOne, copCarOneTarget, 2f, 30f);

                Vehicle copCarTwo = CopGenerator.SpawnCar(4, carOneLocation, 120f);
                Vector3 copCarTwoTarget = new Vector3(128.898f, 308.2f, 1000f);
                copCarTwoTarget.Z = World.GetGroundHeight(copCarTwoTarget);
                copCarTwo.Position += copCarTwo.ForwardVector * -30f;
                copCarTwo.Driver.Task.DriveTo(copCarTwo, copCarTwoTarget, 2f, 30f);

                Vector3 copPedLocation = new Vector3(154.6699f, 304.3619f, 1000f);
                copPedLocation.Z = World.GetGroundHeight(copPedLocation);
                CopGenerator.SpawnPeds(4, copPedLocation);

            }
        }

        private void stageCloseSuccess()
        {
            foreach (CrewMember crewMember in currentCrew)
            {
                if (crewMember.isAliveAtDropoff && !crewMember.ped.IsAlive && crewMember.ped.GetKiller() == Game.Player.Character)
                {
                    isPlayerEligible = false;
                }
            }
            string messageText = isPlayerEligible ?
                "I've just heard the news, congratulations. Call me tomorrow." :
                "You've just made some very powerful enemies. Never contact me again, and sleep with one eye open...";
            Function.Call(Hash._SET_NOTIFICATION_TEXT_ENTRY, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, messageText);
            Function.Call(Hash._SET_NOTIFICATION_MESSAGE, "CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", false, 4, "Message", "Doc");
            Function.Call(Hash._DRAW_NOTIFICATION, false, true);
            Function.Call(Hash.PLAY_MISSION_COMPLETE_AUDIO, "FRANKLIN_BIG_01");
            resetAll();
        }

        private void stageCloseFail()
        {
            foreach (CrewMember crewMember in currentCrew)
            {
                if (crewMember.ped.IsInVehicle())
                {
                    crewMember.ped.Task.LeaveVehicle();
                    crewMember.ped.MarkAsNoLongerNeeded();
                }
            }
            Random random = new Random();
            int index = random.Next(Dialogue.stageCloseFail.Count);
            string response = Dialogue.stageCloseFail[index];
            UI.ShowSubtitle(response, 5);
        }

        
        // Helper Methods
        private void vehicleOnTickUpdates()
        {
            Vehicle currentVehicle = Game.Player.Character.CurrentVehicle;
            currentVehicle.HandbrakeOn = shouldDisableVehicle();

            if ((!eligibleVehicles.Contains(Game.Player.Character.CurrentVehicle) && (currentVehicle.IsStolen)))
            {
                eligibleVehicles.Add(Game.Player.Character.CurrentVehicle);
                // Set Color in Dict
            }
            foreach (Vehicle vehicle in eligibleVehicles)
            {
                if (vehicle.Exists())
                {
                    // Color check
                    // Set IsStolen to false
                } 
            }

            Vehicle attempt = Game.Player.Character.GetVehicleIsTryingToEnter();
            if (attempt.Driver == null && attempt.HasBeenDamagedBy(Game.Player.Character) && !attempt.AlarmActive) attempt.StartAlarm();
            if (attempt.Driver != null && attempt.EngineRunning && attempt.AlarmActive) Function.Call(Hash.SET_VEHICLE_ALARM, attempt, false);
        }

        private void createBlip(Vector3 location, bool showRoute, string name, BlipSprite sprite)
        {
            currentStageBlip = World.CreateBlip(location);
            currentStageBlip.ShowRoute = showRoute;
            currentStageBlip.Name = name;
            currentStageBlip.Sprite = sprite;
            currentStageBlip.Color = MY_BLIP_COLOR;
        }

        private void nextStage()
        {
            if (currentStageAction == Jobs.Stage.PICKUP) { stagePickup(); }
            else if (StageConditions.isStagePickupWaitReady(currentStageAction, currentCrew)) { stagePickupWait(); }
            else if (currentStageAction == Jobs.Stage.JOURNEY) { stageHeistArrival(); }
            else if (currentStageAction == Jobs.Stage.HEIST_START && heistEndTick == 0) { stageHeistStart(); }
            else if (currentStageAction == Jobs.Stage.HEIST_START && Environment.TickCount > heistEndTick) { stageHeistWait(); }
            else if (StageConditions.isStageEscapeReady(currentStageAction, currentCrew)) { stageEscape(); }
            else if (currentStageAction == Jobs.Stage.ESCAPE) { stageEnd(); }
        }

        private void handleMessages()
        {
            if (this.messageQueue.Count > 0)
            {
                Utility.DisplayHelpTextThisFrame(this.messageQueue[0]);
            }
            else
            {
                this.messageQueueReferenceTime = Game.GameTime;
            }
            if (Game.GameTime <= this.messageQueueReferenceTime + this.messageQueueInterval || this.messageQueue.Count <= 0) return;
            this.messageQueue.RemoveAt(0);
            this.messageQueueReferenceTime = Game.GameTime;
        }

        private void closeToStageLocationActions()
        {
            if (!hasPreparedLocation)
            {
                hasPreparedLocation = true;
                Utility.removeVehicles(currentStageLocation, 20f);
            }
            if (currentStageAction != Stage.END && currentStageAction != Stage.FAIL)
            {
                drawMarker();
            }
        }

        private void drawMarker()
        {
            if ((currentStageLocation.Z == 1000f || currentStageLocation.Z == 0f) && currentStageLocation.Y != 0f) currentStageLocation = Utility.heightCorrection(currentStageLocation, currentStageAction);
            World.DrawMarker(
                MarkerTy﻿pe.VerticalCylinder,  // type
                currentStageLocation,         // pos
                Vector3.Zero,                 // dir
                Vector3.Zero,                 // rot
                new Vector3(3f, 3f, 1f),      // scale
                C﻿olor.Yellow                  // color
            );
        }

        private void setupPhone()
        {
            _iFruit = new CustomiFruit();
            iFruitContact contactDoc = new iFruitContact("Doc");
            contactDoc.Answered += docAnswered;
            contactDoc.DialTimeout = 4000;
            contactDoc.Active = true;
            contactDoc.Icon = ContactIcon.Blank;
            _iFruit.Contacts.Add(contactDoc);
        }

        private void docAnswered(iFruitContact contact)
        {
            Random random = new Random();
            int index = random.Next(Dialogue.docPositive.Count);
            string docResponse = Dialogue.docPositive[index];
            newJobStartTick = Environment.TickCount + new Random().Next(NEW_JOB_MIN_TICKS, NEW_JOB_MAX_TICKS);
            _iFruit.Close(5000);
            currentSubtitle = "Doc: " + docResponse;
            subtitleEndTick = Environment.TickCount + 4000;
        }

        private void createNewJob()
        {
            currentJob = JobFactory.CreateNewJob(currentLevel);
            currentStageLocation = currentJob.locationSet.heistPickupLocation;
            currentStageLocation.Z = World.GetGroundHeight(new Vector2(currentStageLocation.X, currentStageLocation.Y));
            createBlip(currentStageLocation, true, CREW_PICKUP_BLIP_NAME, BlipSprite.Cab);
            currentStageAction = Jobs.Stage.PICKUP;
            Utility.DisplayNotificationThisFrame(
                "Okay, we're ready. Drop by.", "Doc", "Message"
            );
            newJobStartTick = 0;
        }

        private void spawnCrew()
        {
            Vector3 crewMemberPos = currentJob.locationSet.heistCrewSpawnLocations[0];
            crewMemberPos.Z = World.GetGroundHeight(new Vector2(crewMemberPos.X, crewMemberPos.Y));
            var crewPed = World.CreatePed(PedHash.Michael, crewMemberPos);
            crewPed.Rotation = currentJob.locationSet.heistCrewSpawnRotation;
            crewPed.Money = 69;
            allPedsInvolved.Add(crewPed);
            crewPed.Weapons.Give(WeaponHash.Pistol, 20, false, true);
            CrewMember crewMember = new CrewMember(this, crewPed);
            crewMember.setToDefaultOutfit();
            currentCrew.Add(crewMember);

            PedGroup playerGroup = Game.Player.Character.CurrentPedGroup;
            Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, crewPed, playerGroup);
            Function.Call(Hash.SET_PED_COMBAT_ABILITY, crewPed, 100);
        }

        private void resetAll()
        {
            mainMenu = new UIMenu("Baby Driver", "~b~SETTINGS");

            UIMenuCheckboxItem policeCheckbox = new UIMenuCheckboxItem("Show police blips on minimap?", false);
            mainMenu.AddItem(policeCheckbox);
            mainMenu.OnCheckboxChange += (sender, item, isChecked) =>
            {
                if (item == policeCheckbox) showPoliceOnMap = isChecked;
            };

            UIMenuListItem levelList = new UIMenuListItem("Current Level:", new List<dynamic> { 0, 1, 2, 3 }, 1);
            mainMenu.AddItem(levelList);
            mainMenu.OnListChange += (sender, item, index) =>
            {
                if (item == levelList) currentLevel = (int)item.Items[index];
            };

            mainMenu.RefreshIndex();
            menuPool = new MenuPool();
            menuPool.Add(mainMenu);

            newJobStartTick = 0;
            heistEndTick = 0;
            jobPayout = 0;
            currentPlayerPosition = Game.Player.Character.GetOffsetInWorldCoords(new Vector3(0, 0, 0));
            currentStageLocation = new Vector3(0f, 0f, 0f);
            currentStageAction = Jobs.Stage.NONE;
            currentJob = null;

            foreach(Blip blip in World.GetActiveBlips())
            {
                if (blip.Color == MY_BLIP_COLOR && (
                    blip.Sprite == CREW_PICKUP_BLIP_SPRITE ||
                    blip.Sprite == HEIST_LOCATION_BLIP_SPRITE ||
                    blip.Sprite == CREW_DROPOFF_BLIP_SPRITE
                ))
                {
                    blip.Remove();
                }
            }
            currentStageBlip = null;

            foreach (Ped ped in World.GetAllPeds())
            {
                int headComp = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, ped, 1);
                int torsoComp = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, ped, 3);
                int legsComp = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, ped, 4);
                if (ped != Game.Player.Character && (
                    (ped.Model == PedHash.Michael && headComp == 3 && torsoComp == 30 && legsComp == 4) ||
                    (ped.Model == PedHash.Trevor && headComp == 2 && torsoComp == 14 && legsComp == 18) ||
                    (ped.Model == PedHash.Franklin && headComp == 2 && torsoComp == 13 && legsComp == 13)
                )) {
                    ped.Delete();
                }
            }

            currentCrew = new List<CrewMember>();
            heistTargets = new List<Ped>();
            allPedsInvolved = new List<Ped>();
            eligibleVehicles = new List<Vehicle>();
    }

        private void playerIsJackingCar()
        {
            Game.Player.Character.GetVehicleIsTryingToEnter().IsStolen = true;
        }

        // Conditions for Other

        private bool isCloseToCurrentStageLocation()
        {
            return (
                currentPlayerPosition.DistanceTo2D(currentStageLocation) < 100f &&
                currentStageLocation != new Vector3(0f, 0f, 0f)
            );
        }

        private bool isAtCurrentStageLocation()
        {
            return (
                currentPlayerPosition.DistanceTo(currentStageLocation) < 3f &&
                currentStageLocation != new Vector3(0f, 0f, 0f)
            );
        }

        private bool shouldSpawnCrew()
        {
            return (
                currentStageAction == Jobs.Stage.PICKUP &&
                currentCrew.Count == 0 &&
                currentPlayerPosition.DistanceTo(currentStageLocation) < 100f
            );
        }

        private bool shouldDisableVehicle()
        {
            return (
                (Game.Player.Character.Position.DistanceTo(currentStageLocation)) < 3f &&
                // Player waiting to pick-up
                ((currentStageAction == Jobs.Stage.PICKUP_WAIT && !currentCrew.All(x => x.ped.IsInVehicle())) ||
                // Player waiting to drop-off
                (
                    (
                        currentStageAction == Jobs.Stage.JOURNEY ||
                        currentStageAction == Jobs.Stage.END
                    ) &&
                    (
                        currentCrew.Count(x => x.ped.IsInVehicle()) > 0 || 
                        (
                            Game.Player.Character.CurrentVehicle.IsDoorOpen(VehicleDoor.BackLeftDoor) ||
                            Game.Player.Character.CurrentVehicle.IsDoorOpen(VehicleDoor.BackRightDoor) ||
                            Game.Player.Character.CurrentVehicle.IsDoorOpen(VehicleDoor.FrontLeftDoor) ||
                            Game.Player.Character.CurrentVehicle.IsDoorOpen(VehicleDoor.FrontRightDoor)
                        )
                    )
                ))
            );
        }

        private void handleSubtitles()
        {
            if (currentSubtitle != null && subtitleEndTick >= Environment.TickCount)
            {
                UI.ShowSubtitle(currentSubtitle);
            } else if (currentSubtitle != null && subtitleEndTick < Environment.TickCount)
            {
                currentSubtitle = null;
            }
        }

    }
}