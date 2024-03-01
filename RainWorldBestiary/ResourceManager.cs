using System.IO;
using System.Reflection;

namespace RainWorldBestiary
{
    internal static class ResourceManager
    {
        static string WorkingDirectory = null;
        static string EntriesPath => Path.Combine(WorkingDirectory, "entries");

        public static void Initialize()
        {
            WorkingDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }
    }
}
