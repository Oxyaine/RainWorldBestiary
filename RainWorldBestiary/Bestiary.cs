using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RainWorldBestiary
{
    public static class Bestiary
    {
        public static bool IncludeDownpour = false;

        public static string[] GetAllEntriesNames()
        {
            string[] files = Directory.GetFiles(ResourceManager.EntriesPath, "*", SearchOption.AllDirectories);
            return files.Select(v => Path.GetFileNameWithoutExtension(v)).ToArray();
        }

        public static Entry GetEntryByName()
        {
            throw new NotImplementedException();
        }
    }

    public class Entry
    {
        public string Name = "";

    }
}
