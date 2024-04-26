using Menu;
using RainWorldBestiary.Types;
using System.Collections;
using System.IO;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal class EntryMenu : OverlappingMenu, IOverlappingMenuOwner
    {
        internal const string ENTRY_REFERENCE_ID = "Reference_To;";
        readonly string ReturnButtonMessage = "RETURN";

        private bool Closing = false, InSubMenu = false;

        readonly int WrapCount = 180;

        private readonly Entry DisplayedEntry;

        readonly SimpleButton backButton;
        readonly string BackButtonMessage = "BACK";
        public EntryMenu(ProcessManager manager, Entry entry, IOverlappingMenuOwner parentMenu) : base(manager, parentMenu)
        {
            DisplayedEntry = entry;

            Vector2 screenSize = manager.rainWorld.options.ScreenSize;

            float leftAnchor = (1366f - manager.rainWorld.options.ScreenSize.x) / 2f;

            backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
            backButton.nextSelectable[0] = backButton;
            backObject = backButton;
            pages[0].subObjects.Add(backButton);

            if (Bestiary.ReadingMenusDeep > 0)
            {
                SimpleButton returnButton = new SimpleButton(this, pages[0], Translator.Translate("RETURN TO ENTRIES"), ReturnButtonMessage, new Vector2(leftAnchor + 250f, -30f), new Vector2(220f, 30f));
                AddMovingObject(returnButton, new Vector2(leftAnchor + 250f, 25f));

                if (Bestiary.ReadingMenusDeep > 1)
                {
                    backButton.menuLabel.text = Translator.Translate("BACK TO PREVIOUS");
                }
                else
                {
                    Enumerators.StartEnumerator(SharedMenuUtilities.AnimateTextSwitch(backButton.menuLabel, Translator.Translate("BACK TO PREVIOUS")));
                }
            }

            DisplayEntryInformation(DisplayedEntry, in screenSize);

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;

            Bestiary.ClosingAllReadingMenus = false;

            backObject.nextSelectable[0] = backObject;
            backObject.nextSelectable[1] = backObject;
            backObject.nextSelectable[2] = backObject;
            backObject.nextSelectable[3] = backObject;

            selectedObject = backObject;
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
        }

        private IEnumerator PerformTextAnimation(Vector2 screenSize)
        {
            int characters = Display.PredictedTextLength / 100;

            MenuIllustration[] sprites = new MenuIllustration[characters];

            // Local Scope For Variable Cleanup
            {
                string[] elements = MenuResources.Instance.Characters.GetRandom(characters);

                int position = characters * 16 * 3;

                float currentX = (screenSize.x / 2f) - (position / 2f), currentY = screenSize.y - 175f;
                for (int i = 0; i < elements.Length; i++)
                {
                    sprites[i] = new MenuIllustration(this, pages[0], MenuResources.Instance.IllustrationsIconsPath, elements[i], new Vector2(currentX, currentY), true, true)
                    {
                        sprite =
                        {
                            scale = 3f
                        }
                    };

                    AddMovingObject(sprites[i], new Vector2(currentX, currentY + screenSize.y), new Vector2(currentX, currentY));

                    currentX += sprites[i].sprite.width;

                    PlaySound(SoundID.SS_AI_Text, 0f, 1.75f, 1f);
                    yield return new WaitTime(0.05f);
                }
            }

            yield return new WaitTime(0.3f);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < sprites.Length; j++)
                    sprites[j].alpha = 0f;

                yield return new WaitTime(0.3f);


                for (int j = 0; j < sprites.Length; j++)
                    sprites[j].alpha = 1f;
                PlaySound(SoundID.SS_AI_Text_Blink, 0f, 1.75f, 1f);

                yield return new WaitTime(0.2f);
            }

            IEnumerator enumerator = Display.Animate(this, sprites, PlaySound);

            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        private EntryTextDisplay Display;
        public void DisplayEntryInformation(Entry entry, in Vector2 screenSize)
        {
            float widthOffset, leftSpriteOffset = 60;

            if (Bestiary.Settings.PerformTextAnimations.Value)
            {
                Display = new EntryTextDisplay(entry.Info.Description.ToString().WrapText(WrapCount), screenSize, this, pages[0]);
                Enumerators.StartEnumerator(PerformTextAnimation(screenSize));
            }
            else
            {
                Display = new EntryTextDisplay(entry.Info.Description.ToString().WrapText(WrapCount), in screenSize, this, pages[0], false);
                foreach (PositionedMenuObject @object in Display.Objects)
                {
                    Vector2 position = @object.pos + new Vector2(0f, screenSize.y);
                    AddMovingObject(@object, position, @object.pos);
                    @object.pos = position;
                }
            }

            MenuIllustration[] illustrations = SharedMenuUtilities.GetMenuTitleIllustration(this, pages[0], entry, in screenSize, out float spriteWidth);
            foreach (MenuIllustration illustration in illustrations)
            {
                Vector2 newPos = illustration.pos + new Vector2(0f, 200f);
                AddMovingObject(illustration, newPos, illustration.pos);
                illustration.pos = newPos;
            }

            widthOffset = spriteWidth / 2f;

            if (entry.Info.IconsNextToTitle)
            {
                float iconOffset = 0;
                for (int i = 0; i < entry.Info.EntryIcons.Length; i++)
                {
                    if (entry.Info.IconsNextToTitle && Futile.atlasManager.DoesContainElementWithName(entry.Info.EntryIcons[i]))
                    {
                        MenuIllustration illustration = new MenuIllustration(this, pages[0], Path.GetDirectoryName(entry.Info.EntryIcons[i]), Path.GetFileName(entry.Info.EntryIcons[i]),
                            new Vector2(screenSize.x / 2f - (widthOffset + leftSpriteOffset) - iconOffset, screenSize.y + 200f), true, true)
                        {
                            sprite =
                            {
                                scale = 2f
                            }
                        };
                        AddMovingObject(illustration, new Vector2(screenSize.x / 2f - (widthOffset + leftSpriteOffset) - iconOffset, screenSize.y - 50));

                        illustration = new MenuIllustration(this, pages[0], Path.GetDirectoryName(entry.Info.EntryIcons[i]), Path.GetFileName(entry.Info.EntryIcons[i]),
                            new Vector2(screenSize.x / 2f + (widthOffset + 10) + iconOffset, screenSize.y + 200f), true, true)
                        {
                            sprite =
                            {
                                scale = 2f
                            }
                        };
                        AddMovingObject(illustration, new Vector2(screenSize.x / 2f + (widthOffset + 10) + iconOffset, screenSize.y - 50));

                        iconOffset += illustration.sprite.width;
                    }
                }
            }

            if (Bestiary.Settings.ShowModuleLockPips.Value)
            {
                for (int i = 0; i < entry.Info.Description.Count; i++)
                {
                    MenuIllustration illustration = new MenuIllustration(this, pages[0], MenuResources.Instance.IllustrationsIconsPath,
                        entry.Info.Description[i].ModuleUnlocked ? MenuResources.Instance.UnlockPipUnlockedName : MenuResources.Instance.UnlockPipName,
                        new Vector2(screenSize.x + 20f, screenSize.y - (i * 10) - 20f), true, true)
                    {
                        sprite =
                        {
                            color = entry.Info.Description[i].UnlockPipColor
                        }
                    };
                    AddMovingObject(illustration, new Vector2(screenSize.x - 20f, screenSize.y - (i * 10) - 20f));
                }
            }
        }

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                Closing = true;

                PlaySound(SoundID.MENU_Switch_Page_Out);

                if (Bestiary.ReadingMenusDeep > 0)
                {
                    --Bestiary.ReadingMenusDeep;
                    if (Bestiary.ReadingMenusDeep == 0)
                        Enumerators.StartEnumerator(SharedMenuUtilities.AnimateDeleteText(backButton.menuLabel));
                }

                CloseMenu();
            }
            else if (message.Equals(ReturnButtonMessage))
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);

                Bestiary.ReadingMenusDeep = 0;
                Bestiary.ClosingAllReadingMenus = true;

                Enumerators.StartEnumerator(SharedMenuUtilities.AnimateDeleteText(backButton.menuLabel));

                CloseMenu();
            }
            else if (message.StartsWith(ENTRY_REFERENCE_ID))
            {
                string referenceID = message.Substring(message.IndexOf(';') + 1);
                Entry entry = Bestiary.GetEntryByReferenceID(referenceID);

                if (entry != null)
                {
                    if (entry.Info.EntryUnlocked)
                    {
                        PlaySound(SoundID.MENU_Switch_Page_In);

                        if (Bestiary.ReadingMenusDeep == 0)
                            Enumerators.StartEnumerator(SharedMenuUtilities.AnimateTextSwitch(backButton.menuLabel, Translator.Translate("BACK TO PREVIOUS")));
                        ++Bestiary.ReadingMenusDeep;

                        InSubMenu = true;
                        manager.ShowDialog(new EntryMenu(manager, entry, this));
                    }
                    else
                    {

                    }
                }
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !Closing && !InSubMenu)
                Singal(backObject, BackButtonMessage);

            base.Update();
        }

        public void ReturningToThisMenu()
        {
            if (Bestiary.ClosingAllReadingMenus)
            {
                Enumerators.StartEnumerator(SharedMenuUtilities.AnimateDeleteText(backButton.menuLabel));
                CloseMenu();
            }
            else if (Bestiary.ReadingMenusDeep == 0)
            {
                Enumerators.StartEnumerator(SharedMenuUtilities.AnimateTextSwitch(backButton.menuLabel, Translator.Translate("BACK")));
            }

            InSubMenu = false;
        }
        public void ClosingSubMenu() { }
    }
}
