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
        internal static string[] GetAllEntriesNames()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(ResourceManager.BaseEntriesPath, "*", SearchOption.AllDirectories);
            if (IncludeDownpour)
                files = files.Concat(Directory.EnumerateFiles(ResourceManager.DownpourEntriesPath, "*", SearchOption.AllDirectories));
            List<string> entries = new List<string>();
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                entries.Add(fileName);
            }

            return entries.ToArray();
        }
        internal static Entry GetEntryByName()
        {
            throw new NotImplementedException();
        }
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
    public class Description : IEnumerable<DescriptionModule>
    {
        [JsonProperty("values")]
        DescriptionModule[] _values = { new DescriptionModule() };

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
