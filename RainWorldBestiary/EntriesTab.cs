using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RainWorldBestiary
{
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

        [JsonIgnore]
        private readonly List<Entry> _entries = new List<Entry>();

        /// <summary>
        /// All the mods that are contributing to this tab, doesn't say which entries they contribute, just if they contribute
        /// </summary>
        [JsonIgnore]
        internal List<string> ContributingMods = new List<string>();

        /// <summary>
        /// Creates an empty <see cref="EntriesTab"/>
        /// </summary>
        [JsonConstructor]
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
        /// <exception cref="KeyNotFoundException"></exception>
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
    /// A class that represents a list of <see cref="EntriesTab"/>
    /// </summary>
    /// <remarks>Not using regular list since this allows more control such as preventing two <see cref="EntriesTab"/> with the same name</remarks>
    public class EntriesTabList : IEnumerable<EntriesTab>, ICollection<EntriesTab>
    {
        ///
        public EntriesTabList() { }
        /// <param name="tabs">The default tabs to add to this tab list</param>
        public EntriesTabList(params EntriesTab[] tabs)
        {
            _tabs = tabs.ToList();
        }

        // All the tabs of this entries tabs list
        private readonly List<EntriesTab> _tabs = new List<EntriesTab>();

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
        /// <summary>
        /// Gets a tab using its name
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        public EntriesTab this[string tabName]
        {
            get
            {
                foreach (EntriesTab tab in _tabs)
                {
                    if (tab.Name.Equals(tabName))
                    {
                        return tab;
                    }
                }

                throw new KeyNotFoundException("The entry with the name " + tabName + " was not found in the list.");
            }
        }
    }
}
