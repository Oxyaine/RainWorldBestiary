using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RainWorldBestiary
{
    /// <summary>
    /// The main class for the bestiary, everything should be accessible from this class
    /// </summary>
    public static class Bestiary
    {
        // The bool determining if the downpour tab should be included
        internal static bool IncludeDownpour = false;

        /// <summary>
        /// Whether downpour entries are shown or not, true if more slug cats is enabled, otherwise false
        /// </summary>
        public static bool IncludeDownpourEntries => IncludeDownpour;

        // The name of the downpour tab, used in BestiaryMenu to check if the tab is the one that should be locked if downpour is disabled
        internal const string DownpourTabName = "Downpour";


        // Force unlocks creature entries, if an entry has a module that is always visible it is automatically added to this list, besides that this list is unused
        internal readonly static List<string> CreatureUnlockIDsOverride = new List<string>();


        /// <summary>
        /// All the unlocked entries, this determines if an entry should be unlocked or not, even if a piece of the description is visible, the entry wont be visible unless its id is in this list
        /// </summary>
        public static List<string> CreatureUnlockIDs = new List<string>();

        // The module unlocks dictionary, is accessed publicly through ModuleUnlocks, since we want people to use add or increase module unlock
        private static Dictionary<string, List<UnlockToken>> _ModuleUnlocks = new Dictionary<string, List<UnlockToken>>();
        /// <summary>
        /// All the manual module unlock tokens, the first element defines the id of the creature the token belongs to, the second element is a list of all unlock tokens belonging to that creature
        /// </summary>
        /// <remarks>
        /// If you'd like to add your own token, Use <see cref="AddOrIncreaseModuleUnlock(string, UnlockTokenType, bool)"/>, as it will increase the token if it exists, otherwise adds it
        /// </remarks>
        public static Dictionary<string, List<UnlockToken>> ModuleUnlocks => new Dictionary<string, List<UnlockToken>>(_ModuleUnlocks);


        /// <summary>
        /// Checks if <see cref="ModuleUnlocks"/> contains an <see cref="UnlockTokenType"/> that belongs to the given creature, if it does, the module unlock token gets increased by 1, otherwise its added as a new element
        /// </summary>
        /// <param name="creatureID">The ID of the creature this unlock token will target</param>
        /// <param name="tokenType">The type of token</param>
        /// <param name="checkIfThisUnlocksCreature">Whether to check if this module unlocks the creature or not, if it does, the creatures ID will be added to <see cref="CreatureUnlockIDs"/>.<code></code>
        /// <see cref="CreatureUnlockIDs"/> is the list that determines whether an entry is unlocked or not, read <see cref="CreatureUnlockIDs"/>' summary for more details</param>
        /// <param name="SpecialData">All the extra data to add to the object</param>
        public static void AddOrIncreaseModuleUnlock(string creatureID, UnlockTokenType tokenType, bool checkIfThisUnlocksCreature = true, params string[] SpecialData)
        {
            if (_ModuleUnlocks.ContainsKey(creatureID))
            {
                int cache = _ModuleUnlocks[creatureID].Count;
                bool tokenExists = false;
                for (int i = 0; i < cache; i++)
                {
                    UnlockToken token = _ModuleUnlocks[creatureID][i];
                    if (token.TokenType == tokenType)
                    {
                        if (token.Count < 255)
                            ++_ModuleUnlocks[creatureID][i].Count;


                        foreach (string data in SpecialData)
                            if (!token.SpecialData.Contains(data))
                                _ModuleUnlocks[creatureID][i].SpecialData.Add(data);

                        tokenExists = true;
                    }
                }

                if (!tokenExists)
                    _ModuleUnlocks[creatureID].Add(new UnlockToken(tokenType) { SpecialData = SpecialData.ToList() });
            }
            else
                _ModuleUnlocks.Add(creatureID, new List<UnlockToken> { new UnlockToken(tokenType) { SpecialData = SpecialData.ToList() } });

            BestiaryEvents.Trigger_UnlockTokenAdded(creatureID, tokenType, checkIfThisUnlocksCreature);

            if (checkIfThisUnlocksCreature)
                Main.StartCoroutinePtr(CheckIfTokenUnlocksCreature(creatureID, tokenType));
        }
        /// <param name="creature">The creature to unlock this token for, this will automatically get run through <see cref="GetCreatureUnlockName(Creature, bool)"/> with default parameters</param>
        /// <inheritdoc cref="AddOrIncreaseModuleUnlock(string, UnlockTokenType, bool, string[])"/>
        /// <param name="tokenType"></param>
        /// <param name="checkIfThisUnlocksCreature"></param>
        /// <param name="AdditionalData"
        public static void AddOrIncreaseModuleUnlock(Creature creature, UnlockTokenType tokenType, bool checkIfThisUnlocksCreature = true, params string[] AdditionalData)
            => AddOrIncreaseModuleUnlock(GetCreatureUnlockName(creature), tokenType, checkIfThisUnlocksCreature, AdditionalData);
        /// <inheritdoc cref="AddOrIncreaseModuleUnlock(Creature, UnlockTokenType, bool, string[])"/>
        public static void AddOrIncreaseModuleUnlock(AbstractCreature creature, UnlockTokenType tokenType, bool checkIfThisUnlocksCreature = true, params string[] AdditionalData)
            => AddOrIncreaseModuleUnlock(GetCreatureUnlockName(creature), tokenType, checkIfThisUnlocksCreature, AdditionalData);

#warning EXTREMELY BAD CODE AND BAD PRACTICE, I'm Marking This As A Problem For Later
        // Checks if this token is a token that would unlock this creature, by checking if the creature has a module with an unlock token that matches this token
        private static IEnumerator CheckIfTokenUnlocksCreature(string creatureID, UnlockTokenType tokenType)
        {

            foreach (EntriesTab tab in EntriesTabs)
            {
                foreach (Entry entry in tab)
                {
                    if (entry.Info.UnlockID.Equals(creatureID))
                    {
                        IEnumerator<CreatureUnlockToken> enumerator = entry.Info.Description.GetAllUniqueUnlockTokens();

                        do
                        {
                            if (enumerator.Current.TokenType == tokenType && enumerator.Current.CreatureID.Equals(creatureID))
                            {
                                CreatureUnlockIDs.Add(creatureID);
                                yield break;
                            }

                            yield return null;
                        }
                        while (enumerator.MoveNext());
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the given token is in either AutoModuleUnlocks or ModuleUnlocks
        /// </summary>
        /// <remarks>Returns true if the count is equal to or greater than the value in the registered token<code></code>
        /// Does not take into account if <see cref="BestiarySettings.UnlockAllEntries"/> is toggled</remarks>
        public static bool IsUnlockTokenValid(CreatureUnlockToken unlockToken)
        {
            if (_ModuleUnlocks.TryGetValue(unlockToken.CreatureID, out List<UnlockToken> value))
            {
                ushort v = 0;
                foreach (UnlockToken token in value)
                    if (unlockToken.Equals(token))
                    {
                        if ((v += token.Count) > unlockToken.Count)
                            return true;
                    }
            }

            return false;
        }


        // The folder the save file is in
        internal static string SaveFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Videocult\\Rain World";
        // The save file to save to (yes, this file intentionally has no extension)
        internal static string SaveFile => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Videocult\\Rain World\\Bestiary";
        // Saves all module unlocks and creature unlocks
        internal static void Save()
        {
            if (Directory.Exists(SaveFolder))
            {
                BestiarySaveData saveData = new BestiarySaveData(_ModuleUnlocks, CreatureUnlockIDs);
                File.WriteAllText(SaveFile, JsonConvert.SerializeObject(saveData));
            }
        }

        // Cleans up things using PerformCleanupOperations
        // saveBeforehand = whether to save before running cleanup incase the game exits before cleanup is done (which would cause nothing to save)
        internal static IEnumerator CleanupAndSave(bool saveBeforehand)
        {
            if (saveBeforehand)
                Save();

            IEnumerator enumerator = PerformCleanupOperations();
            yield return null;

            while (enumerator.MoveNext())
            {
                yield return null;
            }

            Save();
        }

        // Loads all saved data into the bestiary, such as module unlock tokens and creature unlock ids
        internal static void Load()
        {
            if (File.Exists(SaveFile))
            {
                BestiarySaveData saveData = JsonConvert.DeserializeObject<BestiarySaveData>(File.ReadAllText(SaveFile));
                _ModuleUnlocks = saveData.ModuleUnlocks.ToDictionary(v => v.Key, v => v.Value);
                CreatureUnlockIDs = saveData.CreatureUnlockIDs;
            }
        }

        /// <summary>
        /// All the tabs, which hold all the entries, you can add your own, or add your entry to an existing tab
        /// </summary>
        public readonly static EntriesTabList EntriesTabs = new EntriesTabList();


        /// <summary>
        /// The tab that is currently selected
        /// </summary>
        public static EntriesTab CurrentSelectedTab { get; internal set; }
        /// <summary>
        /// The entry that is currently selected
        /// </summary>
        public static Entry CurrentSelectedEntry { get; internal set; }


        /// <inheritdoc cref="GetCreatureUnlockName(AbstractCreature, bool)"/>
        public static string GetCreatureUnlockName(Creature creature, bool useSpecialIdLogic = true) => GetCreatureUnlockName(creature.abstractCreature, useSpecialIdLogic);
        /// <summary>
        /// Gets the creatures name through <see cref="AbstractCreature.creatureTemplate"/> and removes all white space characters
        /// </summary>
        /// <param name="creature">The creature to get the ID of</param>
        /// <param name="useSpecialIdLogic">Whether to check through <see cref="SpecialIDsLogic"/> for any additional logic that should be applied</param>
        public static string GetCreatureUnlockName(AbstractCreature creature, bool useSpecialIdLogic = true)
        {
            string id = creature.creatureTemplate.name.Trim().Replace(" ", "");

            if (useSpecialIdLogic)
                foreach (KeyValuePair<string, Func<string, string>> specialLogic in SpecialIDsLogic)
                    if (specialLogic.Key.Equals(id))
                        return specialLogic.Value(id);

            return id;
        }

        /// <summary>
        /// Special logic to apply to certain IDs, for example, `CicadaA` and `CicadaB` (Squidacada's ID's) have custom logic to remove the `A` and `B` so its just `Cicada`
        /// </summary>
        public static Dictionary<string, Func<string, string>> SpecialIDsLogic = new Dictionary<string, Func<string, string>>
        {
            { "CicadaA", CicadaSpecialIDLogic },
            { "CicadaB", CicadaSpecialIDLogic }
        };

        // Removes the A or B at the end of CicadaA or CicadaB
        private static string CicadaSpecialIDLogic(string _d_) => "Cicada";


        static IEnumerator PerformCleanupOperations()
        {
            yield break;
        }
    }
}
