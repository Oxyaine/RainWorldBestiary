using Menu;
using System;
using System.Collections;
using UnityEngine;

namespace RainWorldBestiary
{
    internal class BestiaryReadingMenu : Dialog
    {
        internal const string ENTRY_REFERENCE_ID = "Reference_To;";
        readonly string ReturnButtonMessage = "RETURN";

        private bool Closing = false;

        readonly int WrapCount = 180;

        readonly string BackButtonMessage = "BACK";
        public BestiaryReadingMenu(ProcessManager manager) : base(manager)
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

            DisplayEntryInformation(Bestiary.CurrentSelectedEntry, in screenSize);

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
        }

        private IEnumerator PerformTextAnimation(EntryTextDisplay display, Vector2 screenSize)
        {
            int characters = display.TotalLength / 100;

            if (characters == 0 && display.TotalLength > 0)
                characters = 1;

            FSprite[] sprites = new FSprite[characters];

            // Local Scope For Variable Cleanup
            {
                string[] elements = Bestiary.MenuResources.Characters.GetRandom(characters);

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

                    PlaySound(SoundID.SS_AI_Text, 0f, 1.75f, 1f);
                    yield return new WaitTime(0.05f);
                }
            }

            yield return new WaitTime(0.3f);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < sprites.Length; j++)
                    sprites[j].RemoveFromContainer();

                yield return new WaitTime(0.3f);


                for (int j = 0; j < sprites.Length; j++)
                    pages[0].Container.AddChild(sprites[j]);
                PlaySound(SoundID.SS_AI_Text_Blink, 0f, 1.75f, 1f);

                yield return new WaitTime(0.2f);
            }

            Enumerators.StartEnumerator(display.Animate(pages[0], sprites));
        }

        public void DisplayEntryInformation(Entry entry, in Vector2 screenSize)
        {
            float widthOffset, leftSpriteOffset = 60;

            try
            {
                if (BestiarySettings.PerformTextAnimations.Value)
                {
                    Enumerators.StartEnumerator(PerformTextAnimation(new EntryTextDisplay(entry.Info.Description.ToString().WrapText(WrapCount), in screenSize, this, pages[0]), screenSize));
                }
                else
                {
                    EntryTextDisplay.CreateAndAdd(entry.Info.Description.ToString().WrapText(WrapCount), in screenSize, this, pages[0]);
                }
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex.ToString());
            }

            FSprite[] titleSprites = SharedMenuUtilities.GetMenuTitle(entry, in screenSize, out float spriteWidth);
            foreach (FSprite sprite in titleSprites)
                pages[0].Container.AddChild(sprite);

            widthOffset = spriteWidth / 2f;

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
                    FSprite pip = new FSprite(entry.Info.Description[i].ModuleUnlocked ? Bestiary.MenuResources.UnlockPipUnlocked : Bestiary.MenuResources.UnlockPip)
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
                Closing = true;

                Bestiary.EnteringMenu = false;
                PlaySound(SoundID.MENU_Switch_Page_Out);

                if (Bestiary.PreviousEntriesChain.Count > 0)
                {
                    Bestiary.CurrentSelectedEntry = Bestiary.PreviousEntriesChain[0];
                    Bestiary.PreviousEntriesChain.RemoveAt(0);

                    manager.RequestMainProcessSwitch(Main.BestiaryReadingMenu, BestiarySettings.MenuFadeTimeSeconds);
                }
                else
                    manager.RequestMainProcessSwitch(Main.BestiaryEntryMenu, BestiarySettings.MenuFadeTimeSeconds);
            }
            else if (message.Equals(ReturnButtonMessage))
            {
                Bestiary.EnteringMenu = false;
                PlaySound(SoundID.MENU_Switch_Page_Out);

                Bestiary.PreviousEntriesChain.Clear();

                manager.RequestMainProcessSwitch(Main.BestiaryEntryMenu, BestiarySettings.MenuFadeTimeSeconds);
            }
            else if (message.StartsWith(ENTRY_REFERENCE_ID))
            {
                Bestiary.EnteringMenu = true;

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
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !Closing)
                Singal(backObject, BackButtonMessage);

            base.Update();
        }
    }
}
