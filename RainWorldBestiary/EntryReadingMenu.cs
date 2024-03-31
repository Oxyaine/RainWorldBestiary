using Menu;
using System;
#if DEBUG
using System.Collections.Generic;
using System.Linq;
#else
using System.Collections;
#endif
using UnityEngine;

namespace RainWorldBestiary
{
    internal class EntryReadingMenu : Dialog
    {
#if DEBUG
        internal const string ENTRY_REFERENCE_ID = "Reference_To;";
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

                string backButtonText = "BACK";
                if (Bestiary.PreviousEntriesChain.Count > 0)
                {
                    SimpleButton returnButton = new SimpleButton(this, pages[0], Translator.Translate("RETURN TO ENTRIES"), ReturnButtonMessage, new Vector2(leftAnchor + 250f, 25f), new Vector2(220f, 30f));
                    pages[0].subObjects.Add(returnButton);

                    backButtonText = "BACK TO PREVIOUS";
                }

                SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate(backButtonText), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
                pages[0].subObjects.Add(backButton);
                backObject = backButton;
                backButton.nextSelectable[0] = backButton;

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

#if !DEBUG
            if (routine != null)
                Main.StopCoroutinePtr(routine);
#endif
        }
#if !DEBUG
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

        private static string RemoveStructures(string text)
        {
            string result = text;
            int currentPosition = 0;
            while ((currentPosition = result.IndexOf('<', currentPosition)) != -1)
            {
                int lastPosition = result.IndexOf('>', currentPosition);
                int eqp = result.IndexOf('=', currentPosition);
                int lEqp = result.LastIndexOf('=', lastPosition);
                result = result.Replace(result.Substring(currentPosition, lastPosition + 1 - currentPosition), result.Substring(eqp + 1, lEqp - eqp - 1).Trim('\"'));
                currentPosition = lastPosition + 1;
            }
            return result;
        }
#endif

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
                routine = Main.StartCoroutinePtr(PerformTextAnimation(RemoveStructures(entry.Info.Description.ToString()).WrapText(WrapCount), screenSize));
            }
            else
            {
                MenuLabel label = new MenuLabel(this, pages[0], RemoveStructures(entry.Info.Description.ToString()).WrapText(WrapCount), new Vector2(screenSize.x / 2f, screenSize.y / 2f), Vector2.one, false);
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
                        scale = 1f,
                        color = entry.Info.Description[i].UnlockPipColor
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

                    manager.RequestMainProcessSwitch(Main.BestiaryReadingMenu, BestiarySettings.MenuFadeTimeSeconds);
                }
                else
                    manager.RequestMainProcessSwitch(Main.BestiaryEntryMenu, BestiarySettings.MenuFadeTimeSeconds);
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

                manager.RequestMainProcessSwitch(Main.BestiaryEntryMenu, BestiarySettings.MenuFadeTimeSeconds);
            }

            if (message.StartsWith(ENTRY_REFERENCE_ID))
            {
                Bestiary.EnteringMenu = true;

                try
                {
                    string referenceID = message.Substring(message.IndexOf(';') + 1);
                    Entry entry = Bestiary.GetEntryByReferenceID(referenceID);

                    if (entry != null)
                    {
                        PlaySound(SoundID.MENU_Switch_Page_In);

                        Bestiary.PreviousEntriesChain.Insert(0, Bestiary.CurrentSelectedEntry);
                        Bestiary.CurrentSelectedEntry = entry;
                        manager.RequestMainProcessSwitch(Main.BestiaryReadingMenu, BestiarySettings.MenuFadeTimeSeconds);
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
                _Objects.AddRange(FormatHorizontalText(line, screenSize, currentY, menu, owner));

                currentY -= LineSpacing;
            }
        }

        public enum StructureType
        {
            PlainText = 0, Plain = 0, Text = 0, Txt = 0,
            Reference = 1, Ref = 1, Refer = 1,
            Color = 2, Clr = 2, Colour = 2, Rgb = 2
        }
        public class StructureData
        {
            public StructureType Type;
            public string Message;
            public string OtherData = string.Empty;

            public StructureData(StructureType type, string message)
            {
                Type = type;
                Message = message;
            }
            public StructureData(StructureType type, string message, string otherData)
                : this(type, message)
            {
                OtherData = otherData;
            }
        }

        // Current Valid Structures
        // References:  <ref="Hello!"=Rain World/creaturetype-Fly>
        // Colors:      <color="Hello!"=FFFFFF>

        private List<MenuObject> FormatHorizontalText(string text, in Vector2 screenSize, in int Y, in Menu.Menu menu, in MenuObject owner)
        {
            List<float> sizes = new List<float>();

            int currentLookPosition = 0;
            int LessThanIndex;
            List<StructureData> structures = new List<StructureData>();
            while ((LessThanIndex = text.IndexOf('<', currentLookPosition)) != -1)
            {
                StructureData structure = new StructureData(StructureType.PlainText, text.Substring(currentLookPosition, LessThanIndex - currentLookPosition), string.Empty);
                sizes.Add(structure.Message.Length);
                structures.Add(structure);

                structure = DecipherStructure(in text, LessThanIndex + 1, out currentLookPosition);
                sizes.Add(structure.Message.Length);
                structures.Add(structure);
            }

            string remainder = text.Substring(currentLookPosition);
            structures.Add(new StructureData(StructureType.PlainText, remainder, string.Empty));
            sizes.Add(remainder.Length);

            List<MenuObject> result = new List<MenuObject>();

            float currentX = (screenSize.x - (sizes.Sum() * 5.3f)) / 2f;
            int currentSizeIndex = 0;
            float currentSizeValue = 0;
            foreach (StructureData structure in structures)
            {
                switch (structure.Type)
                {
                    case StructureType.Reference:
                        {
                            float xSize = sizes[currentSizeIndex] * 1.5f;

                            SimpleButton button = new SimpleButton(menu, owner, structure.Message, EntryReadingMenu.ENTRY_REFERENCE_ID + structure.OtherData,
                                new Vector2(currentX - xSize + currentSizeValue, Y - 10f), new Vector2(sizes[currentSizeIndex] * 5.3f * 1.5f, 20f))
                            {
                                rectColor = new HSLColor(0f, 0f, 0f),
                                labelColor = new HSLColor(0f, 1f, 1f)
                            };

                            result.Add(button);

                            currentSizeValue += 10f;
                        }
                        break;
                    case StructureType.Colour:
                        {
                            MenuLabel label = new MenuLabel(menu, owner, structure.Message, new Vector2(currentX + currentSizeValue + 20f, Y), Vector2.one, false);

                            label.label.color = structure.OtherData.HexToColor();
                            label.label.alignment = FLabelAlignment.Left;

                            result.Add(label);

                            currentSizeValue += 20f;
                        }
                        break;
                    default:
                        {
                            MenuLabel label = new MenuLabel(menu, owner, structure.Message, new Vector2(currentX + currentSizeValue, Y), Vector2.one, false);

                            label.label.alignment = FLabelAlignment.Left;

                            result.Add(label);

                            currentSizeValue += 10f;
                        }
                        break;
                }

                currentX += sizes[currentSizeIndex] * 5.3f;
                currentSizeIndex++;
            }

            return result;
        }
        private static StructureData DecipherStructure(in string text, int startingPosition, out int lastPosition)
        {
            lastPosition = text.IndexOf('>', startingPosition, true);

            string t = text.Substring(startingPosition, lastPosition - startingPosition);
            string[] split = SplitStructure(t);

            Enum.TryParse(split[0], true, out StructureType type);
            string message = type != StructureType.PlainText ? split[1].Trim('\"') : t;
            string otherData = split.Length > 2 ? split[2] : string.Empty; ;

            ++lastPosition;
            return new StructureData(type, message, otherData);
        }
        private static string[] SplitStructure(string text)
        {
            int firstEquals = text.IndexOf('='), lastEquals = text.LastIndexOf('=');
            return new string[3] { text.Substring(0, firstEquals), text.Substring(firstEquals + 1, lastEquals - firstEquals - 1), text.Substring(lastEquals + 1) };
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
