﻿using Menu;
using RainWorldBestiary.Types;
using System.IO;
using UnityEngine;

namespace RainWorldBestiary.Menus
{
    internal class BestiaryEntryMenu : Dialog
    {
        readonly string BackButtonMessage = "BACK";
        readonly string EntryPressedID = "Read_Entry_";

        private bool Closing = false;

        public BestiaryEntryMenu(ProcessManager manager) : base(manager)
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
                alpha = 0.80f
            };
            pages[0].Container.AddChild(darkSprite);

            SimpleButton backButton = new SimpleButton(this, pages[0], Translator.Translate("BACK"), BackButtonMessage, new Vector2(leftAnchor + 15f, 25f), new Vector2(220f, 30f));
            pages[0].subObjects.Add(backButton);

            backObject = backButton;

            CreateEntryButtonsFromTab(Bestiary.CurrentSelectedTab, in screenSize, out MenuObject firstEntryButton);

            SharedMenuUtilities.AddMenuTitleIllustration(this, pages[0], Bestiary.CurrentSelectedTab, in screenSize);

            if (Bestiary.EnteringMenu)
                selectedObject = firstEntryButton;
            else
                backObject.nextSelectable[0] = backButton;

            TipLabel = new MenuLabel(this, pages[0], "", new Vector2(screenSize.x / 2f, 25f), Vector2.one, false);
            pages[0].subObjects.Add(TipLabel);

            mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
        }

        private readonly int ButtonSizeX = 155;
        private readonly int ButtonSizeY = 35;
        private readonly int ButtonSpacing = 15;
        private readonly int MaxButtonsPerRow = 8;

        private readonly HSLColor LockedColor = new HSLColor(0f, 0.55f, 0.85f);

        public void CreateEntryButtonsFromTab(EntriesTab tab, in Vector2 screenSize, out MenuObject firstEntryButton)
        {
            float currentX = ButtonSpacing;
            float currentY = screenSize.y - 100f;

            Vector2 buttonSize = new Vector2(ButtonSizeX, ButtonSizeY);

            firstEntryButton = null;

            for (int i = 0; i < tab.Count; i++)
            {
                if (i % MaxButtonsPerRow == 0)
                {
                    currentY -= ButtonSizeY + ButtonSpacing;
                    currentX = ButtonSpacing;
                }

                bool entryLocked = !tab[i].Info.EntryUnlocked;

                SimpleButton button = new SimpleButton(this, pages[0], entryLocked ? "???" : Translator.Translate(tab[i].Name), string.Concat(EntryPressedID, tab[i].Name), new Vector2(currentX, currentY), buttonSize)
                {
                    rectColor = entryLocked ? LockedColor : tab[i].Info.EntryColor
                };
                pages[0].subObjects.Add(button);

                if (i == 0)
                    firstEntryButton = button;

                float iconOffset = 0;
                for (int j = 0; j < tab[i].Info.EntryIcons.Length; j++)
                {
                    MenuIllustration illustration = new MenuIllustration(this, pages[0], Path.GetDirectoryName(tab[i].Info.EntryIcons[j]), Path.GetFileName(tab[i].Info.EntryIcons[j]),
                        new Vector2(currentX + 5f + iconOffset, currentY + (ButtonSizeY / 2f)), true, true)
                    {
                        color = entryLocked ? Color.black : Color.white
                    };

                    pages[0].subObjects.Add(illustration);
                    iconOffset += illustration.sprite.width;
                }

                currentX += ButtonSizeX + ButtonSpacing;
            }
        }

        private readonly MenuLabel TipLabel;
        private float TipLabelAlpha;

        public override void Singal(MenuObject sender, string message)
        {
            if (message.Equals(BackButtonMessage))
            {
                Closing = true;

                Bestiary.EnteringMenu = false;

                PlaySound(SoundID.MENU_Switch_Page_Out);
                manager.RequestMainProcessSwitch(Main.BestiaryTabMenu, BestiarySettings.MenuFadeTimeSeconds);
                return;
            }

            if (message.StartsWith(EntryPressedID))
            {
                Bestiary.EnteringMenu = true;

                string msg = message.Substring(EntryPressedID.Length);

                foreach (Entry entry in Bestiary.CurrentSelectedTab)
                {
                    if (entry.Name.Equals(msg))
                    {
                        Bestiary.CurrentSelectedEntry = entry;
                        break;
                    }
                }

                if (Bestiary.CurrentSelectedEntry.Info.EntryUnlocked)
                {
                    PlaySound(SoundID.MENU_Switch_Page_In);
                    manager.RequestMainProcessSwitch(Bestiary.CurrentSelectedEntry.EntryReadingMenu, BestiarySettings.MenuFadeTimeSeconds);
                }
                else
                {
                    PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);

                    TipLabel.text = Translator.Translate(Bestiary.CurrentSelectedEntry.Info.LockedText);
                    TipLabelAlpha = Mathf.Clamp(TipLabel.text.Length * 0.04f, 1.5f, 5f);
                }
            }
        }

        public override void ShutDownProcess()
        {
            base.ShutDownProcess();
            TipLabel.RemoveSprites();
        }

        public override void Update()
        {
            if (TipLabelAlpha > 0)
            {
                TipLabelAlpha -= Time.deltaTime / Mathf.Clamp(TipLabelAlpha, 0f, 1f);
                TipLabel.label.color = new Color(1f, 1f, 1f, TipLabelAlpha);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !Closing)
                Singal(backObject, BackButtonMessage);

            base.Update();
        }
    }
}