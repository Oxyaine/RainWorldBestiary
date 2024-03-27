using System.Collections.Generic;
using System.IO;

namespace RainWorldBestiary
{
    internal class Font
    {
        public readonly string Name;

        private readonly float MinCharacterSize = 65f;

        public Font(string fontName, string fontConfigPath)
        {
            Name = fontName;

            string[] lines = File.ReadAllLines(fontConfigPath);
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                string[] splitLine = line.Split('=');
                Futile.atlasManager.LoadImage(splitLine[1]);
                if (Futile.atlasManager.DoesContainElementWithName(splitLine[1]))
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

                    if (BestiarySettings.MinimizeTitleSpacing.Value)
                        currentX += sprites[i].width + ((1f - (sprites[i].width / 40f)) * 20f);
                    else
                        currentX += MinCharacterSize;
                }
                else
                {
                    sprites[i] = new FSprite(ResourceManager.UnknownFontCharacter)
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
