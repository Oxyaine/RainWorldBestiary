using Menu;
using RainWorldBestiary.Plugins;
using RainWorldBestiary.Types;
using System.Collections;
using System.IO;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal static class SharedMenuUtilities
    {
        public static MenuIllustration[] GetMenuTitleIllustration(Menu.Menu menu, MenuObject owner, TitleSprite sprite, in Vector2 screenSize, string generatedTitleText, out float spriteWidth)
        {
            if (!string.IsNullOrEmpty(sprite.ElementName))
            {
                MenuIllustration menuIllustration = new MenuIllustration(menu, owner, Path.GetDirectoryName(sprite.ElementName), Path.GetFileName(sprite.ElementName), new Vector2(screenSize.x / 2f + sprite.XOffset, screenSize.y - 50f - sprite.YOffset), false, true)
                {
                    sprite =
                {
                     scale = 0.3f * sprite.Scale
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

            return GetMenuTitleIllustration(menu, owner, tab.TitleSprite, in screenSize, Translator.Translate(tab.Name), out _);
        }
        public static MenuIllustration[] GetMenuTitleIllustration(Menu.Menu menu, MenuObject owner, Entry entry, in Vector2 screenSize, out float spriteWidth)
        {
            if (entry.Info.TitleSprite == null)
            {
                return GetGeneratedTitleIllustration(menu, owner, Translator.Translate(entry.Name), in screenSize, out spriteWidth);
            }

            return GetMenuTitleIllustration(menu, owner, entry.Info.TitleSprite, in screenSize, Translator.Translate(entry.Name), out spriteWidth);
        }


        public static IEnumerator AnimateTextSwitch(MenuLabel label, string newText)
        {
            if (label.text.Equals(newText))
                yield break;

            IEnumerator enumerator = AnimateDeleteText(label);
            while (enumerator.MoveNext())
                yield return enumerator.Current;

            yield return new WaitTime(0.2f);

            enumerator = AnimatePrintText(label, newText);
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
        public static IEnumerator AnimateDeleteText(MenuLabel label)
        {
            string oldText = label.text;

            while (oldText.Length > 0)
            {
                label.text = oldText = oldText.Substring(0, oldText.Length - 1);
                yield return new WaitTime(0.01f);
            }
        }
        public static IEnumerator AnimatePrintText(MenuLabel label, string newText)
        {
            int i = 0;
            while (i < newText.Length)
            {
                label.text += newText[i];
                ++i;

                yield return new WaitTime(0.01f);
            }
        }
    }
}
