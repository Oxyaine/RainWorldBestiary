using Newtonsoft.Json;
using RainWorldBestiary.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RainWorldBestiary.Plugins
{
    internal class BestiaryModManager
    {
        private static readonly Dictionary<string, BestiaryMod> LoadedMods = new Dictionary<string, BestiaryMod>();

        private static readonly List<string> ignoredMods = new List<string>();

        internal static List<string> ActiveModsIDs = new List<string>();

        public static List<BestiaryPlugin> AllPlugins = new List<BestiaryPlugin>();
        internal static void UpdatePlugins()
        {
            foreach (BestiaryPlugin plugin in AllPlugins)
                plugin.Update();
        }
        internal static void FixedUpdatePlugins()
        {
            foreach (BestiaryPlugin plugin in AllPlugins)
                plugin.FixedUpdate();
        }

        private static bool Initialized = false;
        internal static void Initialize()
        {
            if (Initialized)
                return;

            Initialized = true;

            On.RainWorld.OnModsEnabled += RainWorld_OnModsEnabled;
            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
            On.ModManager.ModFolderHasDLLContent += ModManager_ModFolderHasDLLContent;

            foreach (ModManager.Mod mod in ModManager.ActiveMods)
                ActiveModsIDs.Add(mod.id);

            string ModDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            LoadMod(ModDirectory, Main.MODID);

            foreach (ModManager.Mod mod in ModManager.ActiveMods)
            {
                if (!LoadedMods.ContainsKey(mod.id))
                {
                    LoadMod(mod);
                }
            }

            RecacheBestiaryEntries();
        }
        private static bool ModManager_ModFolderHasDLLContent(On.ModManager.orig_ModFolderHasDLLContent original, string modFolder)
            => original(modFolder)
            || Directory.Exists(Path.Combine(modFolder, Path.Combine("bestiary", "plugins")))
            || Directory.Exists(Path.Combine(modFolder, Path.Combine("bestiary", "patchers")));

        private static void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled original, RainWorld self, ModManager.Mod[] newlyDisabledMods)
        {
            original(self, newlyDisabledMods);

            bool recache = false;
            foreach (ModManager.Mod mod in newlyDisabledMods)
            {
                ActiveModsIDs.Remove(mod.id);
                if (LoadedMods.Remove(mod.id))
                {
                    recache = true;
                }
            }

            if (recache)
            {
                RecacheBestiaryEntries();
            }
        }
        private static void RainWorld_OnModsEnabled(On.RainWorld.orig_OnModsEnabled original, RainWorld self, ModManager.Mod[] newlyEnabledMods)
        {
            original(self, newlyEnabledMods);

            foreach (ModManager.Mod mod in newlyEnabledMods)
            {
                if (!ignoredMods.Contains(mod.id))
                {
                    ActiveModsIDs.Add(mod.id);
                    LoadMod(mod);
                }
            }
        }

        public static void LoadMod(ModManager.Mod mod) => LoadMod(mod.path, mod.id);
        public static void LoadMod(string modPath, string modID)
        {
            string tabsPath = Path.Combine(modPath, "bestiary");

            if (!Directory.Exists(tabsPath))
            {
                ignoredMods.Add(modID);
                return;
            }

            BestiaryMod mod = new BestiaryMod(modID, CheckForDLLs(tabsPath, modID));

            string[] tabFiles = Directory.GetFiles(tabsPath, "*.json", SearchOption.TopDirectoryOnly);
            foreach (string tabFile in tabFiles)
            {
                try
                {
                    EntriesTab entryTab = JsonConvert.DeserializeObject<EntriesTab>(File.ReadAllText(tabFile));

                    if (string.IsNullOrEmpty(entryTab.Name))
                        entryTab.Name = Path.GetFileNameWithoutExtension(tabFile);

                    if (!string.IsNullOrEmpty(entryTab.Path))
                    {
                        string entriesPath = Path.Combine(modPath, entryTab.Path);
                        if (Directory.Exists(entriesPath))
                        {
                            string[] files = Directory.GetFiles(entriesPath, "*.json", SearchOption.AllDirectories);
                            Entry[] entries = GetFilesAsEntries(files, modID);
                            entryTab.AddRange(entries);

                            CacheEntries(entries);
                        }
                    }

                    mod.Tabs.Add(entryTab);
                }
                catch (Exception ex)
                {
                    ErrorManager.AddError("Failed To Load Tab From File " + tabFile, ErrorLevel.Fatal);
                    Main.Logger.LogWarning("Something went wrong trying to parse " + tabFile + ", creating the tab as a default tab using the folders name.");
                    Main.Logger.LogError(ex);
                }
            }

            LoadedMods.Add(modID, mod);
            AllPlugins.AddRange(mod.Plugins);
        }
        public static BestiaryPlugin[] CheckForDLLs(string modPath, string modID)
        {
            string pluginsFolder = Path.Combine(modPath, "bestiary\\plugins");

            if (!Directory.Exists(pluginsFolder))
                return new BestiaryPlugin[0];

            List<BestiaryPlugin> Plugins = new List<BestiaryPlugin>();

            Type basePluginTypeCache = typeof(BestiaryPlugin);
            string[] pluginFiles = Directory.GetFiles(pluginsFolder, "*.dll", SearchOption.AllDirectories);
            foreach (string pluginFile in pluginFiles)
            {
                Assembly assembly = Assembly.LoadFile(pluginFile);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (basePluginTypeCache.IsAssignableFrom(type))
                    {
                        try
                        {
                            BestiaryPlugin plug = (BestiaryPlugin)Activator.CreateInstance(type);
                            plug.OwningModID = modID;
                            plug.Awake();
                            plug.Start();
                            Plugins.Add(plug);
                        }
                        catch (Exception ex)
                        {
                            ErrorManager.AddError(assembly.FullName, ErrorCategory.PluginLoadingFailed, ErrorLevel.Fatal);
                            Main.Logger.LogError("Failed to cast type " + type.Name + " to BestiaryPlugin");
                            Main.Logger.LogError(ex);
                        }
                    }
                }
            }

            return Plugins.ToArray();
        }

        private static Entry[] GetFilesAsEntries(string[] files, string owningModID)
        {
            Entry[] entries = new Entry[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    EntryInfo info = JsonConvert.DeserializeObject<EntryInfo>(File.ReadAllText(files[i]));

                    if (string.IsNullOrEmpty(info.Name))
                        entries[i] = new Entry(Path.GetFileNameWithoutExtension(files[i]), info, owningModID);
                    else
                        entries[i] = new Entry(info.Name, info, owningModID);
                }
                catch (Exception ex)
                {
                    Entry er = Entry.Error;
                    er.Info.Description.Add(new DescriptionModule("Entry: " + Path.GetFileNameWithoutExtension(files[i]) + "\n" + ex.Message) { ModuleUnlockedCondition = e => false });
                    entries[i] = er;
                    Main.Logger.LogWarning("Something went wrong trying to parse entry " + Path.GetFileNameWithoutExtension(files[i]) + " at " + files[i]);
                    Main.Logger.LogError(ex);
                }
            }
            return entries;
        }

        private static void RecacheBestiaryEntries()
        {
            EntriesTabList AllTabs = new EntriesTabList();
            foreach (BestiaryMod mod in LoadedMods.Values)
            {
                foreach (EntriesTab tab in mod.Tabs)
                {
                    AllTabs.Add(tab, true);
                }
            }

            Bestiary.EntriesTabs = AllTabs;
        }

        static void CacheEntries(Entry[] entries)
        {
            foreach (Entry entry in entries)
            {
                foreach (DescriptionModule module in entry.Info.Description)
                {
                    if (module.UnlockTokens.Length == 0)
                    {
                        Bestiary.CreatureUnlockIDsOverride.Add(entry.Info.UnlockID);
                        continue;
                    }

                    foreach (CreatureUnlockToken token in module.UnlockTokens)
                    {
                        if (Bestiary._allUniqueUnlockTokens.TryGetValue(token.CreatureID, out List<UnlockToken> tokens))
                        {
                            int index = tokens.FindIndex(v => v.Equals(token));

                            if (index == -1)
                                tokens.Add(token);
                            else if (token.Count > tokens[index].Count)
                                tokens[index] = token;
                        }
                        else Bestiary._allUniqueUnlockTokens.Add(token.CreatureID, new List<UnlockToken> { token });
                    }
                }
            }
        }
    }
}
