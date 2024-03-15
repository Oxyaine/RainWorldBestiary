namespace RainWorldBestiary
{
    internal class CreatureHooks
    {
        public const string SlugcatUnlockName = "Slugcat";
        public static void Initialize()
        {
            On.Creature.Die += Creature_Die;
            On.Spear.LodgeInCreature += Spear_LodgeInCreature;
            On.Player.Die += Player_Die;
            On.Player.Grabbed += Player_Grabbed;

            //On.Creature.Stun += Creature_Stun;

            //On.Rock.HitSomething += Rock_HitSomething;

            //On.Player.SlugcatGrab += Player_SlugcatGrab;
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

        private static void Player_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig(self, grasp);

            Bestiary.AddOrIncreaseModuleUnlock(Bestiary.GetCreatureUnlockName(grasp.grabber), AutoTokenType.GrabbedPlayer);
        }
        private static void Spear_LodgeInCreature(On.Spear.orig_LodgeInCreature orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            orig(self, result, eu);

            if (Bestiary.GetCreatureUnlockName(self.thrownBy).Equals(SlugcatUnlockName))
            {
                Main.Logger.LogDebug("Thrown By Cat");
                if (result.obj is Creature cr)
                {
                    Main.Logger.LogDebug("Hit Creature");
                    Bestiary.AddOrIncreaseModuleUnlock(Bestiary.GetCreatureUnlockName(cr), AutoTokenType.Impaled);
                }
            }
        }
        private static void Creature_Die(On.Creature.orig_Die orig, Creature self)
        {
            orig(self);

            if (self.killTag == null)
                return;

            if (Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
            {
                Bestiary.GetCreatureUnlockName(self);
                Bestiary.AddOrIncreaseModuleUnlock(Bestiary.GetCreatureUnlockName(self), AutoTokenType.Killed);
            }
        }
        private static void Player_Die(On.Player.orig_Die orig, Player self)
        {
            orig(self);

            if (self.killTag == null)
                return;

            if (!Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
                Bestiary.AddOrIncreaseModuleUnlock(Bestiary.GetCreatureUnlockName(self.killTag), AutoTokenType.KilledPlayer);
        }
    }
}
