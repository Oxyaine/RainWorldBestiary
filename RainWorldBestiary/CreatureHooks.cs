using UnityEngine.Assertions.Must;

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
        }

        private static void Player_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig(self, grasp);

            Bestiary._AutoModuleUnlocks.AddOrIncrease(new AutoModuleUnlockToken(Bestiary.GetCreatureUnlockName(grasp.grabber), AutoTokenType.GrabbedPlayer));
        }
        private static void Spear_LodgeInCreature(On.Spear.orig_LodgeInCreature orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            orig(self, result, eu);

            if (result.obj is Creature cr)
                Bestiary._AutoModuleUnlocks.AddOrIncrease(new AutoModuleUnlockToken(Bestiary.GetCreatureUnlockName(cr), AutoTokenType.Impaled));
        }
        private static void Creature_Die(On.Creature.orig_Die orig, Creature self)
        {
            orig(self);

            if (self.killTag == null)
                return;

            if (Bestiary.GetCreatureUnlockName(self.killTag).Equals(SlugcatUnlockName))
            {
                Bestiary.GetCreatureUnlockName(self);
                Bestiary._AutoModuleUnlocks.AddOrIncrease(new AutoModuleUnlockToken(Bestiary.GetCreatureUnlockName(self), AutoTokenType.Killed));
            }
        }
        private static void Player_Die(On.Player.orig_Die orig, Player self)
        {
            orig(self);
            Bestiary._AutoModuleUnlocks.AddOrIncrease(new AutoModuleUnlockToken(Bestiary.GetCreatureUnlockName(self.killTag), AutoTokenType.KilledPlayer));
        }
    }
}
