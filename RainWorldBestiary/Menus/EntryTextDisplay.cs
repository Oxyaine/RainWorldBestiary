using Menu;
using RainWorldBestiary.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal class EntryTextDisplay
    {
        public int TotalLength = 0;
        public int PredictedTextLength = 0;
        private readonly int LineSpacing = 20;
        private readonly List<MenuObject> _Objects = new List<MenuObject>();

        private enum MenuObjectType : byte
        {
            Unknown,
            Label,
            SimpleButton
        }
        public IEnumerator Animate(MenuObject owner, FSprite[] characterSprites)
        {
            if (_Objects.Count == 0)
                yield break;

            int currentObjectIndex = 0, currentTextPosition = 0, CurrentLength = 0, AmountRevealed = 0, CurrentSpriteIndex = 0;
            int spriteGapBeforeFade = TotalLength / characterSprites.Length;
            int currentSpriteCapBeforeFade = spriteGapBeforeFade;
            const int RevealSpeed = 10;
            bool newLabel = true;
            string text = string.Empty;

            while (currentObjectIndex < _Objects.Count)
            {
                MenuObjectType type = GetMenuObjectType(_Objects[currentObjectIndex], out MovingSimpleButton button, out MenuLabel label);

                if (type == MenuObjectType.SimpleButton)
                {
                    owner.subObjects.Add(button.ToSimpleButton());
                    AmountRevealed += button.menuLabel.text.Length;
                }
                else if (type == MenuObjectType.Label)
                {
                    if (newLabel)
                    {
                        text = label.text;
                        CurrentLength = text.Length;

                        label.text = string.Empty;
                        owner.subObjects.Add(label);
                    }

                    while (currentTextPosition + RevealSpeed < CurrentLength)
                    {
                        label.text += text.Substring(currentTextPosition, RevealSpeed);
                        currentTextPosition += RevealSpeed;
                        AmountRevealed += RevealSpeed;

                        if (AmountRevealed >= currentSpriteCapBeforeFade)
                        {
                            currentSpriteCapBeforeFade += spriteGapBeforeFade;
                            Enumerators.StartEnumerator(FadeIconAnimation(characterSprites[CurrentSpriteIndex]));
                            ++CurrentSpriteIndex;
                        }

                        yield return new WaitTime(0.01f);
                    }

                    label.text += text.Substring(currentTextPosition);
                    AmountRevealed += CurrentLength - currentTextPosition;
                    currentTextPosition = 0;
                    newLabel = true;
                }
                else
                {
                    Main.Logger.LogError(_Objects[currentObjectIndex].GetType().Name + " has no animation behaviour for text revealing.");
                }

                ++currentObjectIndex;

                if (AmountRevealed >= currentSpriteCapBeforeFade)
                {
                    currentSpriteCapBeforeFade += spriteGapBeforeFade;
                    Enumerators.StartEnumerator(FadeIconAnimation(characterSprites[CurrentSpriteIndex]));
                    ++CurrentSpriteIndex;
                }

                yield return new WaitTime(0.005f);
            }
        }
        private IEnumerator FadeIconAnimation(FSprite sprite)
        {
            //PlaySound(SoundID.HUD_Food_Meter_Deplete_Plop_A);

            while (sprite.alpha > 0)
            {
                sprite.alpha -= Time.deltaTime * 2 / sprite.alpha;
                sprite.scale += 0.05f;

                yield return null;
            }

            sprite.RemoveFromContainer();
        }
        private static MenuObjectType GetMenuObjectType(MenuObject @object, out MovingSimpleButton simpleButton, out MenuLabel label)
        {
            switch (@object)
            {
                case MovingSimpleButton movingSimpleButton:
                    simpleButton = movingSimpleButton;
                    label = null;
                    return MenuObjectType.SimpleButton;
                case MenuLabel menuLabel:
                    label = menuLabel;
                    simpleButton = null;
                    return MenuObjectType.Label;

                default:
                    label = null;
                    simpleButton = null;
                    return MenuObjectType.Unknown;
            }
        }

        public EntryTextDisplay() { }
        public EntryTextDisplay(string wrappedText, in Vector2 screenSize, in Menu.Menu menu, in MenuObject owner)
        {
            PredictedTextLength = wrappedText.Length;

            string[] split = wrappedText.Split('\n');
            int currentY = GetStartingYPosition(split.Length, (int)screenSize.y);

            foreach (string line in split)
            {
                Enumerators.CompleteEnumerator(FormatHorizontalText(line, screenSize, currentY, menu, owner, false));
                currentY -= LineSpacing;
            }
        }

        public IEnumerator Populate(string wrappedText, Vector2 screenSize, Menu.Menu menu, MenuObject owner)
        {
            PredictedTextLength = wrappedText.Length;

            string[] split = wrappedText.Split('\n');
            int currentY = GetStartingYPosition(split.Length, (int)screenSize.y);

            foreach (string line in split)
            {
                IEnumerator enumerator = FormatHorizontalText(line, screenSize, currentY, menu, owner, true);

                while (enumerator.MoveNext())
                    yield return null;

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

        private IEnumerator FormatHorizontalText(string text, Vector2 screenSize, int Y, Menu.Menu menu, MenuObject owner, bool buttonsOffScreen = true)
        {
            List<int> sizes = new List<int>();
            List<StructureData> structures = new List<StructureData>();

            // Scope 1
            {
                int currentPosition = 0;
                int LessThanIndex;
                while ((LessThanIndex = text.IndexOf('<', currentPosition)) != -1)
                {
                    StructureData structure = new StructureData(StructureType.PlainText, text.Substring(currentPosition, LessThanIndex - currentPosition));
                    sizes.Add(structure.Message.Length);
                    structures.Add(structure);

                    structure = DecipherStructure(in text, LessThanIndex + 1, out currentPosition);
                    sizes.Add(structure.Message.Length);
                    structures.Add(structure);

                    yield return null;
                }
                // Scope 2
                {
                    string remainder = text.Substring(currentPosition);
                    structures.Add(new StructureData(StructureType.PlainText, remainder, string.Empty));
                    sizes.Add(remainder.Length);
                }
            }

            int sum = sizes.Sum();
            TotalLength += sum;

            float currentX = (screenSize.x - (sum * 5.3f)) / 2f;
            int currentSizeIndex = 0;
            float currentSizeValue = 0;
            foreach (StructureData structure in structures)
            {
                switch (structure.Type)
                {
                    case StructureType.Reference:
                        {
                            float xSize = sizes[currentSizeIndex] * 1.5f;

                            bool entryAvailable = true;
                            Entry ent = Bestiary.GetEntryByReferenceID(structure.OtherData);
                            if (ent == null || !ent.Info.EntryUnlocked)
                                entryAvailable = false;

                            Vector2 Position = new Vector2(currentX - xSize + currentSizeValue, Y - 10f);
                            MovingSimpleButton button = new MovingSimpleButton(menu, owner, structure.Message, BestiaryReadingMenu.ENTRY_REFERENCE_ID + structure.OtherData,
                                buttonsOffScreen ? new Vector2(-100f, -100f) : Position, Position, new Vector2(sizes[currentSizeIndex] * 5.3f * 1.5f, 20f))
                            {
                                rectColor = new HSLColor(0f, 0f, 0f),
                                labelColor = new HSLColor(0f, 1f, 1f),
                                inactive = !entryAvailable
                            };

                            _Objects.Add(button);

                            currentSizeValue += 10f;
                        }
                        break;
                    case StructureType.Colour:
                        {
                            MenuLabel label = new MenuLabel(menu, owner, structure.Message, new Vector2(currentX + currentSizeValue + 20f, Y), Vector2.one, false);

                            label.label.color = structure.OtherData.HexToColor();
                            label.label.alignment = FLabelAlignment.Left;

                            _Objects.Add(label);

                            currentSizeValue += 20f;
                        }
                        break;
                    default:
                        {
                            MenuLabel label = new MenuLabel(menu, owner, structure.Message, new Vector2(currentX + currentSizeValue, Y), Vector2.one, false);

                            label.label.alignment = FLabelAlignment.Left;

                            _Objects.Add(label);

                            currentSizeValue += 10f;
                        }
                        break;
                }

                currentX += sizes[currentSizeIndex] * 5.3f;
                currentSizeIndex++;
                yield return null;
            }
        }
        private static StructureData DecipherStructure(in string text, int startingPosition, out int lastPosition)
        {
            lastPosition = text.IndexOf('>', startingPosition, true);

            string t = text.Substring(startingPosition, lastPosition - startingPosition);
            string[] split = SplitStructure(t);

            Enum.TryParse(split[0], true, out StructureType type);
            string message = split.Length > 1 ? (type != StructureType.PlainText ? split[1].Trim('\"') : t) : string.Empty;
            string otherData = split.Length > 2 ? split[2] : string.Empty;

            ++lastPosition;
            return new StructureData(type, message, otherData);
        }
        private static string[] SplitStructure(string text)
        {
            int firstEquals = text.IndexOf('='), lastEquals = text.LastIndexOf('=');

            if (firstEquals == -1)
                return new string[1] { text };
            else if (lastEquals == firstEquals)
                return new string[2] { text.Substring(0, firstEquals), text.Substring(firstEquals) };
            else
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

    internal class MovingSimpleButton : SimpleButton
    {
        public Vector2 IntendedPosition = Vector2.zero;

        public MovingSimpleButton(Menu.Menu menu, MenuObject owner, string displayText, string singalText, Vector2 currentPosition, Vector2 intendedPosition, Vector2 size) : base(menu, owner, displayText, singalText, currentPosition, size)
        {
            IntendedPosition = intendedPosition;
        }
        public SimpleButton ToSimpleButton()
        {
            pos = IntendedPosition;
            return this;
        }
    }
}
