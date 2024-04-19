using Newtonsoft.Json;
using RainWorldBestiary.Menus;
using RainWorldBestiary.Types;
using System;
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
        // Force unlocks creature entries, if an entry has a module that is always visible it is automatically added to this list, besides that this list is unused
        internal readonly static List<string> CreatureUnlockIDsOverride = new List<string>();

        // The creature unlock ids list, is publicly accessible through CreatureUnlockIDs
        private static List<string> _CreatureUnlockIDs = new List<string>();
        /// <summary>
        /// All the unlocked entries, this determines if an entry should be unlocked or not, even if a piece of the description is visible, the entry wont be visible unless its id is in this list
        /// </summary>
        /// <remarks>To unlock your own creature, use <see cref="UnlockCreature(string)"/></remarks>
        public static List<string> CreatureUnlockIDs => new List<string>(_CreatureUnlockIDs);

        /// <summary>
        /// Adds this creatureID to the <see cref="_CreatureUnlockIDs"/> list if its not already added
        /// </summary>
        public static void UnlockCreature(string creatureID)
        {
            if (!_CreatureUnlockIDs.Contains(creatureID))
                _CreatureUnlockIDs.Add(creatureID);
        }
        /// <param name="creature">Automatically gets run through <see cref="GetCreatureUnlockName(Creature, bool)"/></param>
        /// <inheritdoc cref="UnlockCreature(string)"/>
        public static void UnlockCreature(Creature creature) => UnlockCreature(GetCreatureUnlockName(creature));
        /// <inheritdoc cref="UnlockCreature(Creature)"/>
        public static void UnlockCreature(AbstractCreature creature) => UnlockCreature(GetCreatureUnlockName(creature));

        /// <summary>
        /// Checks if this creature is found in either the <see cref="_CreatureUnlockIDs"/> or <see cref="CreatureUnlockIDsOverride"/>
        /// </summary>
        public static bool IsCreatureUnlocked(string creatureId) => _CreatureUnlockIDs.Contains(creatureId) || CreatureUnlockIDsOverride.Contains(creatureId);



        // The module unlocks dictionary, is accessed publicly through ModuleUnlocks, since we want people to use add or increase module unlock
        private static Dictionary<string, List<UnlockToken>> _ModuleUnlocks = new Dictionary<string, List<UnlockToken>>();
        /// <summary>
        /// All the manual module unlock tokens, the first element defines the id of the creature the token belongs to, the second element is a list of all unlock tokens belonging to that creature
        /// </summary>
        /// <remarks>
        /// If you'd like to add your own token, Use <see cref="AddOrIncreaseToken(string, UnlockTokenType, bool, string[])"/>
        /// <code></code>
        /// You should also check out <see cref="IsUnlockTokenValid(CreatureUnlockToken)"/> for checking if the unlock token is valid
        /// </remarks>
        public static Dictionary<string, List<UnlockToken>> ModuleUnlocks => new Dictionary<string, List<UnlockToken>>(_ModuleUnlocks);

        // Pre cached in the resource manager while it checks if an entry should be unlocked
        // The string is the creatures unlock id, and the list is all unique unlocks tokens for that entry
        internal static readonly Dictionary<string, List<UnlockToken>> _allUniqueUnlockTokens = new Dictionary<string, List<UnlockToken>>();
        /// <summary>
        /// Checks if <see cref="ModuleUnlocks"/> contains the <see cref="UnlockTokenType"/> for the given creature ID, if it does, it increases the token, otherwise adds it as a new token
        /// </summary>
        /// <remarks>
        /// The token will only get added, if the token takes part in unlocking a description module, so if the token will never unlock a module, it wont get added. You can override this by setting <paramref name="alwaysAddToken"/> to true.
        /// </remarks>
        /// <param name="creatureID">The ID of the creature this token is for</param>
        /// <param name="tokenType">The type of token</param>
        /// <param name="alwaysAddToken">Whether to always add this token, regardless of whether this token will be used</param>
        /// <param name="SpecialData">The special data to add onto the token</param>
        public static void AddOrIncreaseToken(string creatureID, UnlockTokenType tokenType, bool alwaysAddToken = false, params string[] SpecialData)
        {
            bool addToken = alwaysAddToken;
            UnlockToken RequiredToken = null;
            if (!addToken && _allUniqueUnlockTokens.TryGetValue(creatureID, out List<UnlockToken> creaturesTokens))
            {
                int tokensCount = creaturesTokens.Count;
                for (int i = 0; i < tokensCount; ++i)
                {
                    if (creaturesTokens[i].TokenType == tokenType)
                    {
                        addToken = true;
                        RequiredToken = creaturesTokens[i];
                    }
                }
            }

            if (addToken)
            {
                UnlockToken token = null;
                if (_ModuleUnlocks.TryGetValue(creatureID, out List<UnlockToken> registeredTokens))
                {
                    bool tokenExists = false;
                    for (int i = 0; i < registeredTokens.Count; ++i)
                    {
                        token = registeredTokens[i];
                        if (token.TokenType == tokenType)
                        {
                            if (alwaysAddToken)
                                if (token.Count < byte.MaxValue)
                                    ++token.Count;
                                else if (token.Count < RequiredToken.Count)
                                    ++token.Count;

                            foreach (string data in SpecialData)
                                if (!token.SpecialData.Contains(data))
                                    token.SpecialData.Add(data);

                            tokenExists = true;
                            break;
                        }
                    }

                    if (!tokenExists)
                    {
                        token = new UnlockToken(tokenType) { SpecialData = SpecialData.ToList() };
                        registeredTokens.Add(token);
                    }
                }
                else
                {
                    token = new UnlockToken(tokenType) { SpecialData = SpecialData.ToList() };
                    _ModuleUnlocks.Add(creatureID, new List<UnlockToken> { token });
                }

                if (!alwaysAddToken)
                    if (!_CreatureUnlockIDs.Contains(creatureID))
                        if (token.Count >= RequiredToken.Count)
                            _CreatureUnlockIDs.Add(creatureID);
            }
        }
        /// <inheritdoc cref="AddOrIncreaseToken(string, UnlockTokenType, bool, string[])"/>
        public static void AddOrIncreaseToken(Creature creature, UnlockTokenType tokenType, bool alwaysAddToken = false, params string[] SpecialData)
            => AddOrIncreaseToken(GetCreatureUnlockName(creature), tokenType, alwaysAddToken, SpecialData);
        /// <inheritdoc cref="AddOrIncreaseToken(string, UnlockTokenType, bool, string[])"/>
        public static void AddOrIncreaseToken(AbstractCreature creature, UnlockTokenType tokenType, bool alwaysAddToken = false, params string[] SpecialData)
            => AddOrIncreaseToken(GetCreatureUnlockName(creature), tokenType, alwaysAddToken, SpecialData);

        /// <summary>
        /// Checks if the given token is in either AutoModuleUnlocks or ModuleUnlocks
        /// </summary>
        /// <remarks>Returns true if the count is equal to or greater than the value in the registered token<code></code>
        /// Does not take into account if <see cref="BestiarySettings.UnlockAllEntries"/> is toggled</remarks>
        public static bool IsUnlockTokenValid(CreatureUnlockToken unlockToken)
        {
            if (_ModuleUnlocks.TryGetValue(unlockToken.CreatureID, out List<UnlockToken> value))
            {
                foreach (UnlockToken token in value)
                    if (unlockToken.Equals(token) && token.ContainsSpecialData(unlockToken.SpecialData))
                    {
                        if (token.Count > unlockToken.Count)
                        {
                            return true;
                        }
                    }
            }

            return false;
        }



        // The folder the save file is in
        internal static string SaveFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Videocult\\Rain World";
        // The save file to save to (yes, this file intentionally has no extension)
        internal static string SaveFile => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Videocult\\Rain World\\Bestiary" + Main.CurrentSaveSlot;
        // Saves all module unlocks and creature unlocks
        internal static void Save()
        {
            if (Directory.Exists(SaveFolder))
            {
                BestiarySaveData saveData = new BestiarySaveData(_ModuleUnlocks, _CreatureUnlockIDs);
                File.WriteAllText(SaveFile, JsonConvert.SerializeObject(saveData));
            }
        }
        // Clears all the currently loaded save data, such as Module unlocks and creature unlock ids
        internal static void ClearLoadedSaveData()
        {
            _ModuleUnlocks.Clear();
            _CreatureUnlockIDs.Clear();
        }

        // Deletes all the save data in the current slot
        internal static void DELETESaveDataInSlot()
        {
            ClearLoadedSaveData();
            Save();
        }

        // Loads all saved data into the bestiary, such as module unlock tokens and creature unlock ids
        internal static void Load()
        {
            if (File.Exists(SaveFile))
            {
                BestiarySaveData saveData = JsonConvert.DeserializeObject<BestiarySaveData>(File.ReadAllText(SaveFile));
                _ModuleUnlocks = saveData.ModuleUnlocks.ToDictionary(v => v.Key, v => v.Value);
                _CreatureUnlockIDs = saveData.CreatureUnlockIDs;
            }
        }

        /// <summary>
        /// All the tabs, which hold all the entries, to add your own, use AddEntries
        /// </summary>
        public static EntriesTabList EntriesTabs { get; internal set; }

        /// <summary>
        /// Gets an entry using a reference id, <code></code>
        /// Reference IDs contain the name of the tab to look in (case sensitive), plus the name of the entry to look for (also case sensitive, this is the name given to the entry, not the translated result), separated by either a forward or backward slash.
        /// Valid IDs include:
        /// <code>Rain World/creaturetype_Fly</code>
        /// <code>Rain World\creaturetype_CicadaA</code>
        /// <code>Downpour/The Gourmand</code>
        /// Invalid IDs include
        /// <code>rain world/creaturetype_Fly</code>
        /// <code>RainWorld/creaturetype_Fly</code>
        /// <code>/Rain World/creaturetype_Fly</code>
        /// </summary>
        /// <returns>The entry that was found, if no entry was found, null is returned</returns>
        public static Entry GetEntryByReferenceID(string refID)
        {
            string[] split = refID.Split('\\', '/');

            if (EntriesTabs.TryGet(split[0], out EntriesTab tab))
            {
                if (tab.TryGet(split[1], out Entry entry))
                {
                    return entry;
                }
            }

            return null;
        }

        internal static List<EntryMenu> PreviousMenusChain = new List<EntryMenu>();

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
    }
}
