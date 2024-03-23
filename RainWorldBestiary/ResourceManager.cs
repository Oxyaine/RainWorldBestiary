using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RainWorldBestiary
{
    internal static class ResourceManager
    {
        private static bool Initialized = false;

        static string ModDirectory = null;

        public const string BestiaryModID = "oxyaine.bestiary";

        private static readonly List<string> LoadedMods = new List<string>() { BestiaryModID };

        internal static readonly List<Font> CustomFonts = new List<Font>();
        internal static bool GetCustomFontByName(string fontName, out Font result)
        {
            foreach (Font font in CustomFonts)
            {
                if (font.FontName.Equals(fontName))
                {
                    result = font;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public const string UnlockPipUnlocked = "illustrations\\bestiary\\icons\\unlock_pip_full";
        public const string UnlockPip = "illustrations\\bestiary\\icons\\unlock_pip";

        public const string BestiaryTitle = "illustrations\\bestiary\\titles\\Bestiary_Title";

        public static IReadOnlyList<string> Characters = new List<string>()
        {
            "illustrations\\bestiary\\icons\\Char_1",
            "illustrations\\bestiary\\icons\\Char_2",
            "illustrations\\bestiary\\icons\\Char_3",
            "illustrations\\bestiary\\icons\\Char_4",
            "illustrations\\bestiary\\icons\\Char_5",
            "illustrations\\bestiary\\icons\\Char_6",
            "illustrations\\bestiary\\icons\\Char_7",
            "illustrations\\bestiary\\icons\\Char_8",
            "illustrations\\bestiary\\icons\\Char_9",
            "illustrations\\bestiary\\icons\\Char_10",
            "illustrations\\bestiary\\icons\\Char_11",
            "illustrations\\bestiary\\icons\\Char_12",
            "illustrations\\bestiary\\icons\\Char_13",
            "illustrations\\bestiary\\icons\\Char_14",
            "illustrations\\bestiary\\icons\\Char_15",
            "illustrations\\bestiary\\icons\\Char_16",
            "illustrations\\bestiary\\icons\\Char_17",
            "illustrations\\bestiary\\icons\\Char_18",
            "illustrations\\bestiary\\icons\\Char_19",
            "illustrations\\bestiary\\icons\\Char_20",
            "illustrations\\bestiary\\icons\\Char_21",
            "illustrations\\bestiary\\icons\\Char_22",
            "illustrations\\bestiary\\icons\\Char_23",
            "illustrations\\bestiary\\icons\\Char_24",
            "illustrations\\bestiary\\icons\\Char_25",
            "illustrations\\bestiary\\icons\\Char_26",
            "illustrations\\bestiary\\icons\\Char_27"
        };

        internal static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;

                ModDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                int removeLength = ModDirectory.Length + 1;

                string illustrationsPath = Path.Combine(ModDirectory, "illustrations");
                string[] images = Directory.GetFiles(illustrationsPath, "*.png", SearchOption.AllDirectories);
                foreach (string image in images)
                {
                    string tmp = image.Substring(removeLength);
                    Futile.atlasManager.LoadImage(tmp.Substring(0, tmp.Length - 4));
                }

                IEnumerable<string> fonts = Directory.GetDirectories(Path.Combine(ModDirectory, "fonts"), "*", SearchOption.TopDirectoryOnly);
                foreach (string font in fonts)
                {
                    string configFile = font + ".txt";
                    if (File.Exists(configFile))
                    {
                        IEnumerable<string> files = Directory.GetFiles(font, "*", SearchOption.AllDirectories);
                        foreach (string file in files)
                        {
                            string tmp = file.Substring(removeLength);
                            Futile.atlasManager.LoadImage(tmp.Substring(0, tmp.Length - 4));
                        }

                        CustomFonts.Add(new Font(Path.GetFileName(font), configFile));
                    }
                }

                //AssetManager.ResolveDirectory()

                BestiaryEvents.Trigger_BeforeEntriesLoaded();

                CheckFolder(Path.Combine(ModDirectory, EntriesLocalPath), BestiaryModID);
                CheckForUnregisteredEntries();

                BestiaryEvents.Trigger_AfterEntriesLoaded();
            }
        }

        const string EntriesLocalPath = "bestiary";

        internal static IEnumerator UnloadMods(ModManager.Mod[] disabledMods)
        {
            List<string> IDs = new List<string>();
            foreach (ModManager.Mod mod in disabledMods)
            {
                IDs.Add(mod.id);
                LoadedMods.Remove(mod.id);
            }

            for (short t = 0; t < Bestiary.EntriesTabs.Count; ++t)
            {
                if (Bestiary.EntriesTabs[t].ContributingMods.ContainsAny(IDs))
                {
                    for (short e = 0; e < Bestiary.EntriesTabs[t].Count; ++e)
                    {
                        if (IDs.Contains(Bestiary.EntriesTabs[t][e].OwningModID))
                        {
                            Bestiary.EntriesTabs[t].RemoveAt(e);
                            --e;
                        }

                        yield return null;
                    }
                }
            }
        }
        internal static IEnumerator LoadMods(ModManager.Mod[] enabledMods)
        {
            foreach (ModManager.Mod mod in enabledMods)
            {
                if (!LoadedMods.Contains(mod.id))
                {
                    string path = Path.Combine(mod.path, EntriesLocalPath);
                    if (Directory.Exists(path))
                    {
                        CheckFolder(path, mod.id);
                        yield return null;
                    }
                    LoadedMods.Add(mod.id);
                }
            }
        }

        internal static void CheckForUnregisteredEntries()
        {
            foreach (ModManager.Mod mod in ModManager.ActiveMods)
            {
                if (mod.enabled && !LoadedMods.Contains(mod.id))
                {
                    string path = Path.Combine(mod.path, EntriesLocalPath);
                    if (Directory.Exists(path))
                    {
                        CheckFolder(path, mod.id);
                    }
                    LoadedMods.Add(mod.id);
                }
            }
        }
        private static void CheckFolder(string path, string modID)
        {
            string[] folders = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (string folder in folders)
            {
                EntriesTab tab;

                if (File.Exists(folder + ".json"))
                {
                    try
                    {
                        tab = JsonConvert.DeserializeObject<EntriesTab>(File.ReadAllText(folder + ".json"));
                    }
                    catch (Exception ex)
                    {
                        tab = new EntriesTab(Path.GetFileName(folder));
                        Main.Logger.LogWarning("Something went wrong trying to parse " + folder + ".json, creating the tab as a default tab using the folders name.");
                        Main.Logger.LogError(ex.Message);
                    }
                }
                else
                {
                    tab = new EntriesTab(Path.GetFileName(folder));
                }

                string[] files = Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories);
                Entry[] entries = GetFilesAsEntries(files, modID);
                tab.AddRange(entries);

                Bestiary.EntriesTabs.Add(tab, true);

                Main.StartCoroutinePtr(CheckEntryUnlocks(entries));
            }
        }
        private static Entry[] GetFilesAsEntries(string[] files, string owningModID)
        {
            Entry[] entries = new Entry[files.Count()];
            int i = 0;
            foreach (string file in files)
            {
                try
                {
                    entries[i] = new Entry(Path.GetFileNameWithoutExtension(file), JsonConvert.DeserializeObject<EntryInfo>(File.ReadAllText(file)), owningModID);
                }
                catch (Exception ex)
                {
                    Entry er = Entry.Error;
                    er.Info.Description.Add(new DescriptionModule("Entry: " + Path.GetFileNameWithoutExtension(file) + "\n" + ex.Message) { ModuleUnlockedCondition = e => false });
                    entries[i] = er;
                    Main.Logger.LogWarning("Something went wrong trying to parse entry " + Path.GetFileNameWithoutExtension(file) + " at " + file);
                    Main.Logger.LogError(ex);
                }

                ++i;
            }

            return entries;
        }

        static IEnumerator CheckEntryUnlocks(Entry[] entries)
        {
            foreach (Entry entry in entries)
            {
                List<UnlockToken> tokens = new List<UnlockToken>();
                bool added = false;

                foreach (DescriptionModule v in entry.Info.Description)
                {
                    if (v.UnlockIDs.Length == 0)
                    {
                        Bestiary.CreatureUnlockIDsOverride.Add(entry.Info.UnlockID);
                        continue;
                    }

                    bool add = false;
                    for (int i = 0; i < v.UnlockIDs.Length; i++)
                    {
                        if (!added && v.UnlockIDs[i] == null)
                            add = true;
                        else if (!tokens.Contains(v.UnlockIDs[i]))
                            tokens.Add(v.UnlockIDs[i]);
                    }

                    if (added = add)
                        Bestiary.CreatureUnlockIDsOverride.Add(entry.Info.UnlockID);

                    yield return null;
                }

                if (!Bestiary._allUniqueUnlockTokens.ContainsKey(entry.Info.UnlockID))
                    Bestiary._allUniqueUnlockTokens.Add(entry.Info.UnlockID, tokens);

                yield return null;
            }
        }
    }
}
