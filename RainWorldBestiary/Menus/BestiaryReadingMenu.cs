using Menu;
using RainWorldBestiary.Types;
using System.Collections;
using System.IO;
using UnityEngine;

namespace RainWorldBestiary.Menus
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

            darkSprite.alpha = 0.9f;

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
            Bestiary.DoAnimation = true;
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
        }

        private IEnumerator PerformTextAnimation(EntryTextDisplay display, Vector2 screenSize)
        {
            int characters = display.PredictedTextLength / 100;

            FSprite[] sprites = new FSprite[characters];

            // Local Scope For Variable Cleanup
            {
                string[] elements = MenuResources.Instance.Characters.GetRandom(characters);

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

            Enumerators.ForceCompleteEnumerators(PopulateDisplayID);
            Enumerators.StartEnumerator(display.Animate(pages[0], sprites));
        }

        private int PopulateDisplayID = -1;
        public void DisplayEntryInformation(Entry entry, in Vector2 screenSize)
        {
            float widthOffset, leftSpriteOffset = 60;

            if (BestiarySettings.PerformTextAnimations.Value && Bestiary.DoAnimation)
            {
                EntryTextDisplay display = new EntryTextDisplay();
                PopulateDisplayID = Enumerators.StartEnumerator(display.Populate(entry.Info.Description.ToString().WrapText(WrapCount), screenSize, this, pages[0]));
                Enumerators.StartEnumerator(PerformTextAnimation(display, screenSize));
            }
            else
            {
                EntryTextDisplay.CreateAndAdd(entry.Info.Description.ToString().WrapText(WrapCount), in screenSize, this, pages[0]);
            }

            SharedMenuUtilities.AddMenuTitleIllustration(this, pages[0], entry, in screenSize, out float spriteWidth);

            widthOffset = spriteWidth / 2f;

            if (entry.Info.IconsNextToTitle)
            {
                float iconOffset = 0;
                for (int i = 0; i < entry.Info.EntryIcons.Length; i++)
                {
                    if (entry.Info.IconsNextToTitle && Futile.atlasManager.DoesContainElementWithName(entry.Info.EntryIcons[i]))
                    {
                        MenuIllustration illustration = new MenuIllustration(this, pages[0], Path.GetDirectoryName(entry.Info.EntryIcons[i]), Path.GetFileName(entry.Info.EntryIcons[i]), 
                            new Vector2(screenSize.x / 2f - (widthOffset + leftSpriteOffset) - iconOffset, screenSize.y - 50), true, true)
                        {
                            sprite =
                            {
                                scale = 2f
                            }
                        };
                        pages[0].subObjects.Add(illustration);

                        illustration = new MenuIllustration(this, pages[0], Path.GetDirectoryName(entry.Info.EntryIcons[i]), Path.GetFileName(entry.Info.EntryIcons[i]),
                            new Vector2(screenSize.x / 2f + (widthOffset + 10) + iconOffset, screenSize.y - 50), true, true)
                        {
                            sprite =
                            {
                                scale = 2f
                            }
                        };
                        pages[0].subObjects.Add(illustration);

                        iconOffset += illustration.sprite.width;
                    }
                }
            }

            if (BestiarySettings.ShowModuleLockPips.Value)
            {
                for (int i = 0; i < entry.Info.Description.Count; i++)
                {
                    FSprite pip = new FSprite(entry.Info.Description[i].ModuleUnlocked ? MenuResources.Instance.UnlockPipUnlocked : MenuResources.Instance.UnlockPip)
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

                    Bestiary.DoAnimation = false;
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
