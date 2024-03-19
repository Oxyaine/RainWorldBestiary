namespace RainWorldBestiary
{
    internal class CreatureHooks
    {
        public const string SlugcatUnlockName = "Slugcat";
        public static void Initialize()
        {
            On.Creature.Die += Creature_Die;
            On.Spear.LodgeInCreature_CollisionResult_bool += Spear_LodgeInCreature_CollisionResult_bool;
            On.Player.Die += Player_Die;
            On.Player.Grabbed += Player_Grabbed;

            //On.Weapon.

            //On.Creature.Stun += Creature_Stun;

            //On.Rock.HitSomething += Rock_HitSomething;

            //On.Player.SlugcatGrab += Player_SlugcatGrab;

            //On.Player.eat


            // Batflies attracted to batnip log here somehow
        }

        //private static bool Rock_HitSomething(On.Rock.orig_HitSomething orig, Rock self, SharedPhysics.CollisionResult result, bool eu)
        //{
        //    return orig(self, result, eu);
        //}

        //private static void Creature_Stun(On.Creature.orig_Stun orig, Creature self, int st)
        //{
        //    orig(self, st);

        //}

        //private static void Player_SlugcatGrab(On.Player.orig_SlugcatGrab orig, Player self, PhysicalObject obj, int graspUsed)
        //{
        //    throw new System.NotImplementedException();
        //}

        private static void Player_Grabbed(On.Player.orig_Grabbed original, Player self, Creature.Grasp grasp)
        {
            original(self, grasp);


            string creatureID = Bestiary.GetCreatureUnlockName(grasp.grabber);
            if (!Bestiary.AutoTrackIgnoredIDs.Contains(creatureID))
                Bestiary.AddOrIncreaseModuleUnlock(creatureID, UnlockTokenType.GrabbedPlayer);
        }
        private static void Spear_LodgeInCreature_CollisionResult_bool(On.Spear.orig_LodgeInCreature_CollisionResult_bool original, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            original(self, result, eu);

            if (Bestiary.GetCreatureUnlockName(self.thrownBy).Equals(SlugcatUnlockName))
                if (result.obj is Creature cr)
                {
                    string creatureID = Bestiary.GetCreatureUnlockName(cr);

                    if (!Bestiary.AutoTrackIgnoredIDs.Contains(creatureID))
                        Bestiary.AddOrIncreaseModuleUnlock(creatureID, UnlockTokenType.Impaled);
                }
        }
        private static void Creature_Die(On.Creature.orig_Die original, Creature self)
        {
            original(self);

            if (self.killTag != null)
                if (Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
                {
                    string creatureID = Bestiary.GetCreatureUnlockName(self);
                    if (!Bestiary.AutoTrackIgnoredIDs.Contains(creatureID))
                        Bestiary.AddOrIncreaseModuleUnlock(creatureID, UnlockTokenType.Killed);
                }
        }
        private static void Player_Die(On.Player.orig_Die original, Player self)
        {
            original(self);

            if (self.killTag != null)
                if (!Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
                {
                    string creatureID = Bestiary.GetCreatureUnlockName(self.killTag);
                    if (!Bestiary.AutoTrackIgnoredIDs.Contains(creatureID))
                        Bestiary.AddOrIncreaseModuleUnlock(creatureID, UnlockTokenType.KilledPlayer);
                }
        }
    }
}
