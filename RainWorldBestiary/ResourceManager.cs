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
    }
}
