using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RainWorldBestiary
{
    /// <summary>
    /// The main class for the bestiary, holds all the bestiary entries
    /// </summary>
    public static class Bestiary
    {
        public static BasicEntryInfo[] GetAllEntriesBasicInfo()
        {
            throw new NotImplementedException();
        }

        public static Entry GetEntryByName()
        {
            throw new NotImplementedException();
        }
    }

    public class BasicEntryInfo
    {
        public string FullPath = "";
        public string Name = "";
    }

    public class Entry
    {
        public string Name = "";

    }
}
