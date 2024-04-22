using RainWorldBestiary.Plugins;
using RainWorldBestiary.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary.Hooks
{
    /// <summary>
    /// A couple utilities that may help with tracking creatures for unlocks
    /// </summary>
    public static class HooksUtilities
    {
        internal static bool RainWorldGameCtorWorked = true;
        private static RainWorldGame game;
        private static AbstractRoom CurrentPlayerRoom;

        private static bool Initialized = false;
        internal static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;

                try
                {
                    On.RainWorldGame.ctor += RainWorldGame_ctor;
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
                    RainWorldGameCtorWorked = false;
                }
            }
        }

        private static void RainWorldGame_ctor(On.RainWorldGame.orig_ctor original, RainWorldGame self, ProcessManager manager)
        {
            original(self, manager);
            game = self;
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


        private class IgnoreIDTimer : IEquatable<IgnoreIDTimer>, IEquatable<int>
        {
            public int ID;
            public UnlockTokenType TypeToIgnore;
            public float TimeSeconds;

            public IgnoreIDTimer(int id, UnlockTokenType typeToIgnore, float timeSeconds)
            {
                ID = id;
                TimeSeconds = timeSeconds;
                TypeToIgnore = typeToIgnore;
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
        public static bool IgnoreID(int id, UnlockTokenType typeToIgnore, float timeSeconds = -50, bool addIfNotFound = true, bool overridePreviousTime = false)
        {
            foreach (IgnoreIDTimer ignoredID in IgnoredCreatureIDs)
                if (ignoredID.Equals(id))
                {
                    if (overridePreviousTime)
                        ignoredID.TimeSeconds = timeSeconds;
                    return true;
                }

            if (addIfNotFound)
                IgnoredCreatureIDs.Add(new IgnoreIDTimer(id, typeToIgnore, timeSeconds));
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

                if (HooksUtilities.IsCreatureOnCamera(TrackCreaturesForObserved[i]))
                {
                    if (!IgnoreID(TrackCreaturesForObserved[i].ID.number, UnlockTokenType.Observed, 60))
                    {
                        Bestiary.AddOrIncreaseToken(TrackCreaturesForObserved[i], UnlockTokenType.Observed);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the given creature is currently visible on screen
        /// </summary>
        public static bool IsCreatureOnCamera(Creature creature)
            => creature != null && IsCreatureOnCamera(creature.abstractCreature);
        /// <summary>
        /// Checks if the given creature is currently visible on screen
        /// </summary>
        public static bool IsCreatureOnCamera(AbstractCreature creature)
        {
            if (game == null || creature == null)
                return false;

            foreach (RoomCamera camera in game.cameras)
                if (camera != null && camera.PositionCurrentlyVisible(creature.pos.ToVector2(), 1f, false))
                    return true;

            return false;
        }
        /// <summary>
        /// Checks if the given position is currently visible on screen
        /// </summary>
        public static bool IsPositionOnCamera(Vector2 position)
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
