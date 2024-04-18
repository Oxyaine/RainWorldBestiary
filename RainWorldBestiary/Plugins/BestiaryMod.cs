using RainWorldBestiary.Types;

namespace RainWorldBestiary.Plugins
{
    internal class BestiaryMod
    {
        public readonly string ID = string.Empty;
        public BestiaryPlugin[] Plugins = new BestiaryPlugin[0];
        public EntriesTabList Tabs = null;

        public BestiaryMod(string id) => ID = id;
        public BestiaryMod(string id, BestiaryPlugin[] plugins) : this(id)
        {
            Plugins = plugins;
        }
        public BestiaryMod(string id, EntriesTabList entries, BestiaryPlugin[] plugins) : this(id, plugins)
        {
            Tabs = entries;
        }
    }
}
