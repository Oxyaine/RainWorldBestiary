using System.Collections.Generic;
using System.IO;
using Menu;

namespace RainWorldBestiary
{
    internal class Font
    {
        public readonly string Name;

        private readonly float MinCharacterSize = 65f;

        public const string UnknownFontCharacter = "illustrations\\bestiary\\icons\\Unknown_Character";

        public Font(string fontName)
        {
            Name = fontName;
        }
        public Font(string fontName, string fontConfigPath, string owningModDirectory)
        {
            Name = fontName;
            ReadFontFileLines(fontConfigPath, owningModDirectory);
        }

        public void Dispose()
        {
            foreach (string image in FontCharacters.Values)
            {
                Futile.atlasManager.UnloadImage(image);
            }
        }

        private void ReadFontFileLines(string filePath, string owningModDirectory)
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                string[] splitLine = line.Split('=');

                if (splitLine[0].Equals("inherit"))
                {
                    string inheritingFile = Path.Combine(owningModDirectory, splitLine[1]);
                    if (File.Exists(inheritingFile))
                        ReadFontFileLines(inheritingFile, owningModDirectory);
                }
                else
                {
                    Futile.atlasManager.LoadImage(splitLine[1]);
                    FontCharacters.Add(splitLine[0][0], splitLine[1]);
                }
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
                    sprites[i] = new FSprite(UnknownFontCharacter)
                    {
                        x = currentX
                    };

                    currentX += sprites[i].width;
                }
            }

            return new GeneratedFontText(sprites, currentX);
        }

        public GeneratedFontTextIllustrations GenerateIllustrations(ref Menu.Menu menu, ref MenuObject owner, string text)
        {
            int textLength = text.Length;
            MenuIllustration[] sprites = new MenuIllustration[textLength];

            float currentX = 0;
            for (int i = 0; i < textLength; i++)
            {
                if (FontCharacters.TryGetValue(char.ToLower(text[i]), out string atlasName))
                {
                    sprites[i] = new MenuIllustration(menu, owner, Path.GetDirectoryName(atlasName), Path.GetFileName(atlasName), new UnityEngine.Vector2(currentX, 0), false, true);

                    if (BestiarySettings.MinimizeTitleSpacing.Value)
                        currentX += sprites[i].sprite.width;
                    else
                        currentX += MinCharacterSize;
                }
                else
                {

                }
            }

            return new GeneratedFontTextIllustrations(sprites, currentX);
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
    internal class GeneratedFontTextIllustrations
    {
        public float X = 0, Y = 0;
        public float Scale = 1;

        public readonly float TotalWidth;
        readonly MenuIllustration[] Sprites;

        public GeneratedFontTextIllustrations(MenuIllustration[] sprites, float totalWidth)
        {
            Sprites = sprites;
            TotalWidth = totalWidth;
        }

        public MenuIllustration[] Finalize()
        {
            for (int i = 0; i < Sprites.Length; i++)
            {
                Sprites[i].pos.x += X;
                Sprites[i].pos.y += Y;
                Sprites[i].sprite.scale *= Scale;
            }

            return Sprites;
        }
    }
}
