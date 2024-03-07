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

            EntriesTabs.Add(new EntriesTab("Rain World", entries) { TitleImage = new TabTitleImage("illustrations\\Rain_World_Title") });

            files = Directory.EnumerateFiles(ResourceManager.DownpourEntriesPath, "*", SearchOption.AllDirectories);
            entries = GetFilesAsEntries(files);

            EntriesTabs.Add(new EntriesTab(DownpourTabName, entries) { TitleImage = new TabTitleImage("illustrations\\Downpour_Title") });
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
        /// All the tabs, which hold all the entries, you can add your own, or add your entry to an existing tab
        /// </summary>
        public static EntriesTabList EntriesTabs = new EntriesTabList();
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
        public string UnlockID = "";

        /// <summary>
        /// The local path to the icon that is displayed on the button
        /// </summary>
        [JsonProperty("entry_icon")]
        public string EntryIcon = "";

        // Hopefully in the future!
        //// <summary>
        //// The scene that is shown while reading the entry
        //// </summary>
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
            return "About 188,000,000 results\rGlobal web icon\rLingoJam\rhttps://lingojam.com/FancyTextGenerator\rFancy Text Generator (𝓬𝓸𝓹𝔂 𝖆𝖓𝖉 𝓹𝓪𝓼𝓽𝓮) ― LingoJam\rWeb ResultFancy Text Generator (𝓬𝓸𝓹𝔂 𝖆𝖓𝖉 𝓹𝓪𝓼𝓽𝓮) ― LingoJam. Check out this completely free (no sign-up) AI Chat! Generating fancy text.\r\rCool Text Fonts\rFont Changer Online\rStylish Text Generator\rFancy Letters\rFonts For Instagram\rCool Fancy Text Generator\rEXPLORE FURTHER\rGlobal web icon\rFont Generator - Cool Symbol\rcoolsymbol.com\rGlobal web icon\rʕ•́ᴥ•̀ʔっ♡ Cute and cool text symbols to copy paste\rtext-symbols.com\rGlobal web icon\rFancy Font ⚡😍 ⓐⓝⓓ 𝕾𝖙𝖞𝖑𝖎𝖘𝖍 ...\rfancy-font-generator.com\rGlobal web icon\rCool Text Fonts (𝓬𝓸𝓹𝔂 🅰🅽🅳 𝖕𝖆𝖘𝖙𝖊 ...\rlingojam.com\rGlobal web icon\rFancy Text Generators and Converters ♥ 𝐓𝐞𝐱𝐭 ...\rtextfancy.com\rGlobal web icon\rFancy Text Generator Font Generator 😎 Copy and Paste\rfancytextpro.com\rRecommended to you based on what's popular • Feedback\r \rGlobal web icon\rMessages by Google\rhttps://messages.google.com\rGoogle Messages - A simple, helpful text messaging app\rExplore this image\rWeb ResultGoogle Messages lets you text anyone from anywhere across devices, with suggested replies, to-dos, search, and group RCS chats. It also offers end-to-end encryption, real-time spam protection, and …\r\rGet Started With MessagesSend & Receive Text Messages in MessagesA Simple, Helpful Text Messaging APPHow to Turn Chat Features OffVoice\rEXPLORE FURTHER\rGlobal web icon\r‎Messages on the App Store\rapps.apple.com\rGlobal web icon\rGoogle Messages for web\rmessages.google.com\rRecommended to you based on what's popular • Feedback\rGlobal web icon\rTextNow\rhttps://www.textnow.com\rTextNow\rWeb ResultTextNow is the ultimate solution for free texting and calling with your own phone number. You can use the TextNow app over wifi or get the TextNow SIM card for …\r\rEXPLORE FURTHER\rGlobal web icon\rTextNow: Call + Text Unlimited - Apps on Google Play\rplay.google.com\rGlobal web icon\rFree Texting and Calling | TextFree\rtextfree.us\rRecommended to you based on what's popular • Feedback\rPeople also ask\rWhat is a text message used for?\rTexting is also used to communicate very brief messages, such as informing someone that you will be late or reminding a friend or colleague about a meeting. As with e-mail, informality and brevity have become an accepted part of text messaging.\rText messaging - Wikipedia\r\ren.wikipedia.org/wiki/Text_messaging\rWhat is the meaning of text?\rThe meaning of TEXT is the original words and form of a written or printed work. How to use text in a sentence.\rText Definition & M";

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
