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

        internal static readonly List<Font> CustomFonts = new List<Font>();
        internal static bool GetCustomFontByName(string fontName, out Font result, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            foreach (Font font in CustomFonts)
            {
                if (font.FontName.Equals(fontName, comparisonType))
                {
                    result = font;
                    return true;
                }
            }

            result = null;
            return false;
        }

        internal static void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;

                ModDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                int removeLength = ModDirectory.Length + 1;

                string illustrationsPath = Path.Combine(ModDirectory, "illustrations");
                string[] images = Directory.GetFiles(illustrationsPath);
                foreach (string image in images)
                {
                    string tmp = image.Substring(removeLength);
                    Futile.atlasManager.LoadImage(tmp.Substring(0, tmp.Length - 4));
                }

                IEnumerable<string> fonts = Directory.GetDirectories(Path.Combine(ModDirectory, "fonts"), "*", SearchOption.TopDirectoryOnly);
                foreach (string font in fonts)
                {
                    string configFile = font + ".txt";
                    if (File.Exists(configFile))
                    {
                        IEnumerable<string> files = Directory.GetFiles(font, "*", SearchOption.AllDirectories);
                        foreach (string file in files)
                        {
                            string tmp = file.Substring(removeLength);
                            Futile.atlasManager.LoadImage(tmp.Substring(0, tmp.Length - 4));
                        }

                        CustomFonts.Add(new Font(Path.GetFileName(font), configFile));
                    }
                }
            }
        }
    }

    internal class Font
    {
        public readonly string FontName;

        public Font(string fontName, string fontConfigPath)
        {
            FontName = fontName;

            string[] lines = File.ReadAllLines(fontConfigPath);
            foreach (string line in lines)
            {
                string[] splitLine = line.Split('=');
                FontCharacters.Add(splitLine[0][0], splitLine[1]);
            }
        }

        readonly Dictionary<char, string> FontCharacters = new Dictionary<char, string>();

        public GeneratedFontText Generate(string text)
        {
            int textLength = text.Length;
            FSprite[] sprites = new FSprite[textLength];

            float currentX = 0;
            for (int i = 0; i < textLength; i++)
            {
                if (FontCharacters.TryGetValue(char.ToLower(text[i]), out string atlasName))
                {
                    sprites[i] = new FSprite(atlasName)
                    {
                        x = currentX
                    };

                    currentX += sprites[i].width + ((1f - (sprites[i].width / 65f)) * 20f);
                }
                else
                {
                    sprites[i] = new FSprite(FontCharacters[' '])
                    {
                        x = currentX
                    };

                    currentX += sprites[i].width;
                }
            }

            return new GeneratedFontText(sprites, currentX);
        }
    }

    internal class GeneratedFontText
    {
        public float X = 0, Y = 0;
        public float Scale = 1;

        public readonly float TotalWidth;
        readonly FSprite[] Sprites;

        public GeneratedFontText(FSprite[] sprites, float totalWidth)
        {
            Sprites = sprites;
            TotalWidth = totalWidth;
        }

        public FSprite[] Finalize()
        {
            for (int i = 0; i < Sprites.Length; i++)
            {
                Sprites[i].x += X;
                Sprites[i].y += Y;
                Sprites[i].scale *= Scale;
            }

            return Sprites;
        }
    }
}
