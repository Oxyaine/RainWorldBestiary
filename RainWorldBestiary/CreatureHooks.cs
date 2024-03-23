using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class CreatureHooks
    {
        internal static Room CurrentPlayerRoom = null;
        internal static Vector2 CurrentCameraPosition = Vector2.zero;

        class IgnoreIDTimer
        {
            public int ID;
            public float Time;

            public IgnoreIDTimer(int id, float time)
            {
                ID = id;
                Time = time;
            }
        }

        static readonly List<IgnoreIDTimer> IgnoredCreatureIDs = new List<IgnoreIDTimer>();
        private static bool DoWeIgnoreID(int id)
        {
            foreach (IgnoreIDTimer ignoredId in IgnoredCreatureIDs)
                if (ignoredId.ID.Equals(id))
                    return true;

            return false;
        }
        private static void IgnoreID(int id, float time = -50)
        {
            for (int i = 0; i < IgnoredCreatureIDs.Count; ++i)
                if (IgnoredCreatureIDs[i].ID.Equals(id))
                {
                    IgnoredCreatureIDs[i].Time = time;
                    return;
                }

            IgnoredCreatureIDs.Add(new IgnoreIDTimer(id, time));
        }

        internal static void Update()
        {
            for (int i = 0; i < IgnoredCreatureIDs.Count; ++i)
            {
                if (IgnoredCreatureIDs[i].Time > -45)
                {
                    IgnoredCreatureIDs[i].Time -= Time.deltaTime;

                    if (IgnoredCreatureIDs[i].Time <= 0)
                        IgnoredCreatureIDs.RemoveAt(i);
                }
            }
        }

        public const string SlugcatUnlockName = "Slugcat";

        public static void Initialize()
        {
            try
            {
                On.Creature.Die += Creature_Die;
            }
            catch
            {
                ErrorManager.AddError("Players Killing Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Spear.LodgeInCreature_CollisionResult_bool += Spear_LodgeInCreature_CollisionResult_bool;
            }
            catch
            {
                ErrorManager.AddError("Players Impaling Creatures With Spears", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Player.Die += Player_Die;
            }
            catch
            {
                ErrorManager.AddError("Players Dying To Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Player.Grabbed += Player_Grabbed;
            }
            catch
            {
                ErrorManager.AddError("Players Getting Grabbed By Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Player.CanEatMeat += Player_CanEatMeat;
            }
            catch
            {
                ErrorManager.AddError("Players Eating Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Creature.HeardNoise += Creature_HeardNoise;
            }
            catch
            {
                ErrorManager.AddError("Creatures Hearing Players Noises", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Player.SlugcatGrab += Player_SlugcatGrab;
            }
            catch
            {
                ErrorManager.AddError("Slugcat Grabbing Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Rock.HitSomething += Rock_HitSomething;
            }
            catch
            {
                ErrorManager.AddError("Slugcat Stunning Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }

#if DEBUG

            On.Player.SpitOutOfShortCut += Player_SpitOutOfShortCut;
            On.Creature.SpitOutOfShortCut += Creature_SpitOutOfShortCut;

            On.RoomCamera.ApplyPositionChange += RoomCamera_ApplyPositionChange;
#endif

            // Batflies attracted to batnip log here somehow
        }

#if DEBUG

        private static void RoomCamera_ApplyPositionChange(On.RoomCamera.orig_ApplyPositionChange original, RoomCamera self)
        {
            CurrentCameraPosition = self.pos;
            original(self);

            //Main.Logger.LogDebug(CurrentCameraPosition);
        }

        private static void Player_SpitOutOfShortCut(On.Player.orig_SpitOutOfShortCut original, Player self, RWCustom.IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            CurrentPlayerRoom = newRoom;
            original(self, pos, newRoom, spitOutAllSticks);
        }

        private static void Creature_SpitOutOfShortCut(On.Creature.orig_SpitOutOfShortCut original, Creature self, RWCustom.IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            original(self, pos, newRoom, spitOutAllSticks);


            // add logic so stuff is only saved when creature is on camera
            //if (CurrentPlayerRoom != null)
            //    if (!Bestiary.GetCreatureUnlockName(self).Equals(SlugcatUnlockName))
            //        if (CurrentPlayerRoom.Equals(newRoom))
            //        {
            //        }
        }
#endif

        private static bool Rock_HitSomething(On.Rock.orig_HitSomething original, Rock self, SharedPhysics.CollisionResult result, bool eu)
        {
            bool b = original(self, result, eu);

            if (result.obj is Creature cr)
                if (Bestiary.GetCreatureUnlockName(self.thrownBy).Equals(SlugcatUnlockName))
                    Bestiary.AddOrIncreaseModuleUnlock(cr, UnlockTokenType.Stunned);

            return b;
        }
        private static void Player_SlugcatGrab(On.Player.orig_SlugcatGrab original, Player self, PhysicalObject obj, int graspUsed)
        {
            original(self, obj, graspUsed);

            if (obj is Creature cr)
            {
                string tag = cr.dead ? "Dead" : "Alive";
                Bestiary.AddOrIncreaseModuleUnlock(cr, UnlockTokenType.PlayerGrabbed, checkIfCreatureShouldBeUnlocked: true, AdditionalData: tag);
            }
        }
        private static void Creature_HeardNoise(On.Creature.orig_HeardNoise original, Creature self, Noise.InGameNoise noise)
        {
            original(self, noise);

            if (noise.sourceObject is Player)
                Bestiary.AddOrIncreaseModuleUnlock(self, UnlockTokenType.HeardPlayer);
        }
        private static bool Player_CanEatMeat(On.Player.orig_CanEatMeat original, Player self, Creature critter)
        {
            int val = critter.abstractCreature.ID.number;
            if (!DoWeIgnoreID(val))
            {
                IgnoreID(val, 300);
                Bestiary.AddOrIncreaseModuleUnlock(Bestiary.GetCreatureUnlockName(critter), UnlockTokenType.Eaten);
            }

            return original(self, critter);
        }
        private static void Player_Grabbed(On.Player.orig_Grabbed original, Player self, Creature.Grasp grasp)
        {
            original(self, grasp);

            Bestiary.AddOrIncreaseModuleUnlock(grasp.grabber, UnlockTokenType.GrabbedPlayer);
        }
        private static void Spear_LodgeInCreature_CollisionResult_bool(On.Spear.orig_LodgeInCreature_CollisionResult_bool original, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            original(self, result, eu);

            if (Bestiary.GetCreatureUnlockName(self.thrownBy).Equals(SlugcatUnlockName))
                if (result.obj is Creature cr)
                    Bestiary.AddOrIncreaseModuleUnlock(cr, UnlockTokenType.Impaled);
        }
        private static void Creature_Die(On.Creature.orig_Die original, Creature self)
        {
            original(self);

            if (self.killTag != null)
                if (Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
                    Bestiary.AddOrIncreaseModuleUnlock(self, UnlockTokenType.Killed);
        }
        private static void Player_Die(On.Player.orig_Die original, Player self)
        {
            original(self);

            if (self.killTag != null)
                if (!Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
                    Bestiary.AddOrIncreaseModuleUnlock(self.killTag, UnlockTokenType.KilledPlayer);
        }
    }
}
