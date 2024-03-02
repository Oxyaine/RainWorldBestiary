using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RainWorldBestiary
{
    internal static class ResourceManager
    {
        static string ModDirectory = null;
        public static string EntriesPath => Path.Combine(ModDirectory, "entries");
        public static string BaseEntriesPath => Path.Combine(EntriesPath, "base");
        public static string DownpourEntriesPath => Path.Combine(EntriesPath, "downpour");

        public static void Initialize()
        {
            ModDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        public static string GetFileByName(string name)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(EntriesPath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileNameWithoutExtension(file).Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return file;
                }
            }

            return string.Empty;
        }
    }
}
