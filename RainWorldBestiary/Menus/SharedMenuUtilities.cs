using Menu;
using System;
using System.IO;
using UnityEngine;
using RainWorldBestiary.Types;
using RainWorldBestiary.Managers;

namespace RainWorldBestiary.Menus
{
    internal class SharedMenuUtilities
    {
        public static FSprite[] GetMenuTitle(string elementName, float elementScale, in Vector2 screenSize, Vector2 offset, string generatedTitleText, out float spriteWidth)
        {
            if (!string.IsNullOrEmpty(elementName) && Futile.atlasManager.DoesContainElementWithName(elementName))
            {
                FSprite sprite = new FSprite(elementName)
                {
                    scale = 0.3f * elementScale,
                    x = screenSize.x / 2f + offset.x,
                    y = screenSize.y - 50f - offset.y
                };
                spriteWidth = sprite.width;
                return new FSprite[1] { sprite };
            }
            else
                return GetGeneratedTitle(generatedTitleText, screenSize, out spriteWidth);
        }
        public static FSprite[] GetGeneratedTitle(string text, in Vector2 screenSize, out float spriteWidth)
        {
            GeneratedFontText fontText = ResourceManager.GetCustomFontByName("rodondo").Generate(text);
            fontText.X = (screenSize.x / 2f) - (fontText.TotalWidth / 2f);
            fontText.Y = screenSize.y - 50f;

            spriteWidth = fontText.TotalWidth;
            return fontText.Finalize();
        }

        public static FSprite[] GetMenuTitle(EntriesTab tab, in Vector2 screenSize)
        {
            if (tab.TitleSprite == null)
                return GetGeneratedTitle(Translator.Translate(tab.Name), in screenSize, out _);

            return GetMenuTitle(tab.TitleSprite.ElementName, tab.TitleSprite.Scale, in screenSize, new Vector2(tab.TitleSprite.XOffset, tab.TitleSprite.YOffset), Translator.Translate(tab.Name), out _);
        }
        public static FSprite[] GetMenuTitle(Entry entry, in Vector2 screenSize, out float spriteWidth)
        {
            if (entry.Info.TitleSprite == null)
                return GetGeneratedTitle(Translator.Translate(entry.Name), in screenSize, out spriteWidth);

            return GetMenuTitle(entry.Info.TitleSprite.ElementName, entry.Info.TitleSprite.Scale, in screenSize, new Vector2(entry.Info.TitleSprite.XOffset, entry.Info.TitleSprite.YOffset), Translator.Translate(entry.Name), out spriteWidth);
        }



        [Obsolete]
        public static void AddMenuTitleIllustration(ref Menu.Menu menu, ref MenuObject owner, string elementName, float elementScale, in Vector2 screenSize, Vector2 offset, string generatedTitleText, out float spriteWidth)
        {
            if (!string.IsNullOrEmpty(elementName) && Futile.atlasManager.DoesContainElementWithName(elementName))
            {
                MenuIllustration menuIllustration = new MenuIllustration(menu, owner, Path.GetDirectoryName(elementName), Path.GetFileName(elementName), new Vector2(screenSize.x / 2f + offset.x, screenSize.y - 50f - offset.y), false, true)
                {
                    color = new Color(162f, 157f, 170f),
                    sprite =
                {
                     scale = 0.3f * elementScale
                }
                };
                spriteWidth = menuIllustration.sprite.width;
                owner.subObjects.Add(menuIllustration);
            }
            else
                GenerateTitleIllustration(ref menu, ref owner, generatedTitleText, screenSize, out spriteWidth);
        }
        [Obsolete]
        public static void GenerateTitleIllustration(ref Menu.Menu menu, ref MenuObject owner, string text, in Vector2 screenSize, out float spriteWidth)
        {
            GeneratedFontTextIllustrations fontText = ResourceManager.GetCustomFontByName("rodondo").GenerateIllustrations(ref menu, ref owner, text);
            fontText.X = (screenSize.x / 2f) - (fontText.TotalWidth / 2f);
            fontText.Y = screenSize.y - 50f;

            spriteWidth = fontText.TotalWidth;

            owner.subObjects.AddRange(fontText.Finalize());
        }

        [Obsolete]
        public static void AddMenuTitleIllustration(ref Menu.Menu menu, ref MenuObject owner, EntriesTab tab, in Vector2 screenSize)
        {
            if (tab.TitleSprite == null)
            {
                GenerateTitleIllustration(ref menu, ref owner, Translator.Translate(tab.Name), in screenSize, out _);
                return;
            }

            AddMenuTitleIllustration(ref menu, ref owner, tab.TitleSprite.ElementName, tab.TitleSprite.Scale, in screenSize, new Vector2(tab.TitleSprite.XOffset, tab.TitleSprite.YOffset), Translator.Translate(tab.Name), out _);
        }
        [Obsolete]
        public static void AddMenuTitleIllustration(ref Menu.Menu menu, ref MenuObject owner, Entry entry, in Vector2 screenSize, out float spriteWidth)
        {
            if (entry.Info.TitleSprite == null)
            {
                GenerateTitleIllustration(ref menu, ref owner, Translator.Translate(entry.Name), in screenSize, out spriteWidth);
                return;
            }

            AddMenuTitleIllustration(ref menu, ref owner, entry.Info.TitleSprite.ElementName, entry.Info.TitleSprite.Scale, in screenSize, new Vector2(entry.Info.TitleSprite.XOffset, entry.Info.TitleSprite.YOffset), Translator.Translate(entry.Name), out spriteWidth);
        }
    }
}
