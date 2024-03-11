using On;

namespace RainWorldBestiary
{
    internal class CreatureHooks
    {
        public static void Initialize()
        {
            return;

            On.Creature.Die += Creature_Die;
            On.Creature.Stun += Creature_Stun;
        }

        private static void Creature_Stun(On.Creature.orig_Stun orig, Creature self, int st)
        {
            orig(self, st);
        }

        private static void Creature_Die(On.Creature.orig_Die orig, Creature self)
        {
            orig(self);
        }
    }
}
