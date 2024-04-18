using RainWorldBestiary.Plugins;
using RainWorldBestiary.Types;

namespace RainWorldBestiary.Hooks
{
    internal static class AutoCreatureHooks
    {
        public const string SlugcatUnlockName = "Slugcat";

        private static bool Initialized = false;
        public static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;
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

                if (HooksUtilities.RainWorldGameCtorWorked)
                {
                    try
                    {
                        On.Creature.Grab += Creature_Grab;
                    }
                    catch
                    {
                        ErrorManager.AddError("Observing creatures fighting, attacking, and eating", ErrorCategory.CreatureHookFailed, ErrorLevel.Medium);
                    }
                }
            }
        }

        private static void Player_ObjectEaten(On.Player.orig_ObjectEaten original, Player self, IPlayerEdible edible)
        {
            original(self, edible);

            if (edible is Creature cr)
                Bestiary.AddOrIncreaseToken(cr, UnlockTokenType.Eaten);
        }
        private static bool Creature_Grab(On.Creature.orig_Grab original, Creature self, PhysicalObject obj, int graspUsed, int chunkGrabbed, Creature.Grasp.Shareability shareable, float dominance, bool overrideEquallyDominant, bool pacifying)
        {
            bool grabbed = original(self, obj, graspUsed, chunkGrabbed, shareable, dominance, overrideEquallyDominant, pacifying);

            if (grabbed && obj is Creature grabbedCreature && !Bestiary.GetCreatureUnlockName(grabbedCreature).Equals(SlugcatUnlockName))
            {
                if (HooksUtilities.IsCreatureOnCamera(self))
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

                    Bestiary.AddOrIncreaseToken(self, UnlockTokenType.ObserveGrabbing, alwaysAddToken: false, Bestiary.GetCreatureUnlockName(grabbedCreature));
                }
            }

            return grabbed;
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
            if (!HooksUtilities.IgnoreID(val, UnlockTokenType.Eaten, 300))
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
    }
}
