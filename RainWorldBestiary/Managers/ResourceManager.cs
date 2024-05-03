using RainWorldBestiary.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RainWorldBestiary.Plugins
{
    internal static class ResourceManager
    {
        private static bool Initialized = false;

        private static void GetAllFonts(string ModDirectory)
        {
            List<Font> customFonts = new List<Font>();

            string[] fonts = Directory.GetDirectories(Path.Combine(ModDirectory, "fonts"), "*", SearchOption.TopDirectoryOnly);
            foreach (string font in fonts)
            {
                string configFile = font + "_" + Main.CurrentLanguage.value + ".txt";

                if (File.Exists(configFile))
                    customFonts.Add(new Font(Path.GetFileName(font), configFile, ModDirectory));
                else
                    customFonts.Add(new Font(Path.GetFileName(font)));
            }

            CustomFonts = CustomFonts.Concat(customFonts).ToArray();
        }
        internal static void ReloadFonts()
        {
            CustomFonts = new Font[0];
            GetAllFonts(Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
        }
        private static Font[] CustomFonts = new Font[0];
        internal static Font GetCustomFontByName(string fontName)
        {
            foreach (Font font in CustomFonts)
            {
                if (font.Name.Equals(fontName))
                {
                    return font;
                }
            }

            return null;
        }
        internal static bool TryGetCustomFontByName(string fontName, out Font result)
        {
            Font f = GetCustomFontByName(fontName);
            result = f;

            if (f == null)
                return false;

            return true;
        }

        internal static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;

                string ModDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                GetAllSprites(ModDirectory);
                GetAllFonts(ModDirectory);
            }
        }

        private static void GetAllSprites(string ModDirectory)
        {
            int removeLength = ModDirectory.Length + 1;
            try
            {
                string illustrationsPath = Path.Combine(ModDirectory, "illustrations");
                string[] images = Directory.GetFiles(illustrationsPath, "*.png", SearchOption.AllDirectories);
                foreach (string image in images)
                {
                    string imagePath = image.Substring(removeLength);
                    Futile.atlasManager.LoadImage(imagePath.Substring(0, imagePath.Length - 4));
                }
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex);
            }
        }
    }
}
