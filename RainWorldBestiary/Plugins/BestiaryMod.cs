using RainWorldBestiary.Types;

namespace RainWorldBestiary.Plugins
{
    internal sealed class BestiaryMod
    {
        public readonly string ID = string.Empty;
        public BestiaryPlugin[] Plugins = new BestiaryPlugin[0];
        public EntriesTabList Tabs = new EntriesTabList();

        public BestiaryMod(string id) => ID = id;
        public BestiaryMod(string id, params BestiaryPlugin[] plugins) : this(id)
        {
            Plugins = plugins;
        }
        public BestiaryMod(string id, EntriesTabList entries, params BestiaryPlugin[] plugins) : this(id, plugins)
        {
            Tabs = entries;
        }
    }
}
