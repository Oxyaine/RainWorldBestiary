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
        /// Whether downpour entries are shown or not, true if more slug cats is enables, otherwise false
        /// </summary>
        public static bool IncludeDownpourEntries => IncludeDownpour;
        /// <summary>
        /// Whether the UnlockAllEntries cheat in the remix menu is enabled
        /// </summary>
        public static bool UnlockAllEntries => Main.Options.UnlockAllEntries.Value;

        internal const string DownpourTabName = "Downpour";

        /// <summary>
        /// All the ID's of entries that have been unlocked, and can be viewed (doesn't completely unlock entry, just makes it available, individual description components must still be unlocked individually)
        /// </summary>
        public static List<string> UnlockedEntriesIDs = new List<string>();

        private readonly static List<AutoModuleUnlockToken> _AutoModuleUnlocks = new List<AutoModuleUnlockToken>();
        /// <summary>
        /// All the module unlock tokens that are automatically tallied and registered, you can check <see cref="AutoTokenType"/> to see what is automatically detected
        /// </summary>
        public static List<AutoModuleUnlockToken> AutoModuleUnlocks => _AutoModuleUnlocks;

        /// <summary>
        /// All the module unlock tokens that are manually registered, you can check <see cref="TokenType"/> to see what is manually detected
        /// </summary>
        public static List<ModuleUnlockToken> ModuleUnlocks = new List<ModuleUnlockToken>();

        /// <summary>
        /// All the tabs, which hold all the entries, you can add your own, or add your entry to an existing tab
        /// </summary>
        public static EntriesTabList EntriesTabs = new EntriesTabList();
    }


    /// <summary>
    /// A base unlock module, holds shared unlock module logic
    /// </summary>
    public abstract class BaseUnlockModule : IEquatable<BaseUnlockModule>
    {
        /// <summary>
        /// The ID of the creature this unlock token applies to
        /// </summary>
        public readonly string CreatureID = string.Empty;
        /// <summary>
        /// The amount of times the unlock token was registered, caps at 255
        /// </summary>
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
        Unknown,
        /// <summary>
        /// For when the player tames the creature
        /// </summary>
        Taming,
        /// <summary>
        /// For when the player evades the creature, by dodging an attack, climbing to a place it can't reach, etc
        /// </summary>
        Evading,
        /// <summary>
        /// For when the player sneaks past a creature
        /// </summary>
        Sneaking,
        /// <summary>
        /// For when the player sees the creature
        /// </summary>
        Observing,
        /// <summary>
        /// For when the player sees the creature run away in fear
        /// </summary>
        ObserveFear,
        /// <summary>
        /// For when the player sees the creature eating or hunting another creature
        /// </summary>
        ObserveFood,
        /// <summary>
        /// For when the player is getting chased by a creature
        /// </summary>
        ObserveHunted,
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
        Unknown,
        /// <summary>
        /// When a creature is killed by the player
        /// </summary>
        Killing,
        /// <summary>
        /// When a creature impaled with a spear, by the player
        /// </summary>
        Impaling,
        /// <summary>
        /// When a creature is stunned with a rock, by the player
        /// </summary>
        Stunning,
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
        /// Adds a new tab that can be filled with entries
        /// </summary>
        public void Add(EntriesTab item)
        {
            foreach (EntriesTab tab in _tabs)
            {
                if (tab.Name.Equals(item.Name))
                    throw new Exception("A tab with the name " + item.Name + " already exists!");
            }

            _tabs.Add(item);
        }
        /// <inheritdoc cref="Add(EntriesTab)"/>
        public void Add(string tabName, params Entry[] entries)
        {
            Add(new EntriesTab(tabName, entries));
        }
        /// <inheritdoc cref="Add(EntriesTab)"/>
        public void Add(string tabName, IEnumerable<Entry> entries, TitleSprite titleSprite = null)
        {
            Add(new EntriesTab(tabName, entries) { TitleImage = titleSprite });
        }
        /// <inheritdoc cref="Add(EntriesTab)"/>
        public void Add(string tabName, IEnumerable<Entry> entries, ProcessManager.ProcessID menuProcessID, TitleSprite titleSprite = null)
        {
            Add(new EntriesTab(tabName, entries) { TabMenuProcessID = menuProcessID, TitleImage = titleSprite });
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

        [JsonProperty("tab_menu_process_id", DefaultValueHandling = DefaultValueHandling.Populate)]
        private string MenuProcessID
        {
            get => TabMenuProcessID.value;
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
            Info = new EntryInfo("Something went wrong with an entry, so this has been created as a warning.\nYou can check the log to see exactly what went wrong.\n", entryIcon: "illustrations\\error") { EntryLockedCondition = e => false }
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
        /// The condition that specifies whether this entry is locked or not, if this returns true, then the entry is locked. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultEntryLockedCondition(EntryInfo)"/>, which checks if <see cref="Bestiary.UnlockedEntriesIDs"/> contains <see cref="ID"/></remarks>
        [JsonIgnore]
        public Func<EntryInfo, bool> EntryLockedCondition = DefaultEntryLockedCondition;
        /// <summary>
        /// Checks whether <see cref="ID"/> is found in <see cref="Bestiary.UnlockedEntriesIDs"/>
        /// </summary>
        /// <returns>True if the the entry is locked, otherwise false</returns>
        public static bool DefaultEntryLockedCondition(EntryInfo info) => !Main.Options.UnlockAllEntries.Value && !Bestiary.UnlockedEntriesIDs.Contains(info.ID);

        /// <summary>
        /// Returns false if <see cref="EntryLockedCondition"/> is null, or if it returns false, otherwise true
        /// </summary>
        [JsonIgnore]
        public bool EntryLocked => EntryLockedCondition != null && EntryLockedCondition(this);


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
        public string EntryIcon = string.Empty;

        /// <summary>
        /// The title image that gets displayed at the top when of the screen while reading the entry, if set to null, or if the image isn't found, some generated text will be placed instead
        /// </summary>
        /// <remarks>By title, I mean the name of the entry that is visible at the top while reading the entry</remarks>
        [JsonProperty("title_sprite")]
        public TitleSprite TitleSprite = null;

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
                    if (!module.ModuleLocked)
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
                if (!module.ModuleLocked)
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
        public string ID = string.Empty;

        /// <summary>
        /// The condition that specifies whether this entry is locked or not, if this returns true, then the entry is locked. You can leave this as the default, or set your own custom condition.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultModuleLockedCondition(DescriptionModule)"/></remarks>
        [JsonIgnore]
        public Func<DescriptionModule, bool> ModuleLockedCondition = DefaultModuleLockedCondition;

        public static bool DefaultModuleLockedCondition(DescriptionModule info) => false;

        /// <summary>
        /// Returns false if <see cref="ModuleLockedCondition"/> is null, or if it returns false, otherwise true
        /// </summary>
        [JsonIgnore]
        public bool ModuleLocked => ModuleLockedCondition != null && ModuleLockedCondition(this);

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
