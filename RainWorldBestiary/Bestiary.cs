using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RainWorldBestiary
{
    /// <summary>
    /// The main class for the bestiary, gives access to all the current entries, and lets you add your own
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

        internal static void Initialize()
        {
            ResourceManager.Initialize();

            IEnumerable<string> files = Directory.EnumerateFiles(ResourceManager.BaseEntriesPath, "*", SearchOption.AllDirectories);
            Entry[] entries = GetFilesAsEntries(files);

            EntriesTabs.Add(new EntriesTab("Rain World", entries) { TitleImage = new TitleSprite("illustrations\\Rain_World_Title") });

            files = Directory.EnumerateFiles(ResourceManager.DownpourEntriesPath, "*", SearchOption.AllDirectories);
            entries = GetFilesAsEntries(files);

            EntriesTabs.Add(new EntriesTab(DownpourTabName, entries) { TitleImage = new TitleSprite("illustrations\\Downpour_Title") });
        }
        internal static Entry[] GetFilesAsEntries(IEnumerable<string> files)
        {
            Entry[] entries = new Entry[files.Count()];
            int i = -1;
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                entries[++i] = new Entry(fileName, JsonConvert.DeserializeObject<EntryInfo>(File.ReadAllText(file)));
            }

            return entries;
        }


        /// <summary>
        /// All the ID's of entries that have been unlocked, and can be viewed (doesn't completely unlock entry, just makes it available, individual description components must still be unlocked individualy)
        /// </summary>
        public static List<string> UnlockedEntriesIDs = new List<string>();



        /// <summary>
        /// All the tabs, which hold all the entries, you can add your own, or add your entry to an existing tab
        /// </summary>
        public static EntriesTabList EntriesTabs = new EntriesTabList();
    }

    
    /// <summary>
    /// A class the represents an unlocked module that can be used by <see cref="DescriptionModule"/>, to check whether it can be made visible
    /// </summary>
    public class UnlockedDescriptionModuleID
    {
        /// <summary>
        /// The type of unlock
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Killing the creature
            /// </summary>
            /// <remarks>Useful for unlocking parts about what it takes to kill the creature, how many spears or even how many rocks, up to you</remarks>
            Killing,
            /// <summary>
            /// Taming the creature, only applicable to tamable creatures
            /// </summary>
            /// <remarks>Useful for unlocking parts about what is required to tame the creature / how much it eats before tamed</remarks>
            Taming,
            /// <summary>
            /// Evading the creature, by dodging, climbing to a place it can't reach, etc
            /// </summary>
            /// <remarks>For unlocking parts about techniques the player could use to evade the creature. Very similar to <see cref="ObserveHunted"/></remarks>
            Evading,
            /// <summary>
            /// Sneaking past the creature
            /// </summary>
            /// <remarks>For unlocking parts about the creatures vision range or how much it relies on sound, if at all</remarks>
            Sneaking,
            /// <summary>
            /// Stunning the creature with a rock, or other stunning object
            /// </summary>
            /// <remarks>For unlocking parts about what happens when a creature is stunned, such as that lizards flip over when stunned, or vultures are mostly* unaffected</remarks>
            Stunning,
            /// <summary>
            /// Hitting the creature with a damaging weapon, such as a spear
            /// </summary>
            /// <remarks>For unlocking parts about a creatures toughness and how it reacts to getting hit</remarks>
            Impaling,
            /// <summary>
            /// Looking at the creature / noticing the creature
            /// </summary>
            /// <remarks>For unlocking parts about the creatures appearance</remarks>
            Observing,
            /// <summary>
            /// Noticing the creatures fears
            /// </summary>
            /// <remarks>For unlocking parts about what the creature fears / how it can be scared away</remarks>
            ObserveFear,
            /// <summary>
            /// Noticing the creature eating
            /// </summary>
            /// <remarks>For unlocking parts about what the creature eats / can be distracted by (when it comes to food)</remarks>
            ObserveFood,
            /// <summary>
            /// Getting hunted by the creature / getting chased
            /// </summary>
            /// <remarks>For unlocking parts about abilities and hunting techniques the creature uses</remarks>
            ObserveHunted,
        }
    }


    /// <summary>
    /// A class that represents a list of <see cref="EntriesTab"/>
    /// </summary>
    /// <remarks>Not using regular list since this allows more control such as preventing two <see cref="EntriesTab"/> with the same name</remarks>
    public class EntriesTabList : IEnumerable<EntriesTab>, ICollection<EntriesTab>
    {
        readonly List<EntriesTab> _tabs = new List<EntriesTab>();

        /// <inheritdoc/>
        public int Count => _tabs.Count;
        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<EntriesTab>)_tabs).IsReadOnly;
        /// <inheritdoc/>
        public void Add(EntriesTab item)
        {
            foreach (EntriesTab tab in _tabs)
            {
                if (tab.Name.Equals(item.Name))
                    throw new Exception("A tab with the name " + item.Name + " already exists!");
            }

            _tabs.Add(item);
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
        public string ElementName = string.Empty;
        /// <summary>
        /// The scale of the image when drawn to the screen
        /// </summary>
        public float Scale = 1;
        /// <summary>
        /// The offset on the X axis from the default position
        /// </summary>
        public int XOffset = 0;
        /// <summary>
        /// THe offset on the Y axis from the default position
        /// </summary>
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
    public class EntriesTab : IEnumerable<Entry>, ICollection<Entry>
    {
        /// <summary>
        /// The name of this tab
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// The title image that gets displayed at the top when of the screen when viewing the tab, if set to null, or if the image isn't found, some generated text will be placed instead
        /// </summary>
        /// <remarks>By title, I mean the name of the entry that is visible at the top while reading the entry</remarks>
        public TitleSprite TitleImage = null;

        /// <summary>
        /// The process ID that gets called when a tab button gets pressed, you can leave this as the default menu, or make a custom menu to display entries.
        /// </summary>
        public ProcessManager.ProcessID EntriesTabMenu = Main.BestiaryTabMenu;

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
        /// <summary>
        /// Creates an <see cref="EntriesTab"/> with all entries in <paramref name="entries"/> as the entries
        /// </summary>
        public EntriesTab(string tabName, IEnumerable<Entry> entries)
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
        /// <inheritdoc/>
        public void Add(Entry item)
        {
            if (Contains(item.Name))
                throw new Exception("The entry with the name " + item.Name + " already exists in this tab!");

            _entries.Add(item);
        }
        /// <summary>
        /// Adds all the entries from the collection into this tab
        /// </summary>
        public void AddRange(IEnumerable<Entry> items)
        {
            foreach (Entry item in items)
            {
                Add(item);
            }
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
        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
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
        ///
        public Entry()
        {
            Name = "";
            Info = new EntryInfo();
        }
        ///
        public Entry(string name, EntryInfo info)
        {
            Name = name;
            Info = info;
        }
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
        /// The local path to the icon that is displayed on the button
        /// </summary>
        [JsonProperty("entry_icon")]
        public string EntryIcon = string.Empty;

        /// <summary>
        /// The title image that gets displayed at the top when of the screen while reading the entry, if set to null, or if the image isn't found, some generated text will be placed instead
        /// </summary>
        /// <remarks>By title, I mean the name of the entry that is visible at the top while reading the entry</remarks>
        [JsonProperty("title_sprite")]
        public TitleSprite TitleSprite = null;

        // Hopefully in the future!
        ///// <summary>
        ///// The scene that is shown while reading the entry
        ///// </summary>
        //[JsonProperty("scene_while_reading")]
        //public string SceneWhileReading = "";

        /// <summary>
        /// The description of this entry, when converted to string, only returns the parts of the entry that are visible
        /// </summary>
        [JsonProperty("description")]
        public Description Description = new Description();
    }

    /// <summary>
    /// A class representing an entries description, saved as a DescriptionModule array, but can be used as a string
    /// </summary>
    public class Description : IEnumerable<DescriptionModule>, ICollection<DescriptionModule>
    {
        /// <summary>
        /// Whether the entire description should be available to read, regardless of which module are locked or unlocked
        /// </summary>
        [JsonProperty("unlock_full_description")]
        public bool UnlockFullDescription = false;

        readonly List<DescriptionModule> _values = new List<DescriptionModule>();

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
                    result = string.Concat(result, module);
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
        public string Text = string.Empty;

        /// <inheritdoc/>
        public override string ToString() => Text;
    }
}
