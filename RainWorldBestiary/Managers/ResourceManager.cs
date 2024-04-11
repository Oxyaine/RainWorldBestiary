using Newtonsoft.Json;
using RainWorldBestiary.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RainWorldBestiary.Managers
{
    internal static class ResourceManager
    {
        private static bool Initialized = false;

        private static readonly List<string> LoadedMods = new List<string>() { Main.MODID };

        private static void GetAllFonts(string ModDirectory)
        {
            List<Font> customFonts = new List<Font>();

            string[] fonts = Directory.GetDirectories(Path.Combine(ModDirectory, "fonts"), "*", SearchOption.TopDirectoryOnly);
            foreach (string font in fonts)
            {
                string configFile = font + "_" + Main.CurrentLanguage.value + ".txt";

                if (File.Exists(configFile))
                    customFonts.Add(new Font(Path.GetFileName(font), configFile, ModDirectory));
                else
                    customFonts.Add(new Font(Path.GetFileName(font)));
            }

            CustomFonts = customFonts.ToArray();
        }
        internal static void ReloadFonts()
        {
            foreach (Font font in CustomFonts)
                font.Dispose();
            CustomFonts = new Font[0];

            GetAllFonts(Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
        }
        private static Font[] CustomFonts = new Font[0];
        internal static Font GetCustomFontByName(string fontName)
        {
            foreach (Font font in CustomFonts)
            {
                if (font.Name.Equals(fontName))
                {
                    return font;
                }
            }

            return null;
        }
        internal static bool GetCustomFontByName(string fontName, out Font result)
        {
            Font f = GetCustomFontByName(fontName);
            result = f;

            if (f == null)
                return false;

            return true;
        }

        internal static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;

                string ModDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                GetAllSprites(ModDirectory);
                GetAllFonts(ModDirectory);

                CheckFolder(ModDirectory, Main.MODID);
                CheckForUnregisteredEntries();
            }
        }

        private static void GetAllSprites(string ModDirectory)
        {
            int removeLength = ModDirectory.Length + 1;

            string illustrationsPath = Path.Combine(ModDirectory, "illustrations\\icons");
            string illustrationsPath2 = Path.Combine(ModDirectory, "illustrations\\titles");
            string[] images = Directory.GetFiles(illustrationsPath, "*.png", SearchOption.AllDirectories).Concat(Directory.GetFiles(illustrationsPath2, "*.png", SearchOption.AllDirectories)).ToArray();
            foreach (string image in images)
            {
                string imagePath = image.Substring(removeLength);
                Futile.atlasManager.LoadImage(imagePath.Substring(0, imagePath.Length - 4));
            }
        }

        internal static int UnloadingModsEnumerator = -1;
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
        internal static int LoadingModsEnumerator = -1;
        internal static IEnumerator LoadMods(ModManager.Mod[] enabledMods)
        {
            foreach (ModManager.Mod mod in enabledMods)
            {
                if (!LoadedMods.Contains(mod.id))
                {
                    CheckFolder(mod.path, mod.id);
                    LoadedMods.Add(mod.id);

                    yield return null;
                }
            }
        }

        internal static void CheckForUnregisteredEntries()
        {
            foreach (ModManager.Mod mod in ModManager.ActiveMods)
            {
                if (mod.enabled && !LoadedMods.Contains(mod.id))
                {
                    CheckFolder(mod.path, mod.id);
                    LoadedMods.Add(mod.id);
                }
            }
        }
        private static void CheckFolder(string modPath, string modID)
        {
            string tabsPath = Path.Combine(modPath, "bestiary");
            if (!Directory.Exists(tabsPath))
                return;

            string[] tabs = Directory.GetFiles(tabsPath, "*.json", SearchOption.TopDirectoryOnly);
            foreach (string tab in tabs)
            {
                EntriesTab entryTab;
                try
                {
                    entryTab = JsonConvert.DeserializeObject<EntriesTab>(File.ReadAllText(tab));
                }
                catch (Exception ex)
                {
                    entryTab = new EntriesTab(Path.GetFileNameWithoutExtension(tab));
                    Main.Logger.LogWarning("Something went wrong trying to parse " + tab + ".json, creating the tab as a default tab using the folders name.");
                    Main.Logger.LogError(ex.Message);
                }

                if (string.IsNullOrEmpty(entryTab.Name))
                    entryTab.Name = Path.GetFileNameWithoutExtension(tab);

                if (!string.IsNullOrEmpty(entryTab.Path))
                {
                    string entriesPath = Path.Combine(modPath, entryTab.Path);
                    if (Directory.Exists(entriesPath))
                    {
                        string[] files = Directory.GetFiles(entriesPath, "*.json", SearchOption.AllDirectories);
                        Entry[] entries = GetFilesAsEntries(files, modID);
                        entryTab.AddRange(entries);

                        cachingOperations.Add(Enumerators.StartEnumerator(ScanEntriesAndCacheTokens(entries)));
                    }
                }

                Bestiary.EntriesTabs.Add(entryTab, true);
            }
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

        internal static List<int> cachingOperations = new List<int>();

        static IEnumerator ScanEntriesAndCacheTokens(Entry[] entries)
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

                    cachingOperations.Add(Enumerators.StartEnumerator(CacheTokens(v)));

                    yield return null;
                }

                yield return null;
            }
        }
        static IEnumerator CacheTokens(DescriptionModule module)
        {
            yield return null;
            foreach (CreatureUnlockToken token in module.UnlockIDs)
            {
                if (Bestiary._allUniqueUnlockTokens.TryGetValue(token.CreatureID, out List<UnlockToken> tokens))
                {
                    bool tokenExists = false;
                    int existingTokenIndex = 0;
                    for (int i = 0; i < tokens.Count; ++i)
                    {
                        if (tokens[i].Equals(token))
                        {
                            tokenExists = true;
                            existingTokenIndex = i;
                            break;
                        }
                    }

                    if (!tokenExists)
                        tokens.Add(token);
                    else if (tokens[existingTokenIndex].Count < token.Count)
                        tokens[existingTokenIndex] = token;
                }
                else Bestiary._allUniqueUnlockTokens.Add(token.CreatureID, new List<UnlockToken>() { token });

                yield return null;
            }
        }
    }
}
