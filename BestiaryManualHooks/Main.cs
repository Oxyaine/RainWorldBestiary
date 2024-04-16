using RainWorldBestiary.Plugins;

namespace BestiaryManualHooks
{
    internal class Main : BestiaryPlugin
    {
        public override void Awake()
        {
            ManualCreatureHooks.Initialize();
        }
    }
}
