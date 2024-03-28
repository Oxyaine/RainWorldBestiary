using Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class EntryReadingMenu : Dialog
    {
#if DEBUG
        internal const string ENTRY_REFERENCE_ID = "Referenced_To;";
        readonly string ReturnButtonMessage = "RETURN";
#endif

        readonly int WrapCount = 180;

        readonly string BackButtonMessage = "BACK";
        public EntryReadingMenu(ProcessManager manager) : base(manager)
        {
            try
            {
                Vector2 screenSize = manager.rainWorld.options.ScreenSize;

                float leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;

                scene = new InteractiveMenuScene(this, pages[0], manager.rainWorld.options.SubBackground);
                pages[0].subObjects.Add(scene);

                darkSprite = new FSprite("pixel")
                {
                    color = new Color(0f, 0f, 0f),
                    anchorX = 0f,
                    anchorY = 0f,
                    scaleX = 1368f,
                    scaleY = 770f,
                    x = -1f,
                    y = -1f,
                    alpha = 0.90f
                };
                pages[0].Container.AddChild(darkSprite);

#if DEBUG
                if (Bestiary.PreviousEntriesChain.Count > 0)
                {
                    SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK TO PREVIOUS"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
                    pages[0].subObjects.Add(backButton);
                    backObject = backButton;
                    backButton.nextSelectable[0] = backButton;

                    SimpleButton returnButton = new SimpleButton(this, pages[0], Translator.Translate("RETURN TO ENTRIES"), ReturnButtonMessage, new Vector2(leftAnchor + 250f, 25f), new Vector2(220f, 30f));
                    pages[0].subObjects.Add(returnButton);
                }
                else
                {
                    SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
                    pages[0].subObjects.Add(backButton);
                    backObject = backButton;
                    backButton.nextSelectable[0] = backButton;
                }
#else
                SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
                pages[0].subObjects.Add(backButton);
                backObject = backButton;
                backButton.nextSelectable[0] = backButton;
#endif

                DisplayEntryInformation(Bestiary.CurrentSelectedEntry, in screenSize);

                mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex);
            }
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();

            if (routine != null)
                Main.StopCoroutinePtr(routine);
        }

        Coroutine routine = null;
        IEnumerator PerformTextAnimation(string text, Vector2 screenSize)
        {
            int characters = text.Length / 100;

            string[] elements = ResourceManager.Characters.GetRandom(characters);
            FSprite[] sprites = new FSprite[characters];

            int position = characters * 16 * 3;

            float currentX = (screenSize.x / 2f) - (position / 2f), currentY = screenSize.y - 175f;
            for (int i = 0; i < elements.Length; i++)
            {
                sprites[i] = new FSprite(elements[i])
                {
                    x = currentX,
                    y = currentY,
                    scale = 3f
                };

                pages[0].Container.AddChild(sprites[i]);

                currentX += sprites[i].width;

                yield return new WaitForSeconds(0.05f);
            }

            elements = null;

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(0.3f);
                for (int j = 0; j < sprites.Length; j++)
                    sprites[j].RemoveFromContainer();

                yield return new WaitForSeconds(0.2f);
                for (int j = 0; j < sprites.Length; j++)
                    pages[0].Container.AddChild(sprites[j]);
            }

            MenuLabel label = new MenuLabel(this, pages[0], "", new Vector2(screenSize.x / 2f, screenSize.y / 2f), Vector2.one, false);
            pages[0].subObjects.Add(label);

            const int IncreaseAmount = 10;
            int cache = text.Length,
                countBeforeIconRemoval = cache / characters,
                currentSpriteIndex = 0;
            for (int i = 0; i < cache; i += IncreaseAmount)
            {
                if (i + IncreaseAmount >= cache)
                    label.text += text.Substring(i);
                else
                    label.text += text.Substring(i, IncreaseAmount);

                if (i >= (countBeforeIconRemoval * currentSpriteIndex))
                {
                    Main.StartCoroutinePtr(FadeIconAnimation(sprites[currentSpriteIndex]));
                    ++currentSpriteIndex;
                }

                yield return new WaitForFixedUpdate();
            }
        }
        IEnumerator FadeIconAnimation(FSprite sprite)
        {
            PlaySound(SoundID.HUD_Food_Meter_Deplete_Plop_A);

            while (sprite.alpha > 0)
            {
                sprite.alpha -= Time.deltaTime * 2 / sprite.alpha;
                sprite.scale += 0.05f;

                yield return null;
            }

            sprite.RemoveFromContainer();
        }

        public void DisplayEntryInformation(Entry entry, in Vector2 screenSize)
        {
            float widthOffset, leftSpriteOffset = 60;

#if DEBUG
            try
            {
                EntryTextDisplay.CreateAndAdd(entry.Info.Description.ToString().WrapText(WrapCount), in screenSize, this, pages[0]);
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex.ToString());
            }
#else

            if (BestiarySettings.PerformTextAnimations.Value)
            {
                routine = Main.StartCoroutinePtr(PerformTextAnimation(entry.Info.Description.ToString().WrapText(WrapCount), screenSize));
            }
            else
            {
                MenuLabel label = new MenuLabel(this, pages[0], entry.Info.Description.ToString().WrapText(WrapCount), new Vector2(screenSize.x / 2f, screenSize.y / 2f), Vector2.one, false);
                pages[0].subObjects.Add(label);
            }
#endif

            if (entry.Info.TitleSprite != null && Futile.atlasManager.DoesContainElementWithName(entry.Info.TitleSprite.ElementName))
            {
                FSprite sprite = new FSprite(entry.Info.TitleSprite.ElementName)
                {
                    scale = 0.3f * entry.Info.TitleSprite.Scale,
                    x = screenSize.x / 2f + entry.Info.TitleSprite.XOffset,
                    y = screenSize.y - 50f - entry.Info.TitleSprite.YOffset
                };
                pages[0].Container.AddChild(sprite);

                widthOffset = sprite.width / 2f;
            }
            else
            {
                GeneratedFontText fontText = ResourceManager.GetCustomFontByName("rodondo").Generate(Translator.Translate(entry.Name));

                fontText.X = (screenSize.x / 2f) - (fontText.TotalWidth / 2f);
                fontText.Y = screenSize.y - 50f;

                FSprite[] sprites = fontText.Finalize();
                for (int i = 0; i < sprites.Length; i++)
                    pages[0].Container.AddChild(sprites[i]);

                widthOffset = fontText.TotalWidth / 2f;
            }

            if (entry.Info.IconsNextToTitle)
            {
                float iconOffset = 0;
                for (int i = 0; i < entry.Info.EntryIcons.Length; i++)
                {
                    if (entry.Info.IconsNextToTitle && Futile.atlasManager.DoesContainElementWithName(entry.Info.EntryIcons[i]))
                    {
                        FSprite sprite = new FSprite(entry.Info.EntryIcons[i])
                        {
                            y = screenSize.y - 50,
                            x = screenSize.x / 2f - (widthOffset + leftSpriteOffset) - iconOffset,
                            scale = 2
                        };
                        pages[0].Container.AddChild(sprite);

                        FSprite sprite2 = new FSprite(entry.Info.EntryIcons[i])
                        {
                            y = screenSize.y - 50,
                            x = screenSize.x / 2f + (widthOffset + 10) + iconOffset,
                            scale = 2
                        };
                        pages[0].Container.AddChild(sprite2);

                        iconOffset += sprite.width;
                    }
                }
            }

            if (BestiarySettings.ShowModuleLockPips.Value)
            {
                for (int i = 0; i < entry.Info.Description.Count; i++)
                {
                    FSprite pip = new FSprite(entry.Info.Description[i].ModuleUnlocked ? ResourceManager.UnlockPipUnlocked : ResourceManager.UnlockPip)
                    {
                        x = screenSize.x - 20f,
                        y = screenSize.y - (i * 10) - 20f,
                        scale = 1f
                    };
                    pages[0].Container.AddChild(pip);
                }
            }
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                Bestiary.EnteringMenu = false;
                PlaySound(SoundID.MENU_Switch_Page_Out);

#if DEBUG
                if (Bestiary.PreviousEntriesChain.Count > 0)
                {
                    Bestiary.CurrentSelectedEntry = Bestiary.PreviousEntriesChain[0];
                    Bestiary.PreviousEntriesChain.RemoveAt(0);

                    manager.RequestMainProcessSwitch(Main.EntryReadingMenu, BestiarySettings.MenuFadeTimeSeconds);
                }
                else
                    manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, BestiarySettings.MenuFadeTimeSeconds);
#else
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, BestiarySettings.MenuFadeTimeSeconds);
#endif
            }

#if DEBUG
            if (message.Equals(ReturnButtonMessage))
            {
                Bestiary.EnteringMenu = false;
                PlaySound(SoundID.MENU_Switch_Page_Out);

                Bestiary.PreviousEntriesChain.Clear();

                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, BestiarySettings.MenuFadeTimeSeconds);
            }

            if (message.StartsWith(ENTRY_REFERENCE_ID))
            {
                Bestiary.EnteringMenu = true;
                PlaySound(SoundID.MENU_Switch_Page_In);

                try
                {
                    string referenceID = message.Substring(message.IndexOf(';') + 1);
                    Entry entry = Bestiary.GetEntryByReferenceID(referenceID);

                    if (entry != null)
                    {
                        Bestiary.PreviousEntriesChain.Insert(0, Bestiary.CurrentSelectedEntry);
                        Bestiary.CurrentSelectedEntry = entry;
                        manager.RequestMainProcessSwitch(Main.EntryReadingMenu, BestiarySettings.MenuFadeTimeSeconds);
                    }
                }
                catch (Exception ex)
                {
                    Main.Logger.LogDebug(ex);
                }
            }
#endif
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Singal(backObject, BackButtonMessage);

            base.Update();
        }
    }
#if DEBUG
    internal class EntryTextDisplay
    {
        readonly int LineSpacing = 20;
        readonly List<MenuObject> _Objects = new List<MenuObject>();

        public EntryTextDisplay(string wrappedText, in Vector2 screenSize, in Menu.Menu menu, in MenuObject owner)
        {
            string[] split = wrappedText.Split('\n');
            int currentY = GetStartingYPosition(split.Length, (int)screenSize.y);

            foreach (string line in split)
            {
                //MenuLabel label = new MenuLabel(menu, owner, line, new Vector2(screenSize.x / 2, currentY), Vector2.one, false);
                //_Objects.Add(label);
                _Objects.AddRange(FormatHorizontalText(line, screenSize, currentY, menu, owner));

                currentY -= LineSpacing;
            }
        }

        enum StructureType
        {
            PlainText,
            Reference
        }

        // Current Valid Structures
        // References: <ref=World!=Rain World/Batfly>
        private List<MenuObject> FormatHorizontalText(string text, in Vector2 screenSize, in int Y, in Menu.Menu menu, in MenuObject owner)
        {
            List<float> sizes = new List<float>();

            int currentLookPosition = 0;
            int LessThanIndex;
            List<(StructureType type, string message, string otherData)> structures = new List<(StructureType type, string message, string otherData)>();
            while ((LessThanIndex = text.IndexOf('<', currentLookPosition)) != -1)
            {
                (StructureType type, string message, string otherData) structure = (StructureType.PlainText, text.Substring(currentLookPosition, LessThanIndex - currentLookPosition), string.Empty);
                sizes.Add(structure.message.Length);
                structures.Add(structure);

                structure = DecipherStructure(in text, LessThanIndex + 1, out currentLookPosition);
                sizes.Add(structure.message.Length);
                structures.Add(structure);
            }

            string remainder = text.Substring(currentLookPosition);
            structures.Add((StructureType.PlainText, remainder, string.Empty));
            sizes.Add(remainder.Length);

            List<MenuObject> result = new List<MenuObject>();

            float currentX = (screenSize.x - (sizes.Sum() * 5.3f)) / 2f;
            int currentSizeIndex = 0;
            foreach ((StructureType type, string message, string otherData) in structures)
            {
                if (type == StructureType.Reference)
                {
                    float xSize = sizes[currentSizeIndex] * 1.5f;
                    SimpleButton button = new SimpleButton(menu, owner, message, EntryReadingMenu.ENTRY_REFERENCE_ID + otherData, new Vector2(currentX - xSize + (currentSizeIndex * 10f), Y - 10f), new Vector2(sizes[currentSizeIndex] * 5.3f * 1.5f, 20f))
                    {
                        rectColor = new HSLColor(0f, 0f, 0f),
                        labelColor = new HSLColor(0f, 1f, 1f)
                    };
                    result.Add(button);
                }
                else
                {
                    MenuLabel label = new MenuLabel(menu, owner, message, new Vector2(currentX + (currentSizeIndex * 10f), Y), Vector2.one, false);
                    label.label.alignment = FLabelAlignment.Left;
                    result.Add(label);
                }

                currentX += sizes[currentSizeIndex] * 5.3f;
                currentSizeIndex++;
            }

            return result;
        }
        private static (StructureType type, string message, string otherData) DecipherStructure(in string text, int startingPosition, out int lastPosition)
        {
            lastPosition = text.IndexOf('>', startingPosition);
            string t = text.Substring(startingPosition, lastPosition - startingPosition);

            string[] split = t.Split('=');

            StructureType type;
            string message = split[1].Trim('\"');
            string otherData = string.Empty;
            switch (split[0])
            {
                case "ref":
                    type = StructureType.Reference;
                    break;
                default:
                    type = StructureType.PlainText;
                    break;
            }

            if (split.Length > 2)
                otherData = split[2];

            ++lastPosition;
            return (type, message, otherData);
        }

        private int GetStartingYPosition(int lines, int screenSizeY)
        {
            int totalHeight = lines * LineSpacing;
            int leftoverScreen = screenSizeY - totalHeight;
            return screenSizeY - (leftoverScreen / 2);
        }

        public void AddToPage(ref MenuObject page)
            => page.subObjects.AddRange(_Objects);

        public static void CreateAndAdd(string wrappedText, in Vector2 screenSize, Menu.Menu menu, MenuObject owner)
            => new EntryTextDisplay(wrappedText, in screenSize, in menu, in owner).AddToPage(ref owner);
    }
#endif
}
