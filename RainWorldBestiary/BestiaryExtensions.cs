using RainWorldBestiary.Plugins;
using RainWorldBestiary.Types;
using System.Linq;

namespace RainWorldBestiary
{
    public static partial class Bestiary
    {
        /// <summary>
        /// Adds an entry to the Bestiary
        /// </summary>
        /// <param name="owningModID">The ID (that is set in the `modinfo.json` file) of the mod that owns this entry</param>
        /// <param name="entry">The entry that is to be added</param>
        /// <param name="tab">The tab the entry is in</param>
        public static void AddEntry(string owningModID, Entry entry, EntriesTab tab)
        {
            if (BestiaryModManager.LoadedMods.TryGetValue(owningModID, out BestiaryMod mod))
            {
                mod.Tabs.Add(new EntriesTab(tab, entry));
                return;
            }

            BestiaryModManager.LoadedMods.Add(owningModID, new BestiaryMod(owningModID, new EntriesTabList(new EntriesTab(tab, entry)), new BestiaryPlugin[0]));
        }
        /// <summary>
        /// Adds a tab of entries to the Bestiary
        /// </summary>
        /// <param name="owningModID">The ID (that is set in the `modinfo.json` file) of the mod that owns this entry</param>
        /// <param name="tab">The tab the entry is in</param>
        public static void AddTab(string owningModID, EntriesTab tab)
        {
            if (BestiaryModManager.LoadedMods.TryGetValue(owningModID, out BestiaryMod mod))
            {
                mod.Tabs.Add(tab);
                return;
            }

            BestiaryModManager.LoadedMods.Add(owningModID, new BestiaryMod(owningModID, new EntriesTabList(tab), new BestiaryPlugin[0]));
        }
        /// <summary>
        /// Adds a plugin to the bestiary
        /// </summary>
        /// <param name="owningModID"></param>
        /// <param name="plugin"></param>
        public static void AddPlugin(string owningModID, BestiaryPlugin plugin)
        {
            if (BestiaryModManager.LoadedMods.TryGetValue(owningModID, out BestiaryMod mod))
            {
                mod.Plugins = mod.Plugins.Append(plugin).ToArray();
                BestiaryModManager.AllPlugins.Add(plugin);
                return;
            }

            BestiaryModManager.LoadedMods.Add(owningModID, new BestiaryMod(owningModID, new EntriesTabList(), plugin));
        }
    }
}
