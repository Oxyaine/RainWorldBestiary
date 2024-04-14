using Menu;
using RainWorldBestiary.Managers;
using RainWorldBestiary.Types;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal static class SharedMenuUtilities
    {
        public static void AddMenuTitleIllustration(Menu.Menu menu, MenuObject owner, string elementName, float elementScale, in Vector2 screenSize, Vector2 offset, string generatedTitleText, out float spriteWidth)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                MenuIllustration menuIllustration = new MenuIllustration(menu, owner, Path.GetDirectoryName(elementName), Path.GetFileName(elementName), new Vector2(screenSize.x / 2f + offset.x, screenSize.y - 50f - offset.y), false, true)
                {
                    sprite =
                {
                     scale = 0.3f * elementScale
                }
                };
                spriteWidth = menuIllustration.sprite.width;
                owner.subObjects.Add(menuIllustration);
            }
            else
                GenerateTitleIllustration(menu, owner, generatedTitleText, screenSize, out spriteWidth);
        }
        public static void GenerateTitleIllustration(Menu.Menu menu, MenuObject owner, string text, in Vector2 screenSize, out float spriteWidth)
        {
            GeneratedFontText fontText = ResourceManager.GetCustomFontByName("rodondo").Generate(ref menu, ref owner, text);
            fontText.X = (screenSize.x / 2f) - (fontText.TotalWidth / 2f);
            fontText.Y = screenSize.y - 50f;

            spriteWidth = fontText.TotalWidth;

            owner.subObjects.AddRange(fontText.Finalize());
        }
        public static void AddMenuTitleIllustration(Menu.Menu menu, MenuObject owner, EntriesTab tab, in Vector2 screenSize)
        {
            if (tab.TitleSprite == null)
            {
                GenerateTitleIllustration(menu, owner, Translator.Translate(tab.Name), in screenSize, out _);
                return;
            }

            AddMenuTitleIllustration(menu, owner, tab.TitleSprite.ElementName, tab.TitleSprite.Scale, in screenSize, new Vector2(tab.TitleSprite.XOffset, tab.TitleSprite.YOffset), Translator.Translate(tab.Name), out _);
        }
        public static void AddMenuTitleIllustration(Menu.Menu menu, MenuObject owner, Entry entry, in Vector2 screenSize, out float spriteWidth)
        {
            if (entry.Info.TitleSprite == null)
            {
                GenerateTitleIllustration(menu, owner, Translator.Translate(entry.Name), in screenSize, out spriteWidth);
                return;
            }

            AddMenuTitleIllustration(menu, owner, entry.Info.TitleSprite.ElementName, entry.Info.TitleSprite.Scale, in screenSize, new Vector2(entry.Info.TitleSprite.XOffset, entry.Info.TitleSprite.YOffset), Translator.Translate(entry.Name), out spriteWidth);
        }


        public static MenuIllustration[] GetMenuTitleIllustration(Menu.Menu menu, MenuObject owner, string elementName, float elementScale, in Vector2 screenSize, Vector2 offset, string generatedTitleText, out float spriteWidth)
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                MenuIllustration menuIllustration = new MenuIllustration(menu, owner, Path.GetDirectoryName(elementName), Path.GetFileName(elementName), new Vector2(screenSize.x / 2f + offset.x, screenSize.y - 50f - offset.y), false, true)
                {
                    sprite =
                {
                     scale = 0.3f * elementScale
                }
                };
                spriteWidth = menuIllustration.sprite.width;
                return new[] { menuIllustration };
            }
            else
                return GetGeneratedTitleIllustration(menu, owner, generatedTitleText, screenSize, out spriteWidth);
        }
        public static MenuIllustration[] GetGeneratedTitleIllustration(Menu.Menu menu, MenuObject owner, string text, in Vector2 screenSize, out float spriteWidth)
        {
            GeneratedFontText fontText = ResourceManager.GetCustomFontByName("rodondo").Generate(ref menu, ref owner, text);
            fontText.X = (screenSize.x / 2f) - (fontText.TotalWidth / 2f);
            fontText.Y = screenSize.y - 50f;

            spriteWidth = fontText.TotalWidth;

            return fontText.Finalize();
        }
        public static MenuIllustration[] GetMenuTitleIllustration(Menu.Menu menu, MenuObject owner, EntriesTab tab, in Vector2 screenSize)
        {
            if (tab.TitleSprite == null)
            {
                return GetGeneratedTitleIllustration(menu, owner, Translator.Translate(tab.Name), in screenSize, out _);
            }

            return GetMenuTitleIllustration(menu, owner, tab.TitleSprite.ElementName, tab.TitleSprite.Scale, in screenSize, new Vector2(tab.TitleSprite.XOffset, tab.TitleSprite.YOffset), Translator.Translate(tab.Name), out _);
        }
        public static MenuIllustration[] GetMenuTitleIllustration(Menu.Menu menu, MenuObject owner, Entry entry, in Vector2 screenSize, out float spriteWidth)
        {
            if (entry.Info.TitleSprite == null)
            {
                return GetGeneratedTitleIllustration(menu, owner, Translator.Translate(entry.Name), in screenSize, out spriteWidth);
            }

            return GetMenuTitleIllustration(menu, owner, entry.Info.TitleSprite.ElementName, entry.Info.TitleSprite.Scale, in screenSize, new Vector2(entry.Info.TitleSprite.XOffset, entry.Info.TitleSprite.YOffset), Translator.Translate(entry.Name), out spriteWidth);
        }


        public static IEnumerator AnimateTextSwitch(MenuLabel label, string newText)
        {
            if (label.text.Equals(newText))
                yield break;

            {
                string oldText = label.text;

                while (oldText.Length > 0)
                {
                    label.text = oldText = oldText.Substring(0, oldText.Length - 1);
                    yield return new WaitTime(0.01f);
                }
            }

            yield return new WaitTime(0.2f);

            int i = 0;
            while(i < newText.Length)
            {
                label.text += newText[i];
                ++i;

                yield return new WaitTime(0.01f);
            }
        }
    }
}
