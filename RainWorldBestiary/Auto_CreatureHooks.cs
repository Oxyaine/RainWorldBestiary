using System;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal static class AutoCreatureHooks
    {
        private static RainWorldGame game;
        private static AbstractRoom CurrentPlayerRoom;

        private class IgnoreIDTimer : IEquatable<IgnoreIDTimer>, IEquatable<int>
        {
            public int ID;
            public float TimeSeconds;

            public IgnoreIDTimer(int id, float timeSeconds)
            {
                ID = id;
                TimeSeconds = timeSeconds;
            }

            public bool Equals(int other) => ID.Equals(other);
            public bool Equals(IgnoreIDTimer other) => ID.Equals(other.ID);
        }

        static readonly List<IgnoreIDTimer> IgnoredCreatureIDs = new List<IgnoreIDTimer>();
        /// <summary>
        /// Checks if the ID is ignored, if its not, it will be added
        /// </summary>
        /// <remarks>
        /// <paramref name="timeSeconds"/>: The time the id should be ignored for when its added
        /// </remarks>
        /// <returns>True if the creature should be ignored, otherwise false</returns>
        internal static bool IgnoreID(int id, float timeSeconds = -50, bool addIfNotFound = true, bool overridePreviousTime = false)
        {
            foreach (IgnoreIDTimer ignoredID in IgnoredCreatureIDs)
                if (ignoredID.Equals(id))
                {
                    if (overridePreviousTime)
                        ignoredID.TimeSeconds = timeSeconds;
                    return true;
                }

            if (addIfNotFound)
                IgnoredCreatureIDs.Add(new IgnoreIDTimer(id, timeSeconds));
            return false;
        }
        private static void TickIgnoreTimers()
        {
            for (int i = 0; i < IgnoredCreatureIDs.Count; ++i)
            {
                if (IgnoredCreatureIDs[i].TimeSeconds > -45f)
                {
                    IgnoredCreatureIDs[i].TimeSeconds -= CheckTimer;

                    if (IgnoredCreatureIDs[i].TimeSeconds <= 0f)
                        IgnoredCreatureIDs.RemoveAt(i);
                }
            }
        }

        static float CheckTimer = 0;
        internal static void Update()
        {
            CheckTimer += Time.deltaTime;

            if (CheckTimer >= 1f)
            {
                TickIgnoreTimers();
                ObserveCreatures();

                CheckTimer = 0f;
            }
        }

        private static readonly List<AbstractCreature> TrackCreaturesForObserved = new List<AbstractCreature>();
        private static void ObserveCreatures()
        {
            for (int i = 0; i < TrackCreaturesForObserved.Count; i++)
            {
                if (TrackCreaturesForObserved[i] == null)
                {
                    TrackCreaturesForObserved.RemoveAt(i);
                    --i;
                    continue;
                }

                if (IsCreatureOnCamera(TrackCreaturesForObserved[i]))
                {
                    if (!IgnoreID(TrackCreaturesForObserved[i].ID.number, 60))
                    {
                        Bestiary.AddOrIncreaseToken(TrackCreaturesForObserved[i], UnlockTokenType.Observed);
                    }
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
                ErrorManager.AddError("Players Eating Corpses", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
            }
            try
            {
                On.Player.ObjectEaten += Player_ObjectEaten;
            }
            catch
            {
                ErrorManager.AddError("Players Eating Creatures (Not Corpses)", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
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

            try
            {
                On.RainWorldGame.ctor += RainWorldGame_ctor;

                try
                {
                    On.Creature.Grab += Creature_Grab;
                }
                catch
                {
                    ErrorManager.AddError("Observing creatures fighting, attacking, and eating", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
                }
                try
                {
                    On.Creature.SpitOutOfShortCut += Creature_SpitOutOfShortCut;
                    On.Player.SpitOutOfShortCut += Player_SpitOutOfShortCut;
                }
                catch
                {
                    ErrorManager.AddError("Observing Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
                }
                try
                {

                    On.Creature.FlyIntoRoom += Creature_FlyIntoRoom;
                    On.Creature.FlyAwayFromRoom += Creature_FlyAwayFromRoom;
                }
                catch
                {
                    ErrorManager.AddError("Observing Flying Creatures", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
                }
            }
            catch
            {
                ErrorManager.AddError("Anything to do with observing creatures, such as seeing them, and watching them fighting, attacking, eating, etc", ErrorCategory.CreatureHookFailed, ErrorLevel.High);
            }
        }

        private static void Creature_FlyAwayFromRoom(On.Creature.orig_FlyAwayFromRoom original, Creature self, bool carriedByOther)
        {
            original(self, carriedByOther);

            TrackCreaturesForObserved.Remove(self.abstractCreature);
        }
        private static void Creature_FlyIntoRoom(On.Creature.orig_FlyIntoRoom original, Creature self, WorldCoordinate entrancePos, Room newRoom)
        {
            original(self, entrancePos, newRoom);

            if (CurrentPlayerRoom == newRoom.abstractRoom)
                TrackCreaturesForObserved.Add(self.abstractCreature);
            else
                TrackCreaturesForObserved.Remove(self.abstractCreature);
        }
        private static void Player_ObjectEaten(On.Player.orig_ObjectEaten original, Player self, IPlayerEdible edible)
        {
            original(self, edible);

            if (edible is Creature cr)
                Bestiary.AddOrIncreaseToken(cr, UnlockTokenType.Eaten);
        }
        private static void Player_SpitOutOfShortCut(On.Player.orig_SpitOutOfShortCut original, Player self, RWCustom.IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            original(self, pos, newRoom, spitOutAllSticks);

            CurrentPlayerRoom = newRoom.abstractRoom;
            TrackCreaturesForObserved.Clear();
            TrackCreaturesForObserved.AddRange(newRoom.abstractRoom.creatures);
        }
        private static void Creature_SpitOutOfShortCut(On.Creature.orig_SpitOutOfShortCut original, Creature self, RWCustom.IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            original(self, pos, newRoom, spitOutAllSticks);

            if (CurrentPlayerRoom == newRoom.abstractRoom)
                TrackCreaturesForObserved.Add(self.abstractCreature);
            else
                TrackCreaturesForObserved.Remove(self.abstractCreature);
        }
        private static bool Creature_Grab(On.Creature.orig_Grab original, Creature self, PhysicalObject obj, int graspUsed, int chunkGrabbed, Creature.Grasp.Shareability shareable, float dominance, bool overrideEquallyDominant, bool pacifying)
        {
            bool grabbed = original(self, obj, graspUsed, chunkGrabbed, shareable, dominance, overrideEquallyDominant, pacifying);

            if (grabbed && obj is Creature grabbedCreature && !Bestiary.GetCreatureUnlockName(grabbedCreature).Equals(SlugcatUnlockName))
            {
                if (IsCreatureOnCamera(self))
                {
                    CreatureTemplate.Relationship.Type relation = self.abstractCreature.creatureTemplate.CreatureRelationship(grabbedCreature).type;

                    if (relation.Equals(CreatureTemplate.Relationship.Type.Eats))
                    {
                        Bestiary.AddOrIncreaseToken(self, UnlockTokenType.ObserveFood, alwaysAddToken: false, Bestiary.GetCreatureUnlockName(grabbedCreature));
                    }
                    else if (relation.Equals(CreatureTemplate.Relationship.Type.Attacks))
                    {
                        Bestiary.AddOrIncreaseToken(self, UnlockTokenType.ObserveAttacking, alwaysAddToken: false, Bestiary.GetCreatureUnlockName(grabbedCreature));
                    }
                    else if (relation.Equals(CreatureTemplate.Relationship.Type.AgressiveRival))
                    {
                        Bestiary.AddOrIncreaseToken(self, UnlockTokenType.ObserveRivals, alwaysAddToken: false, Bestiary.GetCreatureUnlockName(grabbedCreature));
                    }
                    else
                    {
                        Main.Logger.LogWarning("Creature Relation Not Tracked: " + relation.value);
                    }

                    Bestiary.AddOrIncreaseToken(self, UnlockTokenType.ObserveGrabbing);
                }
            }

            return grabbed;
        }
        private static void RainWorldGame_ctor(On.RainWorldGame.orig_ctor original, RainWorldGame self, ProcessManager manager)
        {
            original(self, manager);
            game = self;
        }
        private static bool Rock_HitSomething(On.Rock.orig_HitSomething original, Rock self, SharedPhysics.CollisionResult result, bool eu)
        {
            bool b = original(self, result, eu);

            if (result.obj is Creature cr)
                if (Bestiary.GetCreatureUnlockName(self.thrownBy).Equals(SlugcatUnlockName))
                    Bestiary.AddOrIncreaseToken(cr, UnlockTokenType.Stunned);

            return b;
        }
        private static void Player_SlugcatGrab(On.Player.orig_SlugcatGrab original, Player self, PhysicalObject obj, int graspUsed)
        {
            original(self, obj, graspUsed);

            if (obj is Creature cr)
            {
                string tag = cr.dead ? "Dead" : "Alive";
                Bestiary.AddOrIncreaseToken(cr, UnlockTokenType.PlayerGrabbed, alwaysAddToken: false, tag);
            }
        }
        private static void Creature_HeardNoise(On.Creature.orig_HeardNoise original, Creature self, Noise.InGameNoise noise)
        {
            original(self, noise);

            if (noise.sourceObject is Player)
                Bestiary.AddOrIncreaseToken(self, UnlockTokenType.HeardPlayer);
        }
        private static bool Player_CanEatMeat(On.Player.orig_CanEatMeat original, Player self, Creature critter)
        {
            int val = critter.abstractCreature.ID.number;
            if (!IgnoreID(val, 300))
            {
                Bestiary.AddOrIncreaseToken(Bestiary.GetCreatureUnlockName(critter), UnlockTokenType.Eaten);
            }

            return original(self, critter);
        }
        private static void Player_Grabbed(On.Player.orig_Grabbed original, Player self, Creature.Grasp grasp)
        {
            original(self, grasp);

            Bestiary.AddOrIncreaseToken(grasp.grabber, UnlockTokenType.GrabbedPlayer);
        }
        private static void Spear_LodgeInCreature_CollisionResult_bool(On.Spear.orig_LodgeInCreature_CollisionResult_bool original, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            original(self, result, eu);

            if (Bestiary.GetCreatureUnlockName(self.thrownBy).Equals(SlugcatUnlockName))
                if (result.obj is Creature cr)
                    Bestiary.AddOrIncreaseToken(cr, UnlockTokenType.Impaled);
        }
        private static void Creature_Die(On.Creature.orig_Die original, Creature self)
        {
            original(self);

            if (self.killTag != null)
                if (Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
                    Bestiary.AddOrIncreaseToken(self, UnlockTokenType.Killed);
        }
        private static void Player_Die(On.Player.orig_Die original, Player self)
        {
            original(self);

            if (self.killTag != null)
                if (!Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
                    Bestiary.AddOrIncreaseToken(self.killTag, UnlockTokenType.KilledPlayer);
        }



        internal static bool IsCreatureOnCamera(Creature creature)
            => creature != null && IsCreatureOnCamera(creature.abstractCreature);
        internal static bool IsCreatureOnCamera(AbstractCreature creature)
        {
            if (game == null || creature == null)
                return false;

            foreach (RoomCamera camera in game.cameras)
                if (camera != null && camera.PositionCurrentlyVisible(creature.pos.ToVector2(), 1f, false))
                    return true;

            return false;
        }
        internal static bool IsPositionOnCamera(Vector2 position)
        {
            if (game == null)
                return false;

            foreach (RoomCamera camera in game.cameras)
                if (camera != null && camera.PositionCurrentlyVisible(position, 1f, false))
                    return true;

            return false;
        }
    }
}
