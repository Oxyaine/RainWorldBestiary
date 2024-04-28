using Menu;
using RainWorldBestiary.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal class EntryTextDisplay
    {
        public int TotalLength = 0;
        public int PredictedTextLength = 0;
        private readonly int LineSpacing = 20;
        private readonly List<PositionedMenuObject> _Objects = new List<PositionedMenuObject>();
        public ReadOnlyCollection<PositionedMenuObject> Objects { get; private set; }
        private readonly List<IAnimatableObject> animatableObjects = new List<IAnimatableObject>();

        public IEnumerator Animate(OverlappingMenu owner, MenuIllustration[] characterSprites, Action<SoundID, float, float, float> playSoundFunction = null)
        {
            if (_Objects.Count == 0)
                yield break;

            Vector2 screenSize = owner.manager.rainWorld.options.ScreenSize;

            int currentObjectIndex = 0, AmountRevealed = 0, CurrentSpriteIndex = 0;
            int spriteGapBeforeFade = TotalLength / characterSprites.Length;
            int currentSpriteCapBeforeFade = spriteGapBeforeFade;

            const int RevealSpeed = 15;

            while (currentObjectIndex < _Objects.Count)
            {
                owner.AddMovingObject(_Objects[currentObjectIndex], _Objects[currentObjectIndex].pos + new Vector2(0f, screenSize.y), _Objects[currentObjectIndex].pos);
                IEnumerator enumerator = animatableObjects[currentObjectIndex].Animate();

                while (enumerator.MoveNext())
                {
                    AmountRevealed += RevealSpeed;

                    CheckSpriteFading();

                    yield return enumerator.Current;
                }

                AmountRevealed += RevealSpeed;
                ++currentObjectIndex;

                yield return null;
            }

            for (int i = CurrentSpriteIndex; i < characterSprites.Length; i++)
                Enumerators.StartEnumerator(FadeIconAnimation(characterSprites[i], playSoundFunction));

            void CheckSpriteFading()
            {
                while (AmountRevealed >= currentSpriteCapBeforeFade && CurrentSpriteIndex < characterSprites.Length)
                {
                    currentSpriteCapBeforeFade += spriteGapBeforeFade;
                    Enumerators.StartEnumerator(FadeIconAnimation(characterSprites[CurrentSpriteIndex], playSoundFunction));
                    ++CurrentSpriteIndex;
                }
            }
        }
        private IEnumerator FadeIconAnimation(MenuIllustration sprite, Action<SoundID, float, float, float> playSoundFunction = null)
        {
            playSoundFunction?.Invoke(SoundID.HUD_Food_Meter_Deplete_Plop_A, 0f, 1.5f, 1f);

            while (sprite.alpha > 0)
            {
                sprite.alpha -= Time.deltaTime * 2 / sprite.alpha;
                sprite.sprite.scale += 0.05f;

                yield return null;
            }

            sprite.RemoveSprites();
            sprite.owner.RemoveSubObject(sprite);
        }

        public EntryTextDisplay() { }
        public EntryTextDisplay(string wrappedText, in Vector2 screenSize, in Menu.Menu menu, in MenuObject owner, bool buttonsOffScreen = true)
        {
            PredictedTextLength = wrappedText.Length;

            string[] split = wrappedText.Split('\n');
            int currentY = GetStartingYPosition(split.Length, (int)screenSize.y);

            foreach (string line in split)
            {
                FormatHorizontalText(line, in screenSize, in currentY, menu, owner, buttonsOffScreen);

                currentY -= LineSpacing;
            }

            Objects = _Objects.AsReadOnly();
        }

        public enum StructureType
        {
            PlainText = 0, Plain = 0, Text = 0, Txt = 0,
            Color = 0, Clr = 0, Colour = 0, Rgb = 0,
            Reference = 1, Ref = 1, Refer = 1,
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
        // Colors:      <color="Hello!"=FFFFFF> || <color="Hello!">
        // Text:        <text="Hello!"=FFFFFF> || <text="Hello!">

        private void FormatHorizontalText(string text, in Vector2 screenSize, in int Y, Menu.Menu menu, MenuObject owner, bool buttonsOffScreen = true)
        {
            ParseHorizontalText(text, out List<int> sizes, out List<StructureData> structures);

            int sum = sizes.Sum();
            TotalLength += sum;

            const float AverageCharacterSize = 5.3f;

            float currentX = (screenSize.x - (sum * AverageCharacterSize)) / 2f;
            for (int i = 0; i < structures.Count; i++)
            {
                if (structures[i].Type == StructureType.Reference)
                {
                    Vector2 position = new Vector2(currentX - 6f, Y - 10f);
                    float buttonSize = sizes[i] * AverageCharacterSize + 20f;
                    AnimatableSimpleButton button = new AnimatableSimpleButton(menu, owner, structures[i].Message, EntryMenu.ENTRY_REFERENCE_ID + structures[i].OtherData,
                        buttonsOffScreen ? new Vector2(-100f, -100f) : position, position, new Vector2(buttonSize, 20f))
                    {
                        rectColor = new HSLColor(0f, 0f, 0f),
                        labelColor = new HSLColor(0f, 1f, 1f)
                    };

                    _Objects.Add(button);
                    animatableObjects.Add(button);

                    currentX += buttonSize;
                }
                else
                {
                    AnimatableLabel label = new AnimatableLabel(menu, owner, structures[i].Message, new Vector2(currentX, Y), Vector2.one, false);

                    label.label.color = structures[i].OtherData.HexToColor();
                    label.label.alignment = FLabelAlignment.Left;

                    _Objects.Add(label);
                    animatableObjects.Add(label);

                    currentX += sizes[i] * AverageCharacterSize + 12f;
                }
            }
        }
        private static void ParseHorizontalText(string text, out List<int> sizes, out List<StructureData> structures)
        {
            sizes = new List<int>();
            structures = new List<StructureData>();

            int currentPosition = 0;
            int LessThanIndex = text.IndexOf('<');
            while (LessThanIndex != -1)
            {
                StructureData plainStructure = new StructureData(StructureType.PlainText, text.Substring(currentPosition, LessThanIndex - currentPosition));
                sizes.Add(plainStructure.Message.Length);
                structures.Add(plainStructure);

                StructureData structure = DecipherStructure(in text, LessThanIndex + 1, out currentPosition);
                sizes.Add(structure.Message.Length);
                structures.Add(structure);

                LessThanIndex = text.IndexOf('<', currentPosition);
            }

            string remainder = text.Substring(currentPosition);
            sizes.Add(remainder.Length);
            structures.Add(new StructureData(StructureType.PlainText, remainder, string.Empty));
        }

        private static StructureData DecipherStructure(in string text, int startingPosition, out int lastPosition)
        {
            lastPosition = text.IndexOf('>', startingPosition, true);

            string t = text.Substring(startingPosition, lastPosition - startingPosition);
            string[] split = SplitStructure(t);

            Enum.TryParse(split[0], true, out StructureType type);
            string message = split.Length > 1 ? split[1].Trim('\"') : string.Empty;
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
        public static void CreateAndAdd(string wrappedText, in Vector2 screenSize, Menu.Menu menu, MenuObject owner, bool buttonsOffScreen = false)
            => new EntryTextDisplay(wrappedText, in screenSize, in menu, in owner, buttonsOffScreen).AddToPage(ref owner);
    }
}
