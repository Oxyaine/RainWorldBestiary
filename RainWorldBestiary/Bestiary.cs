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
        internal static bool IncludeDownpour = false;

        /// <summary>
        /// Whether downpour entries are shown or not, true if more slug cats is enabled, otherwise false
        /// </summary>
        public static bool IncludeDownpourEntries => IncludeDownpour;

        internal const string DownpourTabName = "Downpour";


        internal readonly static List<string> CreatureUnlockIDsOverride = new List<string>();


        /// <summary>
        /// All the unlocked entries, this determines if an entry should be unlocked or not, even if a piece of the description is visible, the entry wont be visible unless its id is in this list
        /// </summary>
        public readonly static List<string> CreatureUnlockIDs = new List<string>();


        private static readonly Dictionary<string, List<UnlockToken>> _ModuleUnlocks = new Dictionary<string, List<UnlockToken>>();
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
        /// <param name="unlockCreature">Whether to add this creatureID to <see cref="CreatureUnlockIDs"/> if it's not already in there.<code></code>
        /// <see cref="CreatureUnlockIDs"/> is the list that determines whether an entry is unlocked or not, read <see cref="CreatureUnlockIDs"/>' summary for more details</param>
        public static void AddOrIncreaseModuleUnlock(string creatureID, UnlockTokenType tokenType, bool unlockCreature = true)
        {
            if (_ModuleUnlocks.ContainsKey(creatureID))
            {
                int cache = _ModuleUnlocks[creatureID].Count;
                for (int i = 0; i < cache; i++)
                {
                    if (_ModuleUnlocks[creatureID][i].TokenType == tokenType)
                    {
                        if (_ModuleUnlocks[creatureID][i].Count < 255)
                            ++_ModuleUnlocks[creatureID][i].Count;

                        return;
                    }
                }

                _ModuleUnlocks[creatureID].Add(new UnlockToken(tokenType));
            }
            else
            {
                _ModuleUnlocks.Add(creatureID, new List<UnlockToken> { new UnlockToken(tokenType) });
            }

            if (unlockCreature)
                if (!CreatureUnlockIDs.Contains(creatureID))
                    CreatureUnlockIDs.Add(creatureID);
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


        internal static string SaveFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Videocult\\Rain World";
        internal static string SaveFile => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\Videocult\\Rain World\\Bestiary";
        internal static void SaveUnlockTokens()
        {
            if (Directory.Exists(SaveFolder))
                File.WriteAllText(SaveFile, JsonConvert.SerializeObject(_ModuleUnlocks.ToArray()));
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
        /// A list of creature ids that wont be automatically tracked, any ID's found in here, will not be added to the <see cref="ModuleUnlocks"/> automatically
        /// </summary>
        public static List<string> AutoTrackIgnoredIDs = new List<string>();

        /// <summary>
        /// Special logic to apply to certain IDs, for example, `CicadaA` and `CicadaB` (Squidacada's ID's) have custom logic to remove the `A` and `B` so its just `Cicada`
        /// </summary>
        public static Dictionary<string, Func<string, string>> SpecialIDsLogic = new Dictionary<string, Func<string, string>>
        {
            { "CicadaA", CicadaSpecialIDLogic },
            { "CicadaB", CicadaSpecialIDLogic }
        };

        private static string CicadaSpecialIDLogic(string id) => id.Substring(0, id.Length - 1);
    }
}
