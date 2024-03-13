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

        /// <summary>
        /// All the ID's of entries that have been unlocked, and can be viewed (doesn't completely unlock entry, just makes it available, individual description components must still be unlocked individually)
        /// </summary>
        public readonly static List<string> UnlockedEntriesIDs = new List<string>();

        internal readonly static AutoModuleUnlockTokenList _AutoModuleUnlocks = new AutoModuleUnlockTokenList();
        /// <summary>
        /// All the module unlock tokens that are automatically tallied and registered, you can check <see cref="AutoTokenType"/> to see what is automatically detected
        /// </summary>
        public static AutoModuleUnlockTokenList AutoModuleUnlocks => _AutoModuleUnlocks;

        /// <summary>
        /// All the module unlock tokens that are manually registered, you can check <see cref="TokenType"/> to see what is manually detected
        /// </summary>
        public readonly static ModuleUnlockTokenList ModuleUnlocks = new ModuleUnlockTokenList();

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
        public static string GetCreatureUnlockName(AbstractCreature creature)
        {
            return creature.creatureTemplate.name.Trim().Replace(" ", "");
        }
    }

    /// <summary>
    /// A class representing a list of AutoModuleUnlockTokens, has custom behaviours and accessors
    /// </summary>
    public class AutoModuleUnlockTokenList : IEnumerable<AutoModuleUnlockToken>
    {
        private readonly List<AutoModuleUnlockToken> _values = new List<AutoModuleUnlockToken>();

        /// <summary>
        /// Gets the amount of items in this list
        /// </summary>
        public int Count => _values.Count;

        /// <inheritdoc/>
        internal void AddOrIncrease(AutoModuleUnlockToken item)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                if (_values[i].Equals(item))
                {
                    _values[i]++;
                    return;
                }
            }

            _values.Add(item);
        }

        /// <summary>
        /// Checks if the given token is valid in this collection, meaning the type and id are equal, and the value is greater than or equal to the other value
        /// </summary>
        public bool IsTokenValid(AutoModuleUnlockToken item)
        {
            foreach (AutoModuleUnlockToken autoToken in _values)
                if (item.Equals(autoToken))
                    return true;

            return false;
        }
        /// <inheritdoc cref="IsTokenValid(AutoModuleUnlockToken)"/>
        public bool IsTokenValid(UnlockToken item)
        {
            foreach (AutoModuleUnlockToken autoToken in _values)
                if (item.Equals(autoToken))
                    return true;

            return false;
        }

        /// <inheritdoc/>
        public IEnumerator<AutoModuleUnlockToken> GetEnumerator() => _values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
    }
    /// <summary>
    /// A class representing a list of ModuleUnlockTokens, has custom behaviours and accessors
    /// </summary>
    public class ModuleUnlockTokenList : IEnumerable<ModuleUnlockToken>
    {
        private readonly List<ModuleUnlockToken> _values = new List<ModuleUnlockToken>();

        /// <summary>
        /// Gets the amount of items in this list
        /// </summary>
        public int Count => _values.Count;

        /// <inheritdoc/>
        public void AddOrIncrease(ModuleUnlockToken item)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                if (_values[i].Equals(item))
                {
                    _values[i]++;
                    return;
                }
            }

            _values.Add(item);
        }

        /// <summary>
        /// Removes this exact item from the collection, exact meaning even the value has to be the same
        /// </summary>
        public void RemoveExact(ModuleUnlockToken item) => _values.Remove(item);

        /// <summary>
        /// Checks if the given token is valid in this collection, meaning the type and id are equal, and the value is greater than or equal to the other value
        /// </summary>
        public bool IsTokenValid(ModuleUnlockToken item)
        {
            foreach (ModuleUnlockToken autoToken in _values)
                if (item.Equals(autoToken))
                    return true;

            return false;
        }
        /// <inheritdoc cref="IsTokenValid(ModuleUnlockToken)"/>
        public bool IsTokenValid(UnlockToken item)
        {
            foreach (ModuleUnlockToken autoToken in _values)
                if (item.Equals(autoToken))
                    return true;

            return false;
        }

        /// <inheritdoc/>
        public IEnumerator<ModuleUnlockToken> GetEnumerator() => _values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
    }


    /// <summary>
    /// A base unlock module, holds shared unlock module logic
    /// </summary>
    public abstract class BaseUnlockModule : IEquatable<BaseUnlockModule>
    {
        /// <summary>
        /// The ID of the creature this unlock token applies to
        /// </summary>
        [JsonProperty("creature_id")]
        public readonly string CreatureID = string.Empty;
        /// <summary>
        /// The amount of times the unlock token was registered, caps at 255
        /// </summary>
        [JsonIgnore]
        protected byte _value = 1;

        ///
        public BaseUnlockModule(string creatureID, byte startingValue = 1)
        {
            CreatureID = creatureID;
            _value = startingValue;
        }

        /// <remarks>
        /// Checks if the ID matches, and if the given values count is greater than or equal to this values' count
        /// </remarks>
        public bool Equals(BaseUnlockModule other) => CreatureID.Equals(other.CreatureID) && other._value >= _value;
        /// <summary>
        /// Checks if the two objects are both <see cref="BaseUnlockModule"/>, then compares them using <see cref="Equals(BaseUnlockModule)"/>
        /// </summary>
        /// <remarks><see cref="Equals(BaseUnlockModule)"/> <inheritdoc cref="Equals(BaseUnlockModule)"/></remarks>
        public override bool Equals(object obj) => obj is BaseUnlockModule token && Equals(token);
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    /// <summary>
    /// The type of unlock token
    /// </summary>
    public enum TokenType : byte
    {
        ///
        Unknown = 0,
        /// <inheritdoc cref="UnlockTokenType.Tamed"/>
        Tamed = 1,
        /// <inheritdoc cref="UnlockTokenType.Evaded"/>
        Evaded = 2,
        /// <inheritdoc cref="UnlockTokenType.SnuckPast"/>
        SnuckPast = 3,
        /// <inheritdoc cref="UnlockTokenType.Observed"/>
        Observed = 4,
        /// <inheritdoc cref="UnlockTokenType.ObserveFear"/>
        ObserveFear = 5,
        /// <inheritdoc cref="UnlockTokenType.ObserveFood"/>
        ObserveFood = 6,
        /// <inheritdoc cref="UnlockTokenType.ObserveHunting"/>
        ObserveHunting = 7,
    }
    /// <summary>
    /// A class that represents an unlock token for a description module, this class represents a manual unlock token
    /// </summary>
    public class ModuleUnlockToken : BaseUnlockModule, IEquatable<ModuleUnlockToken>
    {
        /// <summary>
        /// The type of token this module unlock targets
        /// </summary>
        public readonly TokenType TokenType = TokenType.Unknown;

        /// <inheritdoc cref="BaseUnlockModule._value"/>
        public byte Value { get => _value; set => _value = value; }

        ///
        public ModuleUnlockToken(string creatureID, TokenType tokenType, byte startingValue = 1) : base(creatureID, startingValue)
        {
            TokenType = tokenType;
        }

        /// <summary>
        /// Increments Value by one
        /// </summary>
        public static ModuleUnlockToken operator ++(ModuleUnlockToken module)
        {
            module._value++;
            return module;
        }
        /// <summary>
        /// Decrements Value by one
        /// </summary>
        public static ModuleUnlockToken operator --(ModuleUnlockToken module)
        {
            module._value--;
            return module;
        }

        /// <summary>
        /// Checks if both objects are <see cref="ModuleUnlockToken"/>, then compares them using <see cref="Equals(ModuleUnlockToken)"/>
        /// </summary>
        /// <remarks><see cref="Equals(ModuleUnlockToken)"/> <inheritdoc cref="Equals(ModuleUnlockToken)"/></remarks>
        public override bool Equals(object obj) => obj is ModuleUnlockToken other && Equals(other);
        /// <inheritdoc cref="BaseUnlockModule.Equals(BaseUnlockModule)"/>
        /// <remarks>Also checks if the tokenType matches</remarks>
        public bool Equals(ModuleUnlockToken other) => TokenType.Equals(other.TokenType) && base.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// The type of automated unlock token
    /// </summary>
    public enum AutoTokenType : byte
    {
        ///
        Unknown = 0,
        /// <inheritdoc cref="UnlockTokenType.Killed"/>
        Killed = 8,
        /// <inheritdoc cref="UnlockTokenType.Impaled"/>
        Impaled = 9,
        /// <inheritdoc cref="UnlockTokenType.Stunned"/>
        Stunned = 10,
        /// <inheritdoc cref="UnlockTokenType.KilledPlayer"/>
        KilledPlayer = 11,
        /// <inheritdoc cref="UnlockTokenType.GrabbedPlayer"/>
        GrabbedPlayer = 12,
    }
    /// <summary>
    /// A class that represents an unlock token for a description module, this class represents an automated unlock token
    /// </summary>
    public class AutoModuleUnlockToken : BaseUnlockModule, IEquatable<AutoModuleUnlockToken>
    {
        /// <summary>
        /// The type of token this module unlock is for
        /// </summary>
        public readonly AutoTokenType TokenType = AutoTokenType.Unknown;

        /// <inheritdoc cref="BaseUnlockModule._value"/>
        public byte Value { get => _value; internal set => _value = value; }

        ///
        public AutoModuleUnlockToken(string creatureID, AutoTokenType tokenType, byte startingValue = 1) : base(creatureID, startingValue)
        {
            TokenType = tokenType;
        }

        /// <summary>
        /// Increments Value by one
        /// </summary>
        public static AutoModuleUnlockToken operator ++(AutoModuleUnlockToken module)
        {
            module._value++;
            return module;
        }
        /// <summary>
        /// Decrements Value by one
        /// </summary>
        public static AutoModuleUnlockToken operator --(AutoModuleUnlockToken module)
        {
            module._value--;
            return module;
        }

        /// <summary>
        /// Checks if both objects are <see cref="AutoModuleUnlockToken"/>, then compares them using <see cref="Equals(AutoModuleUnlockToken)"/>
        /// </summary>
        /// <remarks><see cref="Equals(AutoModuleUnlockToken)"/> <inheritdoc cref="Equals(AutoModuleUnlockToken)"/></remarks>
        public override bool Equals(object obj) => obj is AutoModuleUnlockToken other && Equals(other);
        /// <inheritdoc cref="BaseUnlockModule.Equals(BaseUnlockModule)"/>
        public bool Equals(AutoModuleUnlockToken other) => TokenType.Equals(other.TokenType) && base.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// The type of unlock this UnlockToken targets
    /// </summary>
    public enum UnlockTokenType : byte
    {
        ///
        Unknown = 0,
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
        /// When the creature impaled with a spear, by the player
        /// </summary>
        Impaled = 9,
        /// <summary>
        /// When the creature is stunned with a rock, by the player
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
    }
    /// <summary>
    /// An unlock token, that can be used to detect whether this module is unlocked
    /// </summary>
    public class UnlockToken : BaseUnlockModule, IEquatable<UnlockToken>, IEquatable<AutoModuleUnlockToken>, IEquatable<ModuleUnlockToken>
    {
        /// <summary>
        /// The type of token this module unlock targets
        /// </summary>
        [JsonProperty("token_type")]
        public readonly UnlockTokenType TokenType = UnlockTokenType.Unknown;

        /// <summary>
        /// The amount of times the token specified through <see cref="BaseUnlockModule.CreatureID"/> and <see cref="TokenType"/> should be registered before this token is valid
        /// </summary>
        [JsonProperty("value")]
        public byte Value { get => _value; }

        /// <param name="creatureID">The ID of the creature that to look for</param>
        /// <param name="tokenType">The type of token to look for</param>
        /// <param name="value">The amount of times this token should be registered before this is considered unlocked</param>
        public UnlockToken(string creatureID, UnlockTokenType tokenType, byte value = 1) : base(creatureID, value)
        {
            TokenType = tokenType;
        }

        /// <summary>
        /// Checks if both objects are <see cref="UnlockToken"/>, then compares them using <see cref="Equals(UnlockToken)"/>
        /// </summary>
        /// <remarks><see cref="Equals(UnlockToken)"/> <inheritdoc cref="Equals(UnlockToken)"/></remarks>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case UnlockToken token:
                    return Equals(token);
                case AutoModuleUnlockToken autoToken:
                    return Equals(autoToken);
                case ModuleUnlockToken moduleToken:
                    return Equals(moduleToken);
                default:
                    return false;
            }
        }

        /// <inheritdoc cref="BaseUnlockModule.Equals(BaseUnlockModule)"/>
        /// <summary>Checks if the tokenType matches then:</summary>
        public bool Equals(UnlockToken other) => TokenType.Equals(other.TokenType) && base.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Checks whether this UnlockToken matches the auto unlock token, by checking if the token type and ID matches, then checking whether <paramref name="other"/>'s value is greater than or equal to this value 
        /// </summary>
        public bool Equals(AutoModuleUnlockToken other) => ((byte)TokenType).Equals((byte)other.TokenType) && base.Equals(other);

        /// <inheritdoc cref="Equals(AutoModuleUnlockToken)"/>
        public bool Equals(ModuleUnlockToken other) => ((byte)TokenType).Equals((byte)other.TokenType) && base.Equals(other);
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
            Add(new EntriesTab(tabName, entries) { TitleImage = titleSprite }, merge);
        }
        /// <inheritdoc cref="Add(EntriesTab, bool)"/>
        public void Add(string tabName, IEnumerable<Entry> entries, ProcessManager.ProcessID menuProcessID, TitleSprite titleSprite = null, bool merge = false)
        {
            Add(new EntriesTab(tabName, entries) { TabMenuProcessID = menuProcessID, TitleImage = titleSprite }, merge);
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
        public TitleSprite TitleImage = null;

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
        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<Entry>)_entries).IsReadOnly;
        /// <summary>
        /// Adds a new entry to this tab
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Add(Entry item)
        {
            if (Contains(item.Name))
                throw new Exception("The entry with the name " + item.Name + " already exists in this tab!");

            _entries.Add(item);
        }
        /// <inheritdoc cref="Add(Entry)"/>
        public void Add(string entryName, EntryInfo info)
        {
            Add(new Entry(entryName, info));
        }
        /// <inheritdoc cref="Add(Entry)"/>
        public void Add(string entryName, string unlockID, string iconAtlasName, Description description)
        {
            Add(new Entry(entryName, new EntryInfo() { ID = unlockID, EntryIcon = iconAtlasName, Description = description }));
        }
        /// <inheritdoc cref="Add(Entry)"/>
        public void Add(string entryName, string unlockID, string iconAtlasName, string description)
        {
            Add(new Entry(entryName, new EntryInfo() { ID = unlockID, EntryIcon = iconAtlasName, Description = new Description(new DescriptionModule() { Body = description }) }));
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

            if (TitleImage == null)
                TitleImage = tab.TitleImage;

            if (TabMenuProcessID == Main.BestiaryTabMenu && tab.TabMenuProcessID != Main.BestiaryTabMenu)
                TabMenuProcessID = tab.TabMenuProcessID;
        }

        /// <inheritdoc/>
        public void Clear() => _entries.Clear();
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public void CopyTo(Entry[] array, int arrayIndex) => _entries.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(Entry item) => _entries.Remove(item);

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
    public class Entry
    {
        /// <summary>
        /// The name of this entry
        /// </summary>
        public string Name;
        /// <summary>
        /// The information of this entry, such as its unlock id, icon, scene to show while reading, and description
        /// </summary>
        public EntryInfo Info;

        /// <summary>
        /// The process ID that gets called when an entry button gets pressed, you can leave this as the default menu, or make a custom menu to display the entry's information.
        /// </summary>
        public ProcessManager.ProcessID EntryReadingMenu = Main.EntryReadingMenu;

        ///
        public Entry()
        {
            Name = "";
            Info = new EntryInfo();
        }
        ///
        public Entry(string name)
        {
            Name = name;
        }
        ///
        public Entry(string name, EntryInfo info)
        {
            Name = name;
            Info = info;
        }
        /// <param name="name">The name of the entry</param>
        /// <param name="description">The main body of this entry</param>
        /// <param name="unlockID">The ID that will be used to determine whether this entry is unlocked or not</param>
        /// <param name="iconAtlasName">The name of the entry's icon in the atlas manager</param>
        /// <param name="lockedText">The text that is shown when pressing on the entry while its locked</param>
        public Entry(string name, Description description, string unlockID = "", string iconAtlasName = "", string lockedText = "This entry is locked.")
        {
            Name = name;
            Info = new EntryInfo() { ID = unlockID, EntryIcon = iconAtlasName, Description = description, LockedText = lockedText };
        }
        /// <inheritdoc cref="Entry(string, Description, string, string, string)"/>
        public Entry(string name, string description, string unlockID = "", string iconAtlasName = "", string lockedText = "This entry is locked.")
        {
            Name = name;
            Info = new EntryInfo() { ID = unlockID, EntryIcon = iconAtlasName, LockedText = lockedText, Description = new Description(new DescriptionModule() { Body = description }) };
        }

        /// <summary>
        /// A default entry that's just an error, is always unlocked, and serves as a placeholder that appears when another entry cant be loaded
        /// </summary>
        public static Entry Error => new Entry("ERROR")
        {
            Info = new EntryInfo("Something went wrong with an entry, so this has been created as a warning.\nYou can check the log to see exactly what went wrong.\n", entryIcon: "illustrations\\error")
            {
                EntryUnlockedCondition = e => false,
                EntryColor = new HSLColor(0f, 0.8f, 0.6f)
            },
        };
    }

    /// <summary>
    /// The contents of the entry file
    /// </summary>
    public class EntryInfo
    {
        /// <summary>
        /// The ID of this entry, if the ID is found in the unlocked entries dictionary, this entry will be made visible
        /// </summary>
        [JsonProperty("unlock_id")]
        public string ID = string.Empty;

        /// <summary>
        /// The condition that specifies whether this entry is visible or not, if this returns true, then the entry is visible. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultEntryUnlockedCondition(EntryInfo)"/>, which checks if <see cref="Bestiary.UnlockedEntriesIDs"/> contains <see cref="ID"/></remarks>
        [JsonIgnore]
        public Func<EntryInfo, bool> EntryUnlockedCondition = DefaultEntryUnlockedCondition;
        /// <summary>
        /// Checks whether <see cref="ID"/> is found in <see cref="Bestiary.UnlockedEntriesIDs"/>
        /// </summary>
        /// <returns>True if the the entry is locked, otherwise false</returns>
        public static bool DefaultEntryUnlockedCondition(EntryInfo info) => BestiarySettings.Cheats.UnlockAllEntries.Value || Bestiary.UnlockedEntriesIDs.Contains(info.ID);

        /// <summary>
        /// Returns true if the entry is visible, else false
        /// </summary>
        [JsonIgnore]
        public bool EntryUnlocked => EntryUnlockedCondition == null || EntryUnlockedCondition(this);


        /// <summary>
        /// The text / tip that is shown when attempting to read the entry while its locked, this could be anything you want, leave blank for no message.
        /// </summary>
        [JsonProperty("locked_text")]
        public string LockedText = "This entry is locked.";


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
            set
            {
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
        public EntryInfo(Description description, string iD = "", string lockedText = "This entry is locked.", string entryIcon = "")
        {
            ID = iD;
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
        public EntryInfo(string description, string iD = "", string lockedText = "This entry is locked.", string entryIcon = "")
        {
            ID = iD;
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
        /// Returns this description as a string where only the parts of the entry that are visible are added
        /// </summary>
        public override string ToString()
        {
            string result = string.Empty;
            foreach (DescriptionModule module in _values)
            {
                if (module.ModuleUnlocked)
                {
                    result += "\n" + module;
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
        /// The ID of this description component, used to determine whether this part of the dictionary is visible or not
        /// </summary>
        [JsonProperty("unlock_id")]
        public UnlockToken UnlockID = null;

        /// <summary>
        /// The condition that specifies whether this entry is visible or not, if this returns true, then the entry is visible. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultModuleUnlockedCondition(DescriptionModule)"/></remarks>
        [JsonIgnore]
        public Func<DescriptionModule, bool> ModuleUnlockedCondition = DefaultModuleUnlockedCondition;

        /// <summary>
        /// Checks if <see cref="UnlockID"/> is found in <see cref="Bestiary.AutoModuleUnlocks"/> or <see cref="Bestiary.ModuleUnlocks"/> using <see cref="UnlockToken.Equals(AutoModuleUnlockToken)"/> and <see cref="UnlockToken.Equals(ModuleUnlockToken)"/>
        /// </summary>
        public static bool DefaultModuleUnlockedCondition(DescriptionModule info)
        {
            if (BestiarySettings.Cheats.UnlockAllEntries.Value)
                return true;

            return Bestiary._AutoModuleUnlocks.IsTokenValid(info.UnlockID) || Bestiary.ModuleUnlocks.IsTokenValid(info.UnlockID);
        }

        /// <summary>
        /// Returns true if the module is unlocked, else false
        /// </summary>
        [JsonIgnore]
        public bool ModuleUnlocked => ModuleUnlockedCondition == null || ModuleUnlockedCondition(this);

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
