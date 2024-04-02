using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal static class CreatureHooks
    {
        private static RainWorldGame game;

        private class IgnoreIDTimer
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
            }
            catch
            {
                ErrorManager.AddError("Anything to do with observing creatures, such as them fighting, attacking, eating, etc", ErrorCategory.CreatureHookFailed, ErrorLevel.High);
            }


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
            if (!DoWeIgnoreID(val))
            {
                IgnoreID(val, 300);
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
            => IsCreatureOnCamera(creature.abstractCreature);
        internal static bool IsCreatureOnCamera(AbstractCreature creature)
        {
            if (game == null)
                return false;

            foreach (RoomCamera camera in game.cameras)
            {
                if (camera.PositionCurrentlyVisible(creature.pos.ToVector2(), 1f, false))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
