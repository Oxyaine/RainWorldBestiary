namespace RainWorldBestiary
{
    internal class CreatureHooks
    {
        public static void Initialize()
        {
            return;

            On.Creature.Die += Creature_Die;
        }

        private static void Creature_Die(On.Creature.orig_Die orig, Creature self)
        {
            orig(self);
        }
    }
}
