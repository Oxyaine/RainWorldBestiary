using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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


        /// <summary>
        /// All the manual module unlock tokens, the first element defines the id of the creature the token belongs to, the second element is a list of all unlock tokens belonging to that creature
        /// </summary>
        /// <remarks>
        /// If you'd like to add your own token, I'd recommended using <see cref="AddOrIncreaseModuleUnlock(string, UnlockTokenType, bool)"/> as it will increase the token if it exists, otherwise adds it, (which just means you don't need to write, essentially the same code, yourself)
        /// </remarks>
        public static Dictionary<string, List<UnlockToken>> ModuleUnlocks = new Dictionary<string, List<UnlockToken>>();
        /// <summary>
        /// Checks if <see cref="ModuleUnlocks"/> contains a <see cref="UnlockTokenType"/> that belongs to the given creature, if it does, the module unlock token gets increased by 1, otherwise its added as a new element
        /// </summary>
        /// <param name="creatureID">The ID of the creature this unlock token will target</param>
        /// <param name="tokenType">The type of token</param>
        /// <param name="unlockCreature">Whether to add this creatureID to <see cref="CreatureUnlockIDs"/> if it's not already in there</param>
        public static void AddOrIncreaseModuleUnlock(string creatureID, UnlockTokenType tokenType, bool unlockCreature = true)
        {
            if (ModuleUnlocks.ContainsKey(creatureID))
            {
                int cache = ModuleUnlocks[creatureID].Count;
                for (int i = 0; i < cache; i++)
                {
                    if (ModuleUnlocks[creatureID][i].TokenType == tokenType)
                    {
                        if (ModuleUnlocks[creatureID][i].Count < 255)
                            ++ModuleUnlocks[creatureID][i].Count;

                        return;
                    }
                }

                ModuleUnlocks[creatureID].Add(new UnlockToken(tokenType));
            }
            else
            {
                ModuleUnlocks.Add(creatureID, new List<UnlockToken> { new UnlockToken(tokenType) });
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
            if (ModuleUnlocks.TryGetValue(unlockToken.CreatureID, out List<UnlockToken> value))
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



        /// <summary>
        /// With loads of module unlocks, this can take up to 20 seconds to run (wont freeze the game, runs in background) changes are only applied to <see cref="ModuleUnlocks"/> when this is finished
        /// </summary>
        internal static IEnumerator PerformUnlockTokensCleanup()
        {
            Dictionary<string, List<UnlockToken>> cache = new Dictionary<string, List<UnlockToken>>(ModuleUnlocks);
            foreach (string creature in cache.Keys)
            {
                Dictionary<UnlockToken, int> result = new Dictionary<UnlockToken, int>();
                foreach (UnlockToken token in cache[creature])
                {
                    if (result.ContainsKey(token))
                    {
                        if ((result[token] += token.Count) > 255)
                            result[token] = 255;
                    }
                    else
                        result.Add(token, token.Count);

                    yield return null;
                }

                List<UnlockToken> compressed = new List<UnlockToken>();
                foreach (var v in result)
                {
                    UnlockToken token = v.Key;
                    token.Count = (byte)v.Value;
                    compressed.Add(token);

                    yield return null;
                }

                cache[creature] = compressed;

                yield return null;
            }

            ModuleUnlocks = cache;
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


        /// <inheritdoc cref="GetCreatureUnlockName(AbstractCreature)"/>
        public static string GetCreatureUnlockName(Creature creature) => GetCreatureUnlockName(creature.abstractCreature);
        /// <summary>
        /// Gets the creatures name through <see cref="AbstractCreature.creatureTemplate"/> and removes all white space characters
        /// </summary>
        public static string GetCreatureUnlockName(AbstractCreature creature) => creature.creatureTemplate.name.Trim().Replace(" ", "");
    }

    /// <summary>
    /// The type of unlock this UnlockToken targets
    /// </summary>
    public enum UnlockTokenType : byte
    {
        /// <summary>
        /// This means this part of the description will always be visible if the entry is visible, however, unlike modules with no unlock token, this wont make the entry visible
        /// </summary>
        None = 0,
        /// <summary>
        /// For when the player tames the creature
        /// </summary>
        Tamed = 1,
        /// <summary>
        /// For when the player evades the creature, by dodging an attack, climbing to a place it can't reach, etc
        /// </summary>
        Evaded = 2,
        /// <summary>
        /// For when the player sneaks past a creature
        /// </summary>
        SnuckPast = 3,
        /// <summary>
        /// For when the player sees the creature
        /// </summary>
        Observed = 4,
        /// <summary>
        /// For when the player sees the creature run away in fear
        /// </summary>
        ObserveFear = 5,
        /// <summary>
        /// For when the player sees the creature eating or hunting another creature
        /// </summary>
        ObserveFood = 6,
        /// <summary>
        /// For when the player is getting chased by a creature
        /// </summary>
        ObserveHunting = 7,
        /// <summary>
        /// When the creature is killed by the player
        /// </summary>
        Killed = 8,
        /// <summary>
        /// When the creature gets impaled with a spear, by the player
        /// </summary>
        Impaled = 9,
        /// <summary>
        /// When the creature is stunned, by the player
        /// </summary>
        Stunned = 10,
        /// <summary>
        /// When the player is killed by the creature
        /// </summary>
        KilledPlayer = 11,
        /// <summary>
        /// When the player is grabbed by the creature
        /// </summary>
        GrabbedPlayer = 12,
        /// <summary>
        /// Whenever the creature gets eaten by the player
        /// </summary>
        Eaten = 13,
        /// <summary>
        /// Whenever the player observes a creature being attracted to something, such as batflies to batnip
        /// </summary>
        ObserveAttraction = 14
    }

    /// <summary>
    /// The base unlock token, used to register tokens so modules can be unlocked, inherited by <see cref="CreatureUnlockToken"/> which is used in <see cref="DescriptionModule"/> as the unlock ID
    /// </summary>
    public class UnlockToken : IEquatable<UnlockToken>
    {
        /// <summary>
        /// The type of token this module unlock targets
        /// </summary>
        [JsonProperty("token_type")]
        public readonly UnlockTokenType TokenType = UnlockTokenType.None;

        /// <summary>
        /// The amount of times this token has been registered, or needs to be registered
        /// </summary>
        [JsonProperty("count")]
        public byte Count;

        [JsonProperty("value")]
        private byte Value { set => Count = value; }

        /// <param name="tokenType">The type of token to look for</param>
        /// <param name="value">The amount of times this token should be registered before this is considered unlocked</param>
        public UnlockToken(UnlockTokenType tokenType, byte value = 1)
        {
            TokenType = tokenType;
            Count = value;
        }

        /// <summary>
        /// Checks if both objects are <see cref="UnlockToken"/>, then compares them using <see cref="Equals(UnlockToken)"/>
        /// </summary>
        /// <remarks><see cref="Equals(UnlockToken)"/> <inheritdoc cref="Equals(UnlockToken)"/></remarks>
        public override bool Equals(object obj) => obj is UnlockToken token && Equals(token);

        /// <remarks>
        /// Checks if the token type matches, ignores <see cref="Count"/>
        /// </remarks>
        public bool Equals(UnlockToken other) => TokenType.Equals(other.TokenType);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <returns>Token Type + Value</returns>
        public override string ToString()
        {
            return TokenType + " " + Count;
        }
    }
    /// <summary>
    /// An unlock token, that can be used to detect whether a <see cref="DescriptionModule"/> is unlocked, similar to <see cref="UnlockToken"/> but has a CreatureID string
    /// </summary>
    public class CreatureUnlockToken : UnlockToken, IEquatable<CreatureUnlockToken>, IEquatable<UnlockToken>
    {
        /// <summary>
        /// The ID of the creature this unlock token applies to
        /// </summary>
        [JsonProperty("creature_id")]
        public readonly string CreatureID = string.Empty;

        /// <param name="creatureID">The ID of the creature that to look for</param>
        /// <param name="tokenType">The type of token to look for</param>
        /// <param name="value">The amount of times this token should be registered before this is considered unlocked</param>
        public CreatureUnlockToken(string creatureID, UnlockTokenType tokenType, byte value = 1)
            : base(tokenType, value) => CreatureID = creatureID;

        /// <summary>
        /// Checks if both objects are <see cref="CreatureUnlockToken"/>, then compares them using <see cref="Equals(CreatureUnlockToken)"/>
        /// </summary>
        /// <remarks><see cref="Equals(CreatureUnlockToken)"/> <inheritdoc cref="Equals(CreatureUnlockToken)"/></remarks>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case CreatureUnlockToken cToken: return Equals(cToken);
                case UnlockToken token: return Equals(token);
                default: return false;
            }
        }

        /// <remarks>
        /// Checks if the creature ID matches, then if the token type matches, ignores <see cref="UnlockToken.Count"/>
        /// </remarks>
        public bool Equals(CreatureUnlockToken other) => CreatureID.Equals(other.CreatureID) && TokenType.Equals(other.TokenType);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <returns>Creature ID + Token Type + Value</returns>
        public override string ToString()
        {
            return string.Join(" ", CreatureID, TokenType, Count);
        }
    }



    /// <summary>
    /// A class that represents a list of <see cref="EntriesTab"/>
    /// </summary>
    /// <remarks>Not using regular list since this allows more control such as preventing two <see cref="EntriesTab"/> with the same name</remarks>
    public class EntriesTabList : IEnumerable<EntriesTab>, ICollection<EntriesTab>
    {
        ///
        public EntriesTabList() { }
        ///
        public EntriesTabList(params EntriesTab[] tabs)
        {
            _tabs = tabs.ToList();
        }

        readonly List<EntriesTab> _tabs = new List<EntriesTab>();

        /// <inheritdoc/>
        public int Count => _tabs.Count;
        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<EntriesTab>)_tabs).IsReadOnly;
        /// <summary>
        /// Adds a new tab of entries to this collection
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="merge">
        /// Whether to resolve the case that two tabs have the same name, by merging them together. Merges as per the rules of <see cref="EntriesTab.MergeWith(in EntriesTab)"/><code></code>
        /// </param>
        public void Add(EntriesTab item, bool merge = false)
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].Name.Equals(item.Name))
                {
                    if (!merge)
                    {
                        Main.Logger.LogError("A tab with the name " + item.Name + "already exists.");
                        throw new Exception("A tab with the name " + item.Name + " already exists.");
                    }
                    else
                    {
                        _tabs[i].AddRange(item.GetEnumerator());
                        return;
                    }
                }
            }

            _tabs.Add(item);
        }
        /// <inheritdoc cref="Add(EntriesTab, bool)"/>
        public void Add(EntriesTab item) => Add(item, false);

        /// <inheritdoc cref="Add(EntriesTab, bool)"/>
        public void Add(string tabName, params Entry[] entries)
        {
            Add(new EntriesTab(tabName, entries), false);
        }
        /// <inheritdoc cref="Add(EntriesTab, bool)"/>
        public void Add(string tabName, bool merge, params Entry[] entries)
        {
            Add(new EntriesTab(tabName, entries), merge);
        }
        /// <inheritdoc cref="Add(EntriesTab, bool)"/>
        public void Add(string tabName, IEnumerable<Entry> entries, TitleSprite titleSprite = null, bool merge = false)
        {
            Add(new EntriesTab(tabName, entries) { TitleSprite = titleSprite }, merge);
        }
        /// <inheritdoc cref="Add(EntriesTab, bool)"/>
        public void Add(string tabName, IEnumerable<Entry> entries, ProcessManager.ProcessID menuProcessID, TitleSprite titleSprite = null, bool merge = false)
        {
            Add(new EntriesTab(tabName, entries) { TabMenuProcessID = menuProcessID, TitleSprite = titleSprite }, merge);
        }



        /// <inheritdoc/>
        public void Clear() => _tabs.Clear();
        /// <inheritdoc/>
        public bool Contains(EntriesTab item) => _tabs.Contains(item);
        /// <inheritdoc/>
        public void CopyTo(EntriesTab[] array, int arrayIndex) => _tabs.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(EntriesTab item) => _tabs.Remove(item);

        /// <inheritdoc/>
        public IEnumerator<EntriesTab> GetEnumerator() => _tabs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _tabs.GetEnumerator();

        /// <summary>
        /// Gets the <see cref="EntriesTab"/> at the given index
        /// </summary>
        public EntriesTab this[int index] => _tabs[index];
    }


    /// <summary>
    /// Represents an element in the atlas manager, but gives some more options to customize the scale and offset of the image from the default values
    /// </summary>
    public class TitleSprite
    {
        /// <summary>
        /// The name of the element in the atlas manager
        /// </summary>
        [JsonProperty("element_name")]
        public string ElementName = string.Empty;
        /// <summary>
        /// The scale of the image when drawn to the screen
        /// </summary>
        [JsonProperty("scale")]
        public float Scale = 1;
        /// <summary>
        /// The offset on the X axis from the default position
        /// </summary>
        [JsonProperty("x_offset")]
        public int XOffset = 0;
        /// <summary>
        /// THe offset on the Y axis from the default position
        /// </summary>
        [JsonProperty("y_offset")]
        public int YOffset = 0;

        ///
        public TitleSprite(string elementName)
        {
            ElementName = elementName;
        }
    }

    /// <summary>
    /// A class representing a tab full of entries in the bestiary
    /// </summary>
    public class EntriesTab
    {
        /// <summary>
        /// The name of this tab
        /// </summary>
        [JsonProperty("name")]
        public string Name = string.Empty;
        /// <summary>
        /// The title image that gets displayed at the top when of the screen when viewing the tab, if set to null, or if the image isn't found, some generated text will be placed instead
        /// </summary>
        /// <remarks>By title, I mean the name of the tab that is visible at the top while viewing entries in the tab</remarks>
        [JsonProperty("title_image")]
        public TitleSprite TitleSprite = null;

        /// <summary>
        /// The process ID that gets called when a tab button gets pressed, you can leave this as the default menu, or make a custom menu to display entries.
        /// </summary>
        [JsonIgnore]
        public ProcessManager.ProcessID TabMenuProcessID = Main.BestiaryTabMenu;

        [JsonProperty("tab_menu_process_id")]
        private string MenuProcessID
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!Main.BestiaryTabMenu.value.Equals(value))
                    {
                        TabMenuProcessID = new ProcessManager.ProcessID(value, true);
                    }
                }
            }
        }

        [JsonIgnore]
        readonly List<Entry> _entries = new List<Entry>();

        [JsonIgnore]
        internal List<string> ContributingMods = new List<string>();

        /// <summary>
        /// Creates an empty <see cref="EntriesTab"/>
        /// </summary>
        public EntriesTab()
        {
            Name = string.Empty;
            _entries = new List<Entry>();
        }
        /// <summary>
        /// Creates an <see cref="EntriesTab"/> with all entries in <paramref name="entries"/> as the entries
        /// </summary>
        public EntriesTab(IEnumerable<Entry> entries)
        {
            Name = string.Empty;
            _entries = entries.ToList();
        }
        /// <inheritdoc cref="EntriesTab(IEnumerable{Entry})"/>
        public EntriesTab(string tabName, IEnumerable<Entry> entries)
        {
            Name = tabName;
            _entries = entries.ToList();
        }
        /// <inheritdoc cref="EntriesTab(IEnumerable{Entry})"/>
        public EntriesTab(string tabName, params Entry[] entries)
        {
            Name = tabName;
            _entries = entries.ToList();
        }

        /// <summary>
        /// Gets the amount of entries in this tab
        /// </summary>
        public int Count => _entries.Count;

        /// <inheritdoc cref="Add(string, string, string, string, string)"/>
        public void Add(Entry item)
        {
            if (Contains(item.Name))
                throw new Exception("The entry with the name " + item.Name + " already exists in this tab!");

            _entries.Add(item);

            if (!ContributingMods.Contains(item.OwningModID))
                ContributingMods.Add(item.OwningModID);
        }
        /// <inheritdoc cref="Add(string, string, string, string, string)"/>
        public void Add(string entryName, EntryInfo info, string owningModID = null)
        {
            Add(new Entry(entryName, info, owningModID));
        }
        /// <inheritdoc cref="Add(string, string, string, string, string)"/>
        public void Add(string entryName, string unlockID, string iconAtlasName, Description description, string owningModID = null)
        {
            Add(new Entry(entryName, new EntryInfo() { UnlockID = unlockID, EntryIcon = iconAtlasName, Description = description }, owningModID));
        }
        /// <summary>
        /// Adds a new entry to this tab
        /// </summary>
        /// <param name="owningModID"><inheritdoc cref="Entry(string, string)"/></param>
        /// <param name="entryName"></param>
        /// <param name="unlockID"></param>
        /// <param name="iconAtlasName"></param>
        /// <param name="description"></param>
        public void Add(string entryName, string unlockID, string iconAtlasName, string description, string owningModID = null)
        {
            Add(new Entry(entryName, new EntryInfo() { UnlockID = unlockID, EntryIcon = iconAtlasName, Description = new Description(new DescriptionModule() { Body = description }) }, owningModID));
        }

        /// <summary>
        /// Adds all the entries from the collection into this tab
        /// </summary>
        public void AddRange(IEnumerable<Entry> items)
        {
            foreach (Entry item in items)
                Add(item);
        }
        /// <inheritdoc cref="AddRange(IEnumerable{Entry})"/>
        public void AddRange(IEnumerator<Entry> items)
        {
            do
            {
                Add(items.Current);
            }
            while (items.MoveNext());
        }

        /// <summary>
        /// Adds <paramref name="tab"/>'s entries to this this<code></code>
        /// If this TitleImage is null, it gets replaced with <paramref name="tab"/>'s TitleImage<code></code>
        /// If this TabMenuProcessID is set to the default, it sets it to <paramref name="tab"/>'s TabMenuProcessID
        /// </summary>
        /// <param name="tab"></param>
        public void MergeWith(in EntriesTab tab)
        {
            _entries.AddRange(tab._entries);

            foreach (string contributor in tab.ContributingMods)
                if (ContributingMods.Contains(contributor))
                    ContributingMods.Add(contributor);

            if (TitleSprite == null)
                TitleSprite = tab.TitleSprite;

            if (TabMenuProcessID == Main.BestiaryTabMenu && tab.TabMenuProcessID != Main.BestiaryTabMenu)
                TabMenuProcessID = tab.TabMenuProcessID;
        }

        /// <summary>
        /// Clears this tab of all its entries
        /// </summary>
        public void Clear() => _entries.Clear();

        /// <summary>
        /// Checks if this tab contains the given entry
        /// </summary>
        public bool Contains(Entry item) => _entries.Contains(item);
        /// <summary>
        /// Determines whether this tab contains an entry with the given name
        /// </summary>
        /// <param name="entryName">The name of the entry to check for</param>
        public bool Contains(string entryName)
        {
            foreach (Entry entry in _entries)
            {
                if (entry.Name.Equals(entryName))
                    return true;
            }

            return false;
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)"/>
        public void CopyTo(Entry[] array, int arrayIndex) => _entries.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the given entry from this tab
        /// </summary>
        public bool Remove(Entry item) => _entries.Remove(item);

        /// <summary>
        /// Removes an entry at the specified index
        /// </summary>
        public void RemoveAt(int index) => _entries.RemoveAt(index);

        /// <summary>
        /// Gets or sets an entry at the given index
        /// </summary>
        public Entry this[int index] { get => _entries[index]; set => _entries[index] = value; }

        /// <summary>
        /// Gets an entry with the given name
        /// </summary>
        public Entry this[string entryName]
        {
            get
            {
                foreach (Entry entry in _entries)
                {
                    if (entry.Name.Equals(entryName))
                    {
                        return entry;
                    }
                }

                throw new KeyNotFoundException("The entry with the name " + entryName + " was not found in the tab.");
            }
        }

        /// <inheritdoc/>
        public IEnumerator<Entry> GetEnumerator() => _entries.GetEnumerator();
    }

    /// <summary>
    /// A class representing an entry
    /// </summary>
    public class Entry : IEquatable<Entry>
    {
        /// <summary>
        /// The name of this entry
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// The information of this entry, such as its unlock id, icon, scene to show while reading, and description
        /// </summary>
        public EntryInfo Info = new EntryInfo();

        /// <summary>
        /// The process ID that gets called when an entry button gets pressed, you can leave this as the default menu, or make a custom menu to display the entry's information.
        /// </summary>
        public ProcessManager.ProcessID EntryReadingMenu = Main.EntryReadingMenu;

        internal readonly string OwningModID = null;

        ///
        public Entry()
        {
        }
        ///<param name="name"></param>
        /// <param name="owningModID">The ID of the mod (id that is set in `modinfo.json` file) that this entry belongs to, set this if you'd like this entry to automatically unload when the mod gets disabled</param>
        public Entry(string name, string owningModID = null) : this()
        {
            Name = name;
            OwningModID = owningModID;
        }
        /// <param name="info">The entry's info</param>
        /// <inheritdoc cref="Entry(string, string)"/>
        /// <param name="name"></param>
        /// <param name="owningModID"></param>
        public Entry(string name, EntryInfo info, string owningModID = null) : this(name, owningModID)
        {
            Name = name;
            Info = info;
        }
        /// <param name="name">The name of the entry</param>
        /// <param name="description">The main body of this entry</param>
        /// <param name="unlockID">The ID that will be used to determine whether this entry is unlocked or not</param>
        /// <param name="iconAtlasName">The name of the entry's icon in the atlas manager</param>
        /// <param name="lockedText">The text that is shown when pressing on the entry while its locked</param>
        /// <inheritdoc cref="Entry(string, string)"/>
        /// <param name="owningModID"></param>
        public Entry(string name, Description description, string unlockID = "", string iconAtlasName = "", string lockedText = EntryInfo.BaseLockedText, string owningModID = null)
            : this(name, owningModID)
        {
            Info = new EntryInfo() { UnlockID = unlockID, EntryIcon = iconAtlasName, Description = description, LockedText = lockedText };
        }
        /// <inheritdoc cref="Entry(string, Description, string, string, string, string)"/>
        public Entry(string name, string description, string unlockID = "", string iconAtlasName = "", string lockedText = EntryInfo.BaseLockedText, string owningModID = null)
            : this(name, owningModID)
        {
            Info = new EntryInfo() { UnlockID = unlockID, EntryIcon = iconAtlasName, LockedText = lockedText, Description = new Description(new DescriptionModule() { Body = description }) };
        }

        /// <summary>
        /// A default entry that's just an error, is always unlocked, and serves as a placeholder that appears when another entry cant be loaded
        /// </summary>
        public static Entry Error => new Entry("ERROR")
        {
            Info = new EntryInfo("Something went wrong with an entry, so this has been created as a warning.\nYou can check the log to see exactly what went wrong.\n", entryIcon: "illustrations\\bestiary\\icons\\error")
            {
                EntryUnlockedCondition = e => false,
                EntryColor = new HSLColor(0f, 0.8f, 0.6f)
            },
        };

        /// <summary>
        /// Checks if this entry is the same as another entry
        /// </summary>
        public bool Equals(Entry other) => OwningModID.Equals(other.OwningModID) && Name.Equals(other.Name) && Info.Equals(other.Info);
    }

    /// <summary>
    /// The contents of the entry file
    /// </summary>
    public class EntryInfo : IEquatable<EntryInfo>
    {
        /// <summary>
        /// A constant defining the default text that is shown when attempting to read a locked entry
        /// </summary>
        public const string BaseLockedText = "This entry is locked.";

        /// <summary>
        /// The ID of this entry, if the ID is found in the unlocked entries dictionary, this entry will be made visible
        /// </summary>
        [JsonProperty("unlock_id")]
        public string UnlockID = string.Empty;

        /// <summary>
        /// The condition that specifies whether this entry is visible or not, if this returns true, then the entry is visible. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultEntryUnlockedCondition(EntryInfo)"/></remarks>
        [JsonIgnore]
        public Func<EntryInfo, bool> EntryUnlockedCondition = DefaultEntryUnlockedCondition;
        /// <summary>
        /// Checks whether any unlock tokens in <see cref="Bestiary"/> have the <see cref="UnlockTokenType"/> for <see cref="CreatureUnlockToken.CreatureID"/> with a value that is equal to or lower than the required value
        /// </summary>
        /// <returns>True if the entry should be locked, otherwise false</returns>
        public static bool DefaultEntryUnlockedCondition(EntryInfo info)
        {
            return Bestiary.CreatureUnlockIDs.Contains(info.UnlockID) || Bestiary.CreatureUnlockIDsOverride.Contains(info.UnlockID);
        }

        /// <summary>
        /// Checks if this entry info's unlock id and icons match
        /// </summary>
        public bool Equals(EntryInfo other) => UnlockID.Equals(other.UnlockID) && EntryIcons.SequenceEqual(other.EntryIcons);

        /// <summary>
        /// Returns true if the entry is visible, else false
        /// </summary>
        [JsonIgnore]
        public bool EntryUnlocked => EntryUnlockedCondition == null || EntryUnlockedCondition(this);


        /// <summary>
        /// The text / tip that is shown when attempting to read the entry while its locked, this could be anything you want, leave blank for no message.
        /// </summary>
        [JsonProperty("locked_text")]
        public string LockedText = BaseLockedText;


        /// <summary>
        /// Whether the two icons specified by <see cref="EntryIcon"/> will be displayed either side of the title
        /// </summary>
        [JsonProperty("icons_next_to_title")]
        public bool IconsNextToTitle = true;


        /// <summary>
        /// The name of the sprite in the atlas manager that will be used as the entry icon
        /// </summary>
        [JsonProperty("entry_icon")]
        public string EntryIcon
        {
            set
            {
                EntryIcons = EntryIcons.Append(value).ToArray();
            }
        }

        /// <summary>
        /// The name of the sprites in the atlas manager that will be used as the entry's icons
        /// </summary>
        [JsonProperty("entry_icons")]
        public string[] EntryIcons = new string[0];

        /// <summary>
        /// The title image that gets displayed at the top when of the screen while reading the entry, if set to null, or if the image isn't found, some generated text will be placed instead
        /// </summary>
        /// <remarks>By title, I mean the name of the entry that is visible at the top while reading the entry</remarks>
        [JsonProperty("title_sprite")]
        public TitleSprite TitleSprite = null;


        /// <summary>
        /// The color of the entry's button and title image
        /// </summary>
        [JsonIgnore]
        public HSLColor EntryColor = new HSLColor(0.4f, 0.6f, 0.9f);


        [JsonProperty("color")]
        private string JSON_Color
        {
            //get
            //{
            //    UnityEngine.Color rgb = EntryColor.rgb;
            //    rgb.r.ToString("X2");

            //    return (UnityEngine.Mathf.RoundToInt(rgb.r * 255f)).ToString("X2") +
            //        (UnityEngine.Mathf.RoundToInt(rgb.g * 255f)).ToString("X2") +
            //        (UnityEngine.Mathf.RoundToInt(rgb.b * 255f)).ToString("X2");
            //}
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                string def;
                if (value.Length > 6)
                    def = value.Substring(value.Length - 6);
                else
                    def = "DFF5D6".Substring(0, value.Length) + value;
                string[] values = def.SplitIntoGroups(2);

                float R = Convert.ToByte(values[1], 16) / 255f;
                float G = Convert.ToByte(values[2], 16) / 255f;
                float B = Convert.ToByte(values[3], 16) / 255f;

                float max = Math.Max(R, Math.Max(G, B));
                float min = Math.Min(R, Math.Min(G, B));
                float delta = max - min;

                float Hue;
                if (delta == 0f)
                {
                    Hue = 0f;
                }
                else
                {
                    if (max.Equals(R))
                        Hue = 60f * ((G - B) / delta % 6f);
                    else if (max.Equals(G))
                        Hue = 60f * ((B - R) / delta + 2f);
                    else
                        Hue = 60f * ((R - G) / delta + 4f);
                }

                float Lightness = (max + min) / 2f;
                float Saturation = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * Lightness - 1));

                if (Hue < 0)
                    Hue += 360f;
                Hue /= 360f;

                EntryColor = new HSLColor(Hue, Lightness, Saturation);
            }
        }


        /// <summary>
        /// The body of this entry, when converted to string, only returns the parts of the entry that are visible
        /// </summary>
        [JsonProperty("description")]
        public Description Description = new Description();

        ///
        public EntryInfo()
        {
        }
        /// <param name="description">The body of this entry</param>
        /// <param name="iD">The ID of this entry, if the ID is found in the unlocked entries dictionary, this entry will be made visible</param>
        /// <param name="lockedText">The text / tip that is shown when attempting to read the entry while its locked, this could be anything you want, leave blank for no message.</param>
        /// <param name="entryIcon">The name of the sprite in the atlas manager that will be used as the entry icon</param>
        public EntryInfo(Description description, string iD = "", string lockedText = BaseLockedText, string entryIcon = "")
        {
            UnlockID = iD;
            LockedText = lockedText;
            EntryIcon = entryIcon;
            Description = description;
        }
        /// <summary>
        /// Creates a new entry info with one description module that is by default visible
        /// </summary>
        /// <param name="description">The body of this entry</param>
        /// <param name="iD">The ID of this entry, if the ID is found in the unlocked entries dictionary, this entry will be made visible</param>
        /// <param name="lockedText">The text / tip that is shown when attempting to read the entry while its locked, this could be anything you want, leave blank for no message.</param>
        /// <param name="entryIcon">The name of the sprite in the atlas manager that will be used as the entry icon</param>
        public EntryInfo(string description, string iD = "", string lockedText = BaseLockedText, string entryIcon = "")
        {
            UnlockID = iD;
            LockedText = lockedText;
            EntryIcon = entryIcon;
            Description = new Description(description);
        }
    }

    /// <summary>
    /// A class representing an entries description, saved as a DescriptionModule array, but can be used as a string
    /// </summary>
    public class Description : IEnumerable<DescriptionModule>, ICollection<DescriptionModule>
    {
        /// <summary>
        /// Whether the entire description should be available to read, regardless of which module are locked or unlocked, this is not saved by the json converter
        /// </summary>
        [JsonIgnore]
        public bool UnlockFullDescription = false;

        readonly List<DescriptionModule> _values = new List<DescriptionModule>();

        ///
        public Description() { }
        ///
        public Description(IEnumerable<DescriptionModule> modules)
        {
            _values = modules.ToList();
        }
        ///
        public Description(params DescriptionModule[] modules)
        {
            _values = modules.ToList();
        }
        /// <summary>
        /// Creates a new description with one module that defaults to unlocked
        /// </summary>
        public Description(string description)
        {
            _values = new List<DescriptionModule>() { new DescriptionModule() { Body = description } };
        }

        /// <summary>
        /// Gets the amount of description modules in this description
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// Gets the amount of description modules that are visible to the player
        /// </summary>
        public int UnlockedCount
        {
            get
            {
                int count = 0;
                foreach (DescriptionModule module in _values)
                {
                    if (!module.ModuleUnlocked)
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// This is unused in <see cref="Description"/>
        /// </summary>
        public bool IsReadOnly => ((ICollection<DescriptionModule>)_values).IsReadOnly;

        /// <summary>
        /// Gets or sets a module at the given index
        /// </summary>
        public DescriptionModule this[int index] { get => _values[index]; set => _values[index] = value; }


        /// <inheritdoc/>
        public IEnumerator<DescriptionModule> GetEnumerator() => _values.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();


        /// <summary>
        /// Checks if any modules of this description are visible
        /// </summary>
        /// <returns>True if atleast one module is visible, otherwise false</returns>
        public bool IsAnythingVisible()
        {
            foreach (DescriptionModule module in _values)
            {
                if (module.ModuleUnlocked)
                {
                    if (module.ContributesToEntryUnlock)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Returns this description as a string where only the parts of the entry that are visible are added
        /// </summary>
        public override string ToString()
        {
            string result = string.Empty;
            foreach (DescriptionModule module in _values)
            {
                if (module.ModuleUnlocked)
                {
                    result += "\n" + OptionInterface.Translate(module.ToString());
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public void Add(DescriptionModule item) => _values.Add(item);
        /// <inheritdoc/>
        public void Clear() => _values.Clear();
        /// <inheritdoc/>
        public bool Contains(DescriptionModule item) => _values.Contains(item);
        /// <inheritdoc/>
        public void CopyTo(DescriptionModule[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(DescriptionModule item) => _values.Remove(item);

        ///
        public static implicit operator Description(DescriptionModule[] modules) => new Description(modules);
        ///
        public static implicit operator Description(List<DescriptionModule> modules) => new Description(modules);
    }

    /// <summary>
    /// A piece of a description, can be given a lock ID to lock this part of an entries description
    /// </summary>
    public class DescriptionModule
    {
        /// <summary>
        /// The unlock token of this description module, used to determine what requirements need to be met to unlock this part of the description
        /// </summary>
        [JsonProperty("unlock_id")]
        public CreatureUnlockToken UnlockID = null;

        /// <summary>
        /// The condition that specifies whether this entry is visible or not, if this returns true, then the entry is visible. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultModuleUnlockedCondition(DescriptionModule)"/></remarks>
        [JsonIgnore]
        public Func<DescriptionModule, bool> ModuleUnlockedCondition = DefaultModuleUnlockedCondition;

        /// <summary>
        /// Checks if <see cref="UnlockID"/> is found in <see cref="Bestiary.ModuleUnlocks"/> by checking for the creature id and using <see cref="UnlockToken.Equals(UnlockToken)"/>
        /// </summary>
        public static bool DefaultModuleUnlockedCondition(DescriptionModule info)
            => BestiarySettings.UnlockAllEntries.Value || info.UnlockID == null || info.UnlockID.TokenType == UnlockTokenType.None || Bestiary.IsUnlockTokenValid(info.UnlockID);

        /// <summary>
        /// Returns true if the module is unlocked, else false
        /// </summary>
        [JsonIgnore]
        public bool ModuleUnlocked => ModuleUnlockedCondition == null || ModuleUnlockedCondition(this);

        /// <summary>
        /// Whether this module contributes to making the entry visible
        /// </summary>
        /// <remarks>Checks if the unlock id TokenType is none, if it is, this returns false, meaning this module doesn't contribute to unlocking the entire entry, see <see cref="UnlockTokenType.None"/> for more info</remarks>
        [JsonIgnore]
        public bool ContributesToEntryUnlock => UnlockID == null || UnlockID.TokenType != UnlockTokenType.None;

        /// <summary>
        /// The text of this part of the entries description
        /// </summary>
        [JsonProperty("text")]
        public string Body = string.Empty;

        ///
        public DescriptionModule() { }
        ///
        public DescriptionModule(string body)
        {
            Body = body;
        }

        /// <inheritdoc/>
        public override string ToString() => Body;
    }
}
