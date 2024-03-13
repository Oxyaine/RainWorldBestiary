namespace RainWorldBestiary
{
    internal class CreatureHooks
    {
        public static void Initialize()
        {
            On.Creature.Die += Creature_Die;
            On.Creature.Stun += Creature_Stun;

            On.Spear.LodgeInCreature += Spear_LodgeInCreature;
        }

        private static void Spear_LodgeInCreature(On.Spear.orig_LodgeInCreature orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            orig(self, result, eu);

        }

        private static void Creature_Stun(On.Creature.orig_Stun orig, Creature self, int st)
        {
            orig(self, st);
        }

        private static void Creature_Die(On.Creature.orig_Die orig, Creature self)
        {
            orig(self);

            Main.Logger.LogDebug(self.killTag.ID.ToString());
        }
    }
}
