using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RainWorldBestiary
{
    internal static class ResourceManager
    {
        static bool Initialized = false;

        static string ModDirectory = null;
        public static string EntriesPath => Path.Combine(ModDirectory, "entries");
        public static string BaseEntriesPath => Path.Combine(EntriesPath, "base");
        public static string DownpourEntriesPath => Path.Combine(EntriesPath, "downpour");

        internal static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;

                ModDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                string illustrationsPath = Path.Combine(ModDirectory, "illustrations");
                string[] images = Directory.GetFiles(illustrationsPath);
                int removeLength = ModDirectory.Length + 1;
                foreach (string image in images)
                {
                    string tmp = image.Substring(removeLength);
                    Futile.atlasManager.LoadImage(tmp.Substring(0, tmp.Length - 4));
                }
            }
        }
    }
}
