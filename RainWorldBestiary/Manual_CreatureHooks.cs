namespace RainWorldBestiary
{
    internal class ManualCreatureHooks
    {
        public static void Initialize()
        {
            On.Fly.Grabbed += Fly_Grabbed;
            On.FlyAI.Burrowed += FlyAI_Burrowed;

            On.Fly.NextInChain += Fly_NextInChain;
        }

        private static Fly Fly_NextInChain(On.Fly.orig_NextInChain original, Fly self)
        {
            Fly fly = original(self);

            if (!AutoCreatureHooks.IgnoreID(self.abstractCreature.ID.number, 60))
                Bestiary.AddOrIncreaseToken(self, UnlockTokenType.ObserveBehaviour, alwaysAddToken: false, "Stacking");

            return fly;
        }
        private static void FlyAI_Burrowed(On.FlyAI.orig_Burrowed original, FlyAI self)
        {
            original(self);

            if (AutoCreatureHooks.IsPositionOnCamera(self.FlyPos))
            {
                Bestiary.AddOrIncreaseToken(CreatureTemplate.Type.Fly.value, UnlockTokenType.ObserveHiding);
            }
        }
        private static void Fly_Grabbed(On.Fly.orig_Grabbed original, Fly self, Creature.Grasp grasp)
        {
            original(self, grasp);

            if (grasp.grabber is Player player)
            {
                bool holdingFlyLure = false;
                foreach (Creature.Grasp v in player.grasps)
                    if (v != null && v.grabbed is PlayerCarryableItem item && item.abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.FlyLure)
                    {
                        holdingFlyLure = true;
                        break;
                    }

                if (holdingFlyLure)
                    Bestiary.AddOrIncreaseToken(self, UnlockTokenType.ObserveAttraction);
            }
        }
    }
}
