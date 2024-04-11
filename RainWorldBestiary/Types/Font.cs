using Menu;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RainWorldBestiary.Types
{
    internal class Font
    {
        public readonly string Name;

        public const string UnknownFontCharacterPath = "illustrations\\bestiary\\icons";
        public const string UnknownFontCharacterName = "Unknown_Character";

        public Font(string fontName)
        {
            Name = fontName;
        }
        public Font(string fontName, string fontConfigPath, string owningModDirectory)
        {
            Name = fontName;
            ReadFontFileLines(fontConfigPath, owningModDirectory);
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
                    FontCharacters.Add(splitLine[0][0], splitLine[1]);
                }
            }
        }
        readonly Dictionary<char, string> FontCharacters = new Dictionary<char, string>();
        public GeneratedFontText Generate(ref Menu.Menu menu, ref MenuObject owner, string text)
        {
            int textLength = text.Length;
            MenuIllustration[] sprites = new MenuIllustration[textLength];

            float currentX = 0;
            for (int i = 0; i < textLength; i++)
            {
                if (FontCharacters.TryGetValue(char.ToLower(text[i]), out string atlasName))
                {
                    sprites[i] = new MenuIllustration(menu, owner, Path.GetDirectoryName(atlasName), Path.GetFileName(atlasName), new Vector2(currentX, 0), false, true);

                    if (BestiarySettings.ConsistentTitleSpacing.Value)
                        currentX += 65f;
                    else
                        currentX += sprites[i].sprite.width + ((1f - (sprites[i].sprite.width / 40f)) * 20f);
                }
                else
                {
                    sprites[i] = new MenuIllustration(menu, owner, UnknownFontCharacterPath, UnknownFontCharacterName, new Vector2(currentX, 0), true, true);
                    currentX += sprites[i].sprite.width;
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
        readonly MenuIllustration[] Sprites;

        public GeneratedFontText(MenuIllustration[] sprites, float totalWidth)
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
