using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

namespace RainWorldBestiary
{
    public static class Bestiary
    {
        public static bool IncludeDownpour = false;

        public static string[] GetAllEntriesNames()
        {
            IEnumerable<string> files = Directory.EnumerateDirectories(ResourceManager.BaseEntriesPath, "*", SearchOption.AllDirectories);
            if (IncludeDownpour)
                files = files.Concat(Directory.EnumerateDirectories(ResourceManager.DownpourEntriesPath, "*", SearchOption.AllDirectories));
            List<string> entries = new List<string>();
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (fileName.StartsWith("."))
                    continue;

                entries.Add(fileName);
            }

            return entries.ToArray();
        }

        public static Entry GetEntryByName()
        {
            throw new NotImplementedException();
        }
    }

    public class Entry
    {
        public string Name;
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

    public class Description : IEnumerable<DescriptionModule>
    {
        [JsonProperty("values")]
        DescriptionModule[] _values = new DescriptionModule[1];

        public IEnumerator<DescriptionModule> GetEnumerator() => _values.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        public override string ToString()
        {
            string result = "";
            foreach (DescriptionModule module in _values)
            {

            }
            return result;
        }
    }

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
