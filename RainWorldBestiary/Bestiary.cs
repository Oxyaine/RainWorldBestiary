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
        /// <summary>
        /// Whether to include downpour entries or not
        /// </summary>
        internal static bool IncludeDownpour = false;

        internal const string DownpourTabName = "Downpour";

        internal static void Initialize()
        {
            ResourceManager.Initialize();

            IEnumerable<string> files = Directory.EnumerateFiles(ResourceManager.BaseEntriesPath, "*", SearchOption.AllDirectories);
            Entry[] entries = GetFilesAsEntries(files);

            EntriesTabs.Add(new EntriesTab("Rain World", entries) { TitleImage = new TabTitleImage("illustrations\\Rain_World_Title") { Scale = 0.3f } });
            
            files = Directory.EnumerateFiles(ResourceManager.DownpourEntriesPath, "*", SearchOption.AllDirectories);
            entries = GetFilesAsEntries(files);

            EntriesTabs.Add(new EntriesTab(DownpourTabName, entries) { TitleImage = new TabTitleImage("illustrations\\Downpour_Title") { Scale = 0.3f } });
        }
        ///
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
        /// All the tabs, which hold all the entries, you can add your own, or add your entry to an existing tab
        /// </summary>
        public static List<EntriesTab> EntriesTabs = new List<EntriesTab>();
    }

    /// <summary>
    /// Represents an element in the atlas manager, but gives some more options to customize the scale and offset of the image from the default values
    /// </summary>
    public class TabTitleImage
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
        public TabTitleImage(string elementName)
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
        /// The image that gets displayed at the top when of the screen when viewing the tab, if left blank, some text will be placed instead
        /// </summary>
        public TabTitleImage TitleImage = null;
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
        public void Add(Entry item) => _entries.Add(item);
        /// <summary>
        /// Adds all the entries from the collection into this tab
        /// </summary>
        public void AddRange(IEnumerable<Entry> items) => _entries.AddRange(items);
        /// <inheritdoc/>
        public void Clear() => _entries.Clear();
        /// <inheritdoc/>
        public bool Contains(Entry item) => _entries.Contains(item);
        /// <inheritdoc/>
        public void CopyTo(Entry[] array, int arrayIndex) => _entries.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(Entry item) => _entries.Remove(item);

        /// <summary>
        /// Gets or sets an entry at the given index
        /// </summary>
        public Entry this[int index] { get => _entries[index]; set => _entries[index] = value; }

        /// <summary>
        /// Gets or sets an entry with the given name
        /// </summary>
        [Obsolete]
        public Entry this[string entryName]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {

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
        public string UnlockID = "";

        /// <summary>
        /// The local path to the icon that is displayed on the button
        /// </summary>
        [JsonProperty("entry_icon")]
        public string EntryIcon = "";
        /// <summary>
        /// The scene that is shown while reading the entry
        /// </summary>
        [JsonProperty("scene_while_reading")]
        public string SceneWhileReading = "";

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
        [JsonProperty("values")]
        readonly List<DescriptionModule> _values = new List<DescriptionModule>() { new DescriptionModule() };

        /// <inheritdoc/>
        public int Count => _values.Count;
        /// <inheritdoc/>
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
            string result = "";
            foreach (DescriptionModule module in _values)
            {

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
        public string UnlockID = "";

        /// <summary>
        /// The text of this part of the entries description
        /// </summary>
        [JsonProperty("text")]
        public string Text = "";
    }
}
